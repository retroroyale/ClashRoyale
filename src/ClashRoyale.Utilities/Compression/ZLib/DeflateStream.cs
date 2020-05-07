using System;
using System.IO;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    public class DeflateStream : Stream
    {
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_disposed) return;

                if (disposing)
                    BaseStream?.Close();
                _disposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        internal ZlibBaseStream BaseStream;
        internal Stream InnerStream;

        public DeflateStream(Stream stream, CompressionMode mode)
            : this(stream, mode, CompressionLevel.Default, false)
        {
        }

        public DeflateStream(Stream stream, CompressionMode mode, CompressionLevel level)
            : this(stream, mode, level, false)
        {
        }

        public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
            : this(stream, mode, CompressionLevel.Default, leaveOpen)
        {
        }

        public DeflateStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            InnerStream = stream;
            BaseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.Deflate, leaveOpen);
        }

        public int BufferSize
        {
            get => BaseStream.BufferSize;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("DeflateStream");
                if (BaseStream._workingBuffer != null)
                    throw new ZlibException("The working buffer is already set.");
                if (value < ZlibConstants.WorkingBufferSizeMin)
                    throw new ZlibException(
                        $"Don't be silly. {value} bytes?? Use a bigger buffer, at least {ZlibConstants.WorkingBufferSizeMin}.");

                BaseStream.BufferSize = value;
            }
        }

        public override bool CanRead
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("DeflateStream");

                return BaseStream.Stream.CanRead;
            }
        }

        public override bool CanSeek => false;

        public override bool CanWrite
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException("DeflateStream");

                return BaseStream.Stream.CanWrite;
            }
        }

        public virtual FlushType FlushMode
        {
            get => BaseStream.FlushMode;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("DeflateStream");

                BaseStream.FlushMode = value;
            }
        }

        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get
            {
                switch (BaseStream._streamMode)
                {
                    case ZlibBaseStream.StreamMode.Writer:
                        return BaseStream.Z.TotalBytesOut;
                    case ZlibBaseStream.StreamMode.Reader:
                        return BaseStream.Z.TotalBytesIn;
                    default:
                        return 0;
                }
            }
            set => throw new NotImplementedException();
        }

        public CompressionStrategy Strategy
        {
            get => BaseStream.Strategy;
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException("DeflateStream");

                BaseStream.Strategy = value;
            }
        }

        public virtual long TotalIn => BaseStream.Z.TotalBytesIn;

        public virtual long TotalOut => BaseStream.Z.TotalBytesOut;

        public static byte[] CompressBuffer(byte[] b)
        {
            using var ms = new MemoryStream();
            Stream compressor =
                new DeflateStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);

            ZlibBaseStream.CompressBuffer(b, compressor);
            return ms.ToArray();
        }

        public static byte[] CompressString(string s)
        {
            using var ms = new MemoryStream();
            Stream compressor =
                new DeflateStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);
            ZlibBaseStream.CompressString(s, compressor);
            return ms.ToArray();
        }

        public static byte[] UncompressBuffer(byte[] compressed)
        {
            using var input = new MemoryStream(compressed);
            Stream decompressor =
                new DeflateStream(input, CompressionMode.Decompress);

            return ZlibBaseStream.UncompressBuffer(compressed, decompressor);
        }

        public static string UncompressString(byte[] compressed)
        {
            using var input = new MemoryStream(compressed);
            Stream decompressor =
                new DeflateStream(input, CompressionMode.Decompress);

            return ZlibBaseStream.UncompressString(compressed, decompressor);
        }

        public override void Flush()
        {
            if (_disposed)
                throw new ObjectDisposedException("DeflateStream");

            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed)
                throw new ObjectDisposedException("DeflateStream");

            return BaseStream.Read(buffer, offset, count);
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
                throw new ObjectDisposedException("DeflateStream");

            BaseStream.Write(buffer, offset, count);
        }
    }
}