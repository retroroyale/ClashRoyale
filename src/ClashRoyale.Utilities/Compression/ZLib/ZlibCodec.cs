using System;
using Interop = System.Runtime.InteropServices;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    [Interop.Guid("ebc25cf6-9120-4283-b972-0e5520d0000D")]
    [Interop.ComVisible(true)]
#if !NETCF
    [Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]
#endif
    public sealed class ZlibCodec
    {
        internal uint _Adler32;

        public int AvailableBytesIn;
        public int AvailableBytesOut;

        public CompressionLevel CompressLevel = CompressionLevel.Default;

        internal DeflateManager Dstate;

        public byte[] InputBuffer;

        internal InflateManager Istate;

        public string Message;

        public int NextIn;
        public int NextOut;

        public byte[] OutputBuffer;

        public CompressionStrategy Strategy = CompressionStrategy.Default;

        public long TotalBytesIn;
        public long TotalBytesOut;

        public int WindowBits = ZlibConstants.WindowBitsDefault;

        public ZlibCodec()
        {
        }

        public ZlibCodec(CompressionMode mode)
        {
            switch (mode)
            {
                case CompressionMode.Compress:
                {
                    var rc = InitializeDeflate();
                    if (rc != ZlibConstants.ZOk)
                        throw new ZlibException("Cannot initialize for deflate.");

                    break;
                }

                case CompressionMode.Decompress:
                {
                    var rc = InitializeInflate();
                    if (rc != ZlibConstants.ZOk)
                        throw new ZlibException("Cannot initialize for inflate.");

                    break;
                }

                default:
                    throw new ZlibException("Invalid ZlibStreamFlavor.");
            }
        }

        public int Adler32 => (int) _Adler32;

        public int InitializeInflate()
        {
            return InitializeInflate(WindowBits);
        }

        public int InitializeInflate(bool expectRfc1950Header)
        {
            return InitializeInflate(WindowBits, expectRfc1950Header);
        }

        public int InitializeInflate(int windowBits)
        {
            WindowBits = windowBits;
            return InitializeInflate(windowBits, true);
        }

        public int InitializeInflate(int windowBits, bool expectRfc1950Header)
        {
            WindowBits = windowBits;
            if (Dstate != null)
                throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");

            Istate = new InflateManager(expectRfc1950Header);
            return Istate.Initialize(this, windowBits);
        }

        public int Inflate(FlushType flush)
        {
            if (Istate == null)
                throw new ZlibException("No Inflate State!");

            return Istate.Inflate(flush);
        }

        public int EndInflate()
        {
            if (Istate == null)
                throw new ZlibException("No Inflate State!");

            var ret = Istate.End();
            Istate = null;
            return ret;
        }

        public int SyncInflate()
        {
            if (Istate == null)
                throw new ZlibException("No Inflate State!");

            return Istate.Sync();
        }

        public int InitializeDeflate()
        {
            return _InternalInitializeDeflate(true);
        }

        public int InitializeDeflate(CompressionLevel level)
        {
            CompressLevel = level;
            return _InternalInitializeDeflate(true);
        }

        public int InitializeDeflate(CompressionLevel level, bool wantRfc1950Header)
        {
            CompressLevel = level;
            return _InternalInitializeDeflate(wantRfc1950Header);
        }

        public int InitializeDeflate(CompressionLevel level, int bits)
        {
            CompressLevel = level;
            WindowBits = bits;
            return _InternalInitializeDeflate(true);
        }

        public int InitializeDeflate(CompressionLevel level, int bits, bool wantRfc1950Header)
        {
            CompressLevel = level;
            WindowBits = bits;
            return _InternalInitializeDeflate(wantRfc1950Header);
        }

        private int _InternalInitializeDeflate(bool wantRfc1950Header)
        {
            if (Istate != null)
                throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");

            Dstate = new DeflateManager {WantRfc1950HeaderBytes = wantRfc1950Header};

            return Dstate.Initialize(this, CompressLevel, WindowBits, Strategy);
        }

        public int Deflate(FlushType flush)
        {
            if (Dstate == null)
                throw new ZlibException("No Deflate State!");

            return Dstate.Deflate(flush);
        }

        public int EndDeflate()
        {
            if (Dstate == null)
                throw new ZlibException("No Deflate State!");

            Dstate = null;
            return ZlibConstants.ZOk; ;
        }

        public void ResetDeflate()
        {
            if (Dstate == null)
                throw new ZlibException("No Deflate State!");

            Dstate.Reset();
        }

        public int SetDeflateParams(CompressionLevel level, CompressionStrategy strategy)
        {
            if (Dstate == null)
                throw new ZlibException("No Deflate State!");

            return Dstate.SetParams(level, strategy);
        }

        public int SetDictionary(byte[] dictionary)
        {
            if (Istate != null)
                return Istate.SetDictionary(dictionary);

            if (Dstate != null)
                return Dstate.SetDictionary(dictionary);

            throw new ZlibException("No Inflate or Deflate state!");
        }

        internal void Flush_pending()
        {
            var len = Dstate.PendingCount;

            if (len > AvailableBytesOut)
                len = AvailableBytesOut;
            if (len == 0)
                return;

            if (Dstate.Pending.Length <= Dstate.NextPending ||
                OutputBuffer.Length <= NextOut ||
                Dstate.Pending.Length < Dstate.NextPending + len ||
                OutputBuffer.Length < NextOut + len)
                throw new ZlibException(
                    $"Invalid State. (pending.Length={Dstate.Pending.Length}, pendingCount={Dstate.PendingCount})");

            Array.Copy(Dstate.Pending, Dstate.NextPending, OutputBuffer, NextOut, len);

            NextOut += len;
            Dstate.NextPending += len;
            TotalBytesOut += len;
            AvailableBytesOut -= len;
            Dstate.PendingCount -= len;
            if (Dstate.PendingCount == 0) Dstate.NextPending = 0;
        }

        internal int Read_buf(byte[] buf, int start, int size)
        {
            var len = AvailableBytesIn;

            if (len > size)
                len = size;
            if (len == 0)
                return 0;

            AvailableBytesIn -= len;

            if (Dstate.WantRfc1950HeaderBytes) _Adler32 = Adler.Adler32(_Adler32, InputBuffer, NextIn, len);
            Array.Copy(InputBuffer, NextIn, buf, start, len);
            NextIn += len;
            TotalBytesIn += len;
            return len;
        }
    }
}