using System;
using System.IO;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    public class ZlibStream : Stream
    {
        internal ZlibBaseStream _baseStream;
        private bool _disposed;

        public ZlibStream(Stream stream, CompressionMode mode)
            : this(stream, mode, CompressionLevel.Default, false)
        {
        }

        public ZlibStream(Stream stream, CompressionMode mode, CompressionLevel level)
            : this(stream, mode, level, false)
        {
        }

        public ZlibStream(Stream stream, CompressionMode mode, bool leaveOpen)
            : this(stream, mode, CompressionLevel.Default, leaveOpen)
        {
        }

        public ZlibStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            _baseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.Zlib, leaveOpen);
        }

        public int BufferSize
        {
            get => _baseStream.BufferSize;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("ZlibStream");
                if (_baseStream._workingBuffer != null)
                    throw new ZlibException("The working buffer is already set.");
                if (value < ZlibConstants.WorkingBufferSizeMin)
                    throw new ZlibException(
                        $"Don't be silly. {value} bytes?? Use a bigger buffer, at least {ZlibConstants.WorkingBufferSizeMin}.");

                _baseStream.BufferSize = value;
            }
        }

        public override bool CanRead
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("ZlibStream");

                return _baseStream.Stream.CanRead;
            }
        }

        public override bool CanSeek => false;

        public override bool CanWrite
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("ZlibStream");

                return _baseStream.Stream.CanWrite;
            }
        }

        public virtual FlushType FlushMode
        {
            get => _baseStream.FlushMode;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("ZlibStream");

                _baseStream.FlushMode = value;
            }
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get
            {
                switch (_baseStream._streamMode)
                {
                    case ZlibBaseStream.StreamMode.Writer:
                        return _baseStream.Z.TotalBytesOut;
                    case ZlibBaseStream.StreamMode.Reader:
                        return _baseStream.Z.TotalBytesIn;
                    default:
                        return 0;
                }
            }

            set => throw new NotSupportedException();
        }

        public virtual long TotalIn => _baseStream.Z.TotalBytesIn;

        public virtual long TotalOut => _baseStream.Z.TotalBytesOut;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_disposed) return;

                if (disposing)
                    _baseStream?.Close();
                _disposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public static byte[] CompressBuffer(byte[] b)
        {
            using (var ms = new MemoryStream())
            {
                Stream compressor =
                    new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);

                ZlibBaseStream.CompressBuffer(b, compressor);
                return ms.ToArray();
            }
        }

        public static byte[] CompressString(string s)
        {
            using (var ms = new MemoryStream())
            {
                Stream compressor =
                    new ZlibStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);
                ZlibBaseStream.CompressString(s, compressor);
                return ms.ToArray();
            }
        }

        public static byte[] UncompressBuffer(byte[] compressed)
        {
            using (var input = new MemoryStream(compressed))
            {
                Stream decompressor =
                    new ZlibStream(input, CompressionMode.Decompress);

                return ZlibBaseStream.UncompressBuffer(compressed, decompressor);
            }
        }

        public static string UncompressString(byte[] compressed)
        {
            using (var input = new MemoryStream(compressed))
            {
                Stream decompressor =
                    new ZlibStream(input, CompressionMode.Decompress);

                return ZlibBaseStream.UncompressString(compressed, decompressor);
            }
        }

        public override void Flush()
        {
            if (_disposed)
                throw new ObjectDisposedException("ZlibStream");

            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("ZlibStream");

            return _baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("ZlibStream");

            _baseStream.Write(buffer, offset, count);
        }
    }
}