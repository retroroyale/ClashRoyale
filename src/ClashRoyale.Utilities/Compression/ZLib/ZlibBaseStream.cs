using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    internal enum ZlibStreamFlavor
    {
        Zlib = 1950,
        Deflate = 1951,
        Gzip = 1952
    }

    internal class ZlibBaseStream : Stream
    {
        protected internal ZlibCodec Z; 

        protected internal StreamMode _streamMode = StreamMode.Undefined;
        protected internal FlushType FlushMode;
        protected internal ZlibStreamFlavor Flavor;
        protected internal CompressionMode CompressionMode;
        protected internal CompressionLevel Level;
        protected internal bool LeaveOpen;
        protected internal byte[] _workingBuffer;
        protected internal int BufferSize = ZlibConstants.WorkingBufferSizeDefault;
        protected internal byte[] Buf1 = new byte[1];

        protected internal Stream Stream;
        protected internal CompressionStrategy Strategy = CompressionStrategy.Default;

        private readonly Crc32 _crc;

        protected internal string GzipFileName;
        protected internal string GzipComment;
        protected internal DateTime GzipMtime;
        protected internal int GzipHeaderByteCount;

        internal int Crc32 => _crc?.Crc32Result ?? 0;

        public ZlibBaseStream(Stream stream,
            CompressionMode compressionMode,
            CompressionLevel level,
            ZlibStreamFlavor flavor,
            bool leaveOpen)
        {
            FlushMode = FlushType.None;
            Stream = stream;
            LeaveOpen = leaveOpen;
            CompressionMode = compressionMode;
            Flavor = flavor;
            Level = level;
            if (flavor == ZlibStreamFlavor.Gzip) _crc = new Crc32();
        }

        protected internal bool WantCompress => CompressionMode == CompressionMode.Compress;

        private ZlibCodec z
        {
            get
            {
                if (Z != null) return Z;

                var wantRfc1950Header = Flavor == ZlibStreamFlavor.Zlib;
                Z = new ZlibCodec();
                if (CompressionMode == CompressionMode.Decompress)
                {
                    Z.InitializeInflate(wantRfc1950Header);
                }
                else
                {
                    Z.Strategy = Strategy;
                    Z.InitializeDeflate(Level, wantRfc1950Header);
                }

                return Z;
            }
        }

        private byte[] WorkingBuffer => _workingBuffer ??= new byte[BufferSize];

        public override void Write(byte[] buffer, int offset, int count)
        {
            _crc?.SlurpBlock(buffer, offset, count);

            if (_streamMode == StreamMode.Undefined)
                _streamMode = StreamMode.Writer;
            else if (_streamMode != StreamMode.Writer)
                throw new ZlibException("Cannot Write after Reading.");

            if (count == 0)
                return;

            z.InputBuffer = buffer;
            Z.NextIn = offset;
            Z.AvailableBytesIn = count;
            bool done;
            do
            {
                Z.OutputBuffer = WorkingBuffer;
                Z.NextOut = 0;
                Z.AvailableBytesOut = _workingBuffer.Length;
                var rc = WantCompress
                    ? Z.Deflate(FlushMode)
                    : Z.Inflate(FlushMode);
                if (rc != ZlibConstants.ZOk && rc != ZlibConstants.ZStreamEnd)
                    throw new ZlibException((WantCompress ? "de" : "in") + "flating: " + Z.Message);

                Stream.Write(_workingBuffer, 0, _workingBuffer.Length - Z.AvailableBytesOut);

                done = Z.AvailableBytesIn == 0 && Z.AvailableBytesOut != 0;

                if (Flavor == ZlibStreamFlavor.Gzip && !WantCompress)
                    done = Z.AvailableBytesIn == 8 && Z.AvailableBytesOut != 0;
            } while (!done);
        }

        private void Finish()
        {
            if (Z == null)
                return;

            switch (_streamMode)
            {
                case StreamMode.Writer:
                {
                    bool done;
                    do
                    {
                        Z.OutputBuffer = WorkingBuffer;
                        Z.NextOut = 0;
                        Z.AvailableBytesOut = _workingBuffer.Length;
                        var rc = WantCompress
                            ? Z.Deflate(FlushType.Finish)
                            : Z.Inflate(FlushType.Finish);

                        if (rc != ZlibConstants.ZStreamEnd && rc != ZlibConstants.ZOk)
                        {
                            var verb = (WantCompress ? "de" : "in") + "flating";
                            if (Z.Message == null)
                                throw new ZlibException($"{verb}: (rc = {rc})");

                            throw new ZlibException(verb + ": " + Z.Message);
                        }

                        if (_workingBuffer.Length - Z.AvailableBytesOut > 0)
                            Stream.Write(_workingBuffer, 0, _workingBuffer.Length - Z.AvailableBytesOut);

                        done = Z.AvailableBytesIn == 0 && Z.AvailableBytesOut != 0;
                        if (Flavor == ZlibStreamFlavor.Gzip && !WantCompress)
                            done = Z.AvailableBytesIn == 8 && Z.AvailableBytesOut != 0;
                    } while (!done);

                    Flush();

                    if (Flavor == ZlibStreamFlavor.Gzip)
                    {
                        if (WantCompress)
                        {
                            var c1 = _crc.Crc32Result;
                            Stream.Write(BitConverter.GetBytes(c1), 0, 4);
                            var c2 = (int) (_crc.TotalBytesRead & 0x00000000FFFFFFFF);
                            Stream.Write(BitConverter.GetBytes(c2), 0, 4);
                        }
                        else
                        {
                            throw new ZlibException("Writing with decompression is not supported.");
                        }
                    }

                    break;
                }

                case StreamMode.Reader:
                {
                    if (Flavor == ZlibStreamFlavor.Gzip)
                    {
                        if (!WantCompress)
                        {
                            if (Z.TotalBytesOut == 0L)
                                return;

                            var trailer = new byte[8];

                            if (Z.AvailableBytesIn < 8)
                            {
                                Array.Copy(Z.InputBuffer, Z.NextIn, trailer, 0, Z.AvailableBytesIn);
                                var bytesNeeded = 8 - Z.AvailableBytesIn;
                                var bytesRead = Stream.Read(trailer,
                                    Z.AvailableBytesIn,
                                    bytesNeeded);
                                if (bytesNeeded != bytesRead)
                                    throw new ZlibException(
                                        $"Missing or incomplete GZIP trailer. Expected 8 bytes, got {Z.AvailableBytesIn + bytesRead}.");
                            }
                            else
                            {
                                Array.Copy(Z.InputBuffer, Z.NextIn, trailer, 0, trailer.Length);
                            }

                            var crc32Expected = BitConverter.ToInt32(trailer, 0);
                            var crc32Actual = _crc.Crc32Result;
                            var isizeExpected = BitConverter.ToInt32(trailer, 4);
                            var isizeActual = (int) (Z.TotalBytesOut & 0x00000000FFFFFFFF);

                            if (crc32Actual != crc32Expected)
                                throw new ZlibException(
                                    $"Bad CRC32 in GZIP trailer. (actual({crc32Actual:X8})!=expected({crc32Expected:X8}))");

                            if (isizeActual != isizeExpected)
                                throw new ZlibException(
                                    $"Bad size in GZIP trailer. (actual({isizeActual})!=expected({isizeExpected}))");
                        }
                        else
                        {
                            throw new ZlibException("Reading with compression is not supported.");
                        }
                    }

                    break;
                }
            }
        }

        private void End()
        {
            if (z == null)
                return;

            if (WantCompress) Z.EndDeflate();
            else Z.EndInflate();
            Z = null;
        }

        public override void Close()
        {
            if (Stream == null)
                return;

            try
            {
                Finish();
            }
            finally
            {
                End();
                if (!LeaveOpen)
                    Stream.Close();
                Stream = null;
            }
        }

        public override void Flush()
        {
            Stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            Stream.SetLength(value);
        }

        private bool _nomoreinput;

        private string ReadZeroTerminatedString()
        {
            var list = new List<byte>();
            var done = false;
            do
            {
                var n = Stream.Read(Buf1, 0, 1);
                if (n != 1)
                    throw new ZlibException("Unexpected EOF reading GZIP header.");

                if (Buf1[0] == 0)
                    done = true;
                else
                    list.Add(Buf1[0]);
            } while (!done);

            var a = list.ToArray();
            return GZipStream.Iso8859dash1.GetString(a, 0, a.Length);
        }

        private int ReadAndValidateGzipHeader()
        {
            var totalBytesRead = 0;
            var header = new byte[10];
            var n = Stream.Read(header, 0, header.Length);

            if (n == 0)
                return 0;

            if (n != 10)
                throw new ZlibException("Not a valid GZIP stream.");

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
                throw new ZlibException("Bad GZIP header.");

            var timet = BitConverter.ToInt32(header, 4);
            GzipMtime = GZipStream.UnixEpoch.AddSeconds(timet);
            totalBytesRead += n;
            if ((header[3] & 0x04) == 0x04)
            {
                n = Stream.Read(header, 0, 2); 
                totalBytesRead += n;

                var extraLength = (short) (header[0] + header[1] * 256);
                var extra = new byte[extraLength];
                n = Stream.Read(extra, 0, extra.Length);
                if (n != extraLength)
                    throw new ZlibException("Unexpected end-of-file reading GZIP header.");

                totalBytesRead += n;
            }

            if ((header[3] & 0x08) == 0x08)
                GzipFileName = ReadZeroTerminatedString();
            if ((header[3] & 0x10) == 0x010)
                GzipComment = ReadZeroTerminatedString();
            if ((header[3] & 0x02) == 0x02)
                Read(Buf1, 0, 1); 

            return totalBytesRead;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_streamMode == StreamMode.Undefined)
            {
                if (!Stream.CanRead)
                    throw new ZlibException("The stream is not readable.");

                _streamMode = StreamMode.Reader;
                z.AvailableBytesIn = 0;
                if (Flavor == ZlibStreamFlavor.Gzip)
                {
                    GzipHeaderByteCount = ReadAndValidateGzipHeader();
                    if (GzipHeaderByteCount == 0)
                        return 0;
                }
            }

            if (_streamMode != StreamMode.Reader)
                throw new ZlibException("Cannot Read after Writing.");

            if (count == 0)
                return 0;
            if (_nomoreinput && WantCompress)
                return 0; 

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (offset < buffer.GetLowerBound(0))
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (offset + count > buffer.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(count));

            int rc;

            Z.OutputBuffer = buffer;
            Z.NextOut = offset;
            Z.AvailableBytesOut = count;

            Z.InputBuffer = WorkingBuffer;

            do
            {
                if (Z.AvailableBytesIn == 0 && !_nomoreinput)
                {
                    Z.NextIn = 0;
                    Z.AvailableBytesIn = Stream.Read(_workingBuffer, 0, _workingBuffer.Length);
                    if (Z.AvailableBytesIn == 0)
                        _nomoreinput = true;
                }

                rc = WantCompress
                    ? Z.Deflate(FlushMode)
                    : Z.Inflate(FlushMode);

                if (_nomoreinput && rc == ZlibConstants.ZBufError)
                    return 0;

                if (rc != ZlibConstants.ZOk && rc != ZlibConstants.ZStreamEnd)
                    throw new ZlibException($"{(WantCompress ? "de" : "in")}flating:  rc={rc}  msg={Z.Message}");

                if ((_nomoreinput || rc == ZlibConstants.ZStreamEnd) && Z.AvailableBytesOut == count)
                    break; 
            }

            while (Z.AvailableBytesOut > 0 && !_nomoreinput && rc == ZlibConstants.ZOk);

            if (Z.AvailableBytesOut > 0)
            {
                if (_nomoreinput)
                    if (WantCompress)
                    {
                        rc = Z.Deflate(FlushType.Finish);

                        if (rc != ZlibConstants.ZOk && rc != ZlibConstants.ZStreamEnd)
                            throw new ZlibException($"Deflating:  rc={rc}  msg={Z.Message}");
                    }
            }

            rc = count - Z.AvailableBytesOut;

            _crc?.SlurpBlock(buffer, offset, rc);

            return rc;
        }

        public override bool CanRead => Stream.CanRead;

        public override bool CanSeek => Stream.CanSeek;

        public override bool CanWrite => Stream.CanWrite;

        public override long Length => Stream.Length;

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        internal enum StreamMode
        {
            Writer,
            Reader,
            Undefined
        }

        public static void CompressString(string s, Stream compressor)
        {
            var uncompressed = Encoding.UTF8.GetBytes(s);
            using (compressor)
            {
                compressor.Write(uncompressed, 0, uncompressed.Length);
            }
        }

        public static void CompressBuffer(byte[] b, Stream compressor)
        {
            using (compressor)
            {
                compressor.Write(b, 0, b.Length);
            }
        }

        public static string UncompressString(byte[] compressed, Stream decompressor)
        {
            var working = new byte[1024];
            var encoding = Encoding.UTF8;
            using var output = new MemoryStream();
            using (decompressor)
            {
                int n;
                while ((n = decompressor.Read(working, 0, working.Length)) != 0) output.Write(working, 0, n);
            }

            output.Seek(0, SeekOrigin.Begin);
            var sr = new StreamReader(output, encoding);
            return sr.ReadToEnd();
        }

        public static byte[] UncompressBuffer(byte[] compressed, Stream decompressor)
        {
            var working = new byte[1024];
            using var output = new MemoryStream();
            using (decompressor)
            {
                int n;
                while ((n = decompressor.Read(working, 0, working.Length)) != 0) output.Write(working, 0, n);
            }

            return output.ToArray();
        }
    }
}