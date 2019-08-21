using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    internal class WorkItem
    {
        public byte[] Buffer;
        public byte[] Compressed;
        public int CompressedBytesAvailable;
        public ZlibCodec Compressor;
        public int Crc;
        public int Index;
        public int InputBytesAvailable;
        public int Ordinal;

        public WorkItem(int size,
            CompressionLevel compressLevel,
            CompressionStrategy strategy,
            int ix)
        {
            Buffer = new byte[size];
            var n = size + (size / 32768 + 1) * 5 * 2;
            Compressed = new byte[n];
            Compressor = new ZlibCodec();
            Compressor.InitializeDeflate(compressLevel, false);
            Compressor.OutputBuffer = Compressed;
            Compressor.InputBuffer = Buffer;
            Index = ix;
        }
    }

    public class ParallelDeflateOutputStream : Stream
    {
        private const int IoBufferSizeDefault = 64 * 1024;
        private const int BufferPairsPerCore = 4;

        private List<WorkItem> _pool;
        private readonly bool _leaveOpen;
        private bool _emitting;
        private Stream _outStream;
        private int _maxBufferPairs;
        private int _bufferSize = IoBufferSizeDefault;
        private AutoResetEvent _newlyCompressedBlob;

        private readonly object _outputLock = new object();

        private bool _isClosed;
        private bool _firstWriteDone;
        private int _currentlyFilling;
        private int _lastFilled;
        private int _lastWritten;
        private int _latestCompressed;
        private Crc32 _runningCrc;
        private readonly object _latestLock = new object();
        private Queue<int> _toWrite;
        private Queue<int> _toFill;
        private readonly CompressionLevel _compressLevel;
        private volatile Exception _pendingException;
        private bool _handlingException;
        private readonly object _eLock = new object(); 

        private readonly TraceBits _desiredTrace =
            TraceBits.Session |
            TraceBits.Compress |
            TraceBits.WriteTake |
            TraceBits.WriteEnter |
            TraceBits.EmitEnter |
            TraceBits.EmitDone |
            TraceBits.EmitLock |
            TraceBits.EmitSkip |
            TraceBits.EmitBegin;

        public ParallelDeflateOutputStream(Stream stream)
            : this(stream, CompressionLevel.Default, CompressionStrategy.Default, false)
        {
        }

        public ParallelDeflateOutputStream(Stream stream, CompressionLevel level)
            : this(stream, level, CompressionStrategy.Default, false)
        {
        }

        public ParallelDeflateOutputStream(Stream stream, bool leaveOpen)
            : this(stream, CompressionLevel.Default, CompressionStrategy.Default, leaveOpen)
        {
        }

        public ParallelDeflateOutputStream(Stream stream, CompressionLevel level, bool leaveOpen)
            : this(stream, CompressionLevel.Default, CompressionStrategy.Default, leaveOpen)
        {
        }

        public ParallelDeflateOutputStream(Stream stream,
            CompressionLevel level,
            CompressionStrategy strategy,
            bool leaveOpen)
        {
            _outStream = stream;
            _compressLevel = level;
            Strategy = strategy;
            _leaveOpen = leaveOpen;
            MaxBufferPairs = 16; 
        }

        public CompressionStrategy Strategy { get; }

        public int MaxBufferPairs
        {
            get => _maxBufferPairs;
            set
            {
                if (value < 4)
                    throw new ArgumentException("MaxBufferPairs",
                        "Value must be 4 or greater.");

                _maxBufferPairs = value;
            }
        }

        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                if (value < 1024)
                    throw new ArgumentOutOfRangeException("BufferSize",
                        "BufferSize must be greater than 1024 bytes");

                _bufferSize = value;
            }
        }

        public int Crc32 { get; private set; }

        public long BytesProcessed { get; private set; }

        private void InitializePoolOfWorkItems()
        {
            _toWrite = new Queue<int>();
            _toFill = new Queue<int>();
            _pool = new List<WorkItem>();
            var nTasks = BufferPairsPerCore * Environment.ProcessorCount;
            nTasks = Math.Min(nTasks, _maxBufferPairs);
            for (var i = 0; i < nTasks; i++)
            {
                _pool.Add(new WorkItem(_bufferSize, _compressLevel, Strategy, i));
                _toFill.Enqueue(i);
            }

            _newlyCompressedBlob = new AutoResetEvent(false);
            _runningCrc = new Crc32();
            _currentlyFilling = -1;
            _lastFilled = -1;
            _lastWritten = -1;
            _latestCompressed = -1;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var mustWait = false;

            if (_isClosed)
                throw new InvalidOperationException();

            if (_pendingException != null)
            {
                _handlingException = true;
                var pe = _pendingException;
                _pendingException = null;
                throw pe;
            }

            if (count == 0)
                return;

            if (!_firstWriteDone)
            {
                InitializePoolOfWorkItems();
                _firstWriteDone = true;
            }

            do
            {
                EmitPendingBuffers(false, mustWait);

                mustWait = false;
                int ix;
                if (_currentlyFilling >= 0)
                {
                    ix = _currentlyFilling;
                }
                else
                {
                    if (_toFill.Count == 0)
                    {
                        mustWait = true;
                        continue;
                    }

                    ix = _toFill.Dequeue();
                    ++_lastFilled; 
                }

                var workitem = _pool[ix];

                var limit = workitem.Buffer.Length - workitem.InputBytesAvailable > count
                    ? count
                    : workitem.Buffer.Length - workitem.InputBytesAvailable;

                workitem.Ordinal = _lastFilled;

                Buffer.BlockCopy(buffer,
                    offset,
                    workitem.Buffer,
                    workitem.InputBytesAvailable,
                    limit);

                count -= limit;
                offset += limit;
                workitem.InputBytesAvailable += limit;
                if (workitem.InputBytesAvailable == workitem.Buffer.Length)
                {
                    if (!ThreadPool.QueueUserWorkItem(DeflateOne, workitem))
                        throw new Exception("Cannot enqueue workitem");

                    _currentlyFilling = -1;
                }
                else
                {
                    _currentlyFilling = ix;
                }
            } while (count > 0);
        }

        private void FlushFinish()
        {
            var buffer = new byte[128];
            var compressor = new ZlibCodec
            {
                InputBuffer = null,
                NextIn = 0,
                AvailableBytesIn = 0,
                OutputBuffer = buffer,
                NextOut = 0,
                AvailableBytesOut = buffer.Length
            };

            var rc = compressor.Deflate(FlushType.Finish);

            if (rc != ZlibConstants.ZStreamEnd && rc != ZlibConstants.ZOk)
                throw new Exception("deflating: " + compressor.Message);

            if (buffer.Length - compressor.AvailableBytesOut > 0)
            {
                _outStream.Write(buffer, 0, buffer.Length - compressor.AvailableBytesOut);
            }

            compressor.EndDeflate();

            Crc32 = _runningCrc.Crc32Result;
        }

        private void Flush(bool lastInput)
        {
            if (_isClosed)
                throw new InvalidOperationException();

            if (_emitting)
                return;

            if (_currentlyFilling >= 0)
            {
                var workitem = _pool[_currentlyFilling];
                DeflateOne(workitem);
                _currentlyFilling = -1; 
            }

            if (lastInput)
            {
                EmitPendingBuffers(true, false);
                FlushFinish();
            }
            else
            {
                EmitPendingBuffers(false, false);
            }
        }

        public override void Flush()
        {
            if (_pendingException != null)
            {
                _handlingException = true;
                var pe = _pendingException;
                _pendingException = null;
                throw pe;
            }

            if (_handlingException)
                return;

            Flush(false);
        }

        public override void Close()
        {
            if (_pendingException != null)
            {
                _handlingException = true;
                var pe = _pendingException;
                _pendingException = null;
                throw pe;
            }

            if (_handlingException)
                return;

            if (_isClosed)
                return;

            Flush(true);

            if (!_leaveOpen)
                _outStream.Close();

            _isClosed = true;
        }

        public new void Dispose()
        {
            Close();
            _pool = null;
            Dispose(true);
        }

        public void Reset(Stream stream)
        {
            if (!_firstWriteDone)
                return;

            _toWrite.Clear();
            _toFill.Clear();
            foreach (var workitem in _pool)
            {
                _toFill.Enqueue(workitem.Index);
                workitem.Ordinal = -1;
            }

            _firstWriteDone = false;
            BytesProcessed = 0L;
            _runningCrc = new Crc32();
            _isClosed = false;
            _currentlyFilling = -1;
            _lastFilled = -1;
            _lastWritten = -1;
            _latestCompressed = -1;
            _outStream = stream;
        }

        private void EmitPendingBuffers(bool doAll, bool mustWait)
        {
            if (_emitting)
                return;

            _emitting = true;
            if (doAll || mustWait)
                _newlyCompressedBlob.WaitOne();

            do
            {
                var firstSkip = -1;
                var millisecondsToWait = doAll ? 200 : mustWait ? -1 : 0;
                int nextToWrite;
                do
                {
                    if (Monitor.TryEnter(_toWrite, millisecondsToWait))
                    {
                        nextToWrite = -1;
                        try
                        {
                            if (_toWrite.Count > 0)
                                nextToWrite = _toWrite.Dequeue();
                        }
                        finally
                        {
                            Monitor.Exit(_toWrite);
                        }

                        if (nextToWrite < 0) continue;

                        var workitem = _pool[nextToWrite];
                        if (workitem.Ordinal != _lastWritten + 1)
                        {
                            lock (_toWrite)
                            {
                                _toWrite.Enqueue(nextToWrite);
                            }

                            if (firstSkip == nextToWrite)
                            {
                                _newlyCompressedBlob.WaitOne();
                                firstSkip = -1;
                            }
                            else if (firstSkip == -1)
                            {
                                firstSkip = nextToWrite;
                            }

                            continue;
                        }

                        firstSkip = -1;

                        _outStream.Write(workitem.Compressed, 0, workitem.CompressedBytesAvailable);
                        _runningCrc.Combine(workitem.Crc, workitem.InputBytesAvailable);
                        BytesProcessed += workitem.InputBytesAvailable;
                        workitem.InputBytesAvailable = 0;

                        _lastWritten = workitem.Ordinal;
                        _toFill.Enqueue(workitem.Index);

                        if (millisecondsToWait == -1)
                            millisecondsToWait = 0;
                    }
                    else
                    {
                        nextToWrite = -1;
                    }
                } while (nextToWrite >= 0);
            } while (doAll && _lastWritten != _latestCompressed);

            _emitting = false;
        }

        private void DeflateOne(object wi)
        {
            var workitem = (WorkItem) wi;
            try
            {
                var crc = new Crc32();

                crc.SlurpBlock(workitem.Buffer, 0, workitem.InputBytesAvailable);

                DeflateOneSegment(workitem);

                workitem.Crc = crc.Crc32Result;

                lock (_latestLock)
                {
                    if (workitem.Ordinal > _latestCompressed)
                        _latestCompressed = workitem.Ordinal;
                }

                lock (_toWrite)
                {
                    _toWrite.Enqueue(workitem.Index);
                }

                _newlyCompressedBlob.Set();
            }
            catch (Exception exc1)
            {
                lock (_eLock)
                {
                    if (_pendingException != null)
                        _pendingException = exc1;
                }
            }
        }

        private bool DeflateOneSegment(WorkItem workitem)
        {
            var compressor = workitem.Compressor;
            compressor.ResetDeflate();
            compressor.NextIn = 0;

            compressor.AvailableBytesIn = workitem.InputBytesAvailable;

            compressor.NextOut = 0;
            compressor.AvailableBytesOut = workitem.Compressed.Length;
            do
            {
                compressor.Deflate(FlushType.None);
            } while (compressor.AvailableBytesIn > 0 || compressor.AvailableBytesOut == 0);

            compressor.Deflate(FlushType.Sync);

            workitem.CompressedBytesAvailable = (int) compressor.TotalBytesOut;
            return true;
        }

        [Flags]
        private enum TraceBits : uint
        {
            None = 0,
            NotUsed1 = 1,
            EmitLock = 2,
            EmitEnter = 4, 
            EmitBegin = 8, 
            EmitDone = 16,
            EmitSkip = 32, 
            EmitAll = 58,
            Flush = 64,
            Lifecycle = 128, 
            Session = 256, 
            Synch = 512,
            Instance = 1024,
            Compress = 2048, 
            Write = 4096,
            WriteEnter = 8192, 
            WriteTake = 16384, 
            All = 0xffffffff
        }

        public override bool CanSeek => false;

        public override bool CanRead => false;

        public override bool CanWrite => _outStream.CanWrite;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _outStream.Position;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}