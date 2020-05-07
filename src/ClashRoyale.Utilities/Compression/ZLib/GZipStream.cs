using System;
using System.IO;
using System.Text;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    public class GZipStream : Stream
    {
        public string Comment
        {
            get => _comment;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("GZipStream");

                _comment = value;
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("GZipStream");

                _fileName = value;
                if (_fileName == null)
                    return;

                if (_fileName.IndexOf("/", StringComparison.Ordinal) != -1) _fileName = _fileName.Replace("/", "\\");
                if (_fileName.EndsWith("\\"))
                    throw new Exception("Illegal filename");

                if (_fileName.IndexOf("\\", StringComparison.Ordinal) != -1)
                    _fileName = Path.GetFileName(_fileName);
            }
        }

        public DateTime? LastModified;

        public int Crc32 { get; private set; }

        private int _headerByteCount;
        internal ZlibBaseStream BaseStream;
        private bool _disposed;
        private bool _firstReadDone;
        private string _fileName;
        private string _comment;

        public GZipStream(Stream stream, CompressionMode mode)
            : this(stream, mode, CompressionLevel.Default, false)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level)
            : this(stream, mode, level, false)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
            : this(stream, mode, CompressionLevel.Default, leaveOpen)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            BaseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.Gzip, leaveOpen);
        }

        public virtual FlushType FlushMode
        {
            get => BaseStream.FlushMode;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("GZipStream");

                BaseStream.FlushMode = value;
            }
        }

        public int BufferSize
        {
            get => BaseStream.BufferSize;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("GZipStream");
                if (BaseStream._workingBuffer != null)
                    throw new ZlibException("The working buffer is already set.");
                if (value < ZlibConstants.WorkingBufferSizeMin)
                    throw new ZlibException(
                        $"Don't be silly. {value} bytes?? Use a bigger buffer, at least {ZlibConstants.WorkingBufferSizeMin}.");

                BaseStream.BufferSize = value;
            }
        }

        public virtual long TotalIn => BaseStream.Z.TotalBytesIn;

        public virtual long TotalOut => BaseStream.Z.TotalBytesOut;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_disposed) return;

                if (disposing && BaseStream != null)
                {
                    BaseStream.Close();
                    Crc32 = BaseStream.Crc32;
                }

                _disposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override bool CanRead
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("GZipStream");

                return BaseStream.Stream.CanRead;
            }
        }

        public override bool CanSeek => false;

        public override bool CanWrite
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("GZipStream");

                return BaseStream.Stream.CanWrite;
            }
        }

        public override void Flush()
        {
            if (_disposed)
                throw new ObjectDisposedException("GZipStream");

            BaseStream.Flush();
        }

        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get
            {
                switch (BaseStream._streamMode)
                {
                    case ZlibBaseStream.StreamMode.Writer:
                        return BaseStream.Z.TotalBytesOut + _headerByteCount;
                    case ZlibBaseStream.StreamMode.Reader:
                        return BaseStream.Z.TotalBytesIn + BaseStream.GzipHeaderByteCount;
                    default:
                        return 0;
                }
            }

            set => throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("GZipStream");

            var n = BaseStream.Read(buffer, offset, count);

            if (_firstReadDone) return n;

            _firstReadDone = true;
            FileName = BaseStream.GzipFileName;
            Comment = BaseStream.GzipComment;

            return n;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("GZipStream");

            if (BaseStream._streamMode == ZlibBaseStream.StreamMode.Undefined)
            {
                if (BaseStream.WantCompress)
                    _headerByteCount = EmitHeader();
                else throw new InvalidOperationException();
            }

            BaseStream.Write(buffer, offset, count);
        }

        internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        internal static readonly Encoding Iso8859dash1 = Encoding.GetEncoding("iso-8859-1");

        private int EmitHeader()
        {
            var commentBytes = Comment == null ? null : Iso8859dash1.GetBytes(Comment);
            var filenameBytes = FileName == null ? null : Iso8859dash1.GetBytes(FileName);

            if (commentBytes != null)
            {
                var cbLength = Comment == null ? 0 : commentBytes.Length + 1;
                if (filenameBytes != null)
                {
                    var fnLength = FileName == null ? 0 : filenameBytes.Length + 1;

                    var bufferLength = 10 + cbLength + fnLength;
                    var header = new byte[bufferLength];
                    var i = 0;

                    header[i++] = 0x1F;
                    header[i++] = 0x8B;

                    header[i++] = 8;
                    byte flag = 0;
                    if (Comment != null)
                        flag ^= 0x10;
                    if (FileName != null)
                        flag ^= 0x8;

                    header[i++] = flag;

                    if (!LastModified.HasValue)
                        LastModified = DateTime.Now;
                    var delta = LastModified.Value - UnixEpoch;
                    var timet = (int) delta.TotalSeconds;
                    Array.Copy(BitConverter.GetBytes(timet), 0, header, i, 4);
                    i += 4;

                    header[i++] = 0; 
                    header[i++] = 0xFF; 

                    if (fnLength != 0)
                    {
                        Array.Copy(filenameBytes, 0, header, i, fnLength - 1);
                        i += fnLength - 1;
                        header[i++] = 0; 
                    }

                    if (cbLength != 0)
                    {
                        Array.Copy(commentBytes, 0, header, i, cbLength - 1);
                        i += cbLength - 1;
                        header[i++] = 0; 
                    }

                    BaseStream.Stream.Write(header, 0, header.Length);

                    return header.Length; 
                }
            }

            return 0;
        }

        public static byte[] CompressString(string s)
        {
            using var ms = new MemoryStream();
            Stream compressor =
                new GZipStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);
            ZlibBaseStream.CompressString(s, compressor);
            return ms.ToArray();
        }

        public static byte[] CompressBuffer(byte[] b)
        {
            using var ms = new MemoryStream();
            Stream compressor =
                new GZipStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);

            ZlibBaseStream.CompressBuffer(b, compressor);
            return ms.ToArray();
        }

        public static string UncompressString(byte[] compressed)
        {
            using var input = new MemoryStream(compressed);
            Stream decompressor = new GZipStream(input, CompressionMode.Decompress);
            return ZlibBaseStream.UncompressString(compressed, decompressor);
        }

        public static byte[] UncompressBuffer(byte[] compressed)
        {
            using var input = new MemoryStream(compressed);
            Stream decompressor =
                new GZipStream(input, CompressionMode.Decompress);

            return ZlibBaseStream.UncompressBuffer(compressed, decompressor);
        }
    }
}