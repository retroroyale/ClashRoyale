using System;
using System.IO;
using Interop = System.Runtime.InteropServices;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    [Interop.Guid("ebc25cf6-9120-4283-b972-0e5520d0000C")]
    [Interop.ComVisible(true)]
#if !NETCF
    [Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]
#endif
    public class Crc32
    {
        private const int BufferSize = 8192;

        private readonly uint _dwPolynomial;
        private readonly bool _reverseBits;

        private uint _register = 0xFFFFFFFFU;
        private uint[] _crc32Table;

        public Crc32() : this(false)
        {
        }

        public Crc32(bool reverseBits) :
            this(unchecked((int) 0xEDB88320), reverseBits)
        {
        }

        public Crc32(int polynomial, bool reverseBits)
        {
            _reverseBits = reverseBits;
            _dwPolynomial = (uint) polynomial;
            GenerateLookupTable();
        }

        public long TotalBytesRead { get; private set; }

        public int Crc32Result => unchecked((int) ~_register);

        public int GetCrc32(Stream input)
        {
            return GetCrc32AndCopy(input, null);
        }

        public int GetCrc32AndCopy(Stream input, Stream output)
        {
            if (input == null)
                throw new Exception("The input stream must not be null.");

            unchecked
            {
                var buffer = new byte[BufferSize];

                TotalBytesRead = 0;
                var count = input.Read(buffer, 0, BufferSize);
                output?.Write(buffer, 0, count);
                TotalBytesRead += count;
                while (count > 0)
                {
                    SlurpBlock(buffer, 0, count);
                    count = input.Read(buffer, 0, BufferSize);
                    output?.Write(buffer, 0, count);
                    TotalBytesRead += count;
                }

                return (int) ~_register;
            }
        }

        public int ComputeCrc32(int w, byte b)
        {
            return InternalComputeCrc32((uint) w, b);
        }

        public int InternalComputeCrc32(uint w, byte b)
        {
            return (int) (_crc32Table[(w ^ b) & 0xFF] ^ (w >> 8));
        }

        public void SlurpBlock(byte[] block, int offset, int count)
        {
            if (block == null)
                throw new Exception("The data buffer must not be null.");

            for (var i = 0; i < count; i++)
            {
                var x = offset + i;
                var b = block[x];
                if (_reverseBits)
                {
                    var temp = (_register >> 24) ^ b;
                    _register = (_register << 8) ^ _crc32Table[temp];
                }
                else
                {
                    var temp = (_register & 0x000000FF) ^ b;
                    _register = (_register >> 8) ^ _crc32Table[temp];
                }
            }

            TotalBytesRead += count;
        }

        public void UpdateCrc(byte b)
        {
            if (_reverseBits)
            {
                var temp = (_register >> 24) ^ b;
                _register = (_register << 8) ^ _crc32Table[temp];
            }
            else
            {
                var temp = (_register & 0x000000FF) ^ b;
                _register = (_register >> 8) ^ _crc32Table[temp];
            }
        }

        public void UpdateCrc(byte b, int n)
        {
            while (n-- > 0)
                if (_reverseBits)
                {
                    var temp = (_register >> 24) ^ b;
                    _register = (_register << 8) ^ _crc32Table[temp];
                }
                else
                {
                    var temp = (_register & 0x000000FF) ^ b;
                    _register = (_register >> 8) ^ _crc32Table[temp];
                }
        }

        private static uint ReverseBits(uint data)
        {
            unchecked
            {
                var ret = data;
                ret = ((ret & 0x55555555) << 1) | ((ret >> 1) & 0x55555555);
                ret = ((ret & 0x33333333) << 2) | ((ret >> 2) & 0x33333333);
                ret = ((ret & 0x0F0F0F0F) << 4) | ((ret >> 4) & 0x0F0F0F0F);
                ret = (ret << 24) | ((ret & 0xFF00) << 8) | ((ret >> 8) & 0xFF00) | (ret >> 24);
                return ret;
            }
        }

        private static byte ReverseBits(byte data)
        {
            unchecked
            {
                var u = (uint) data * 0x00020202;
                uint m = 0x01044010;
                var s = u & m;
                var t = (u << 2) & (m << 1);
                return (byte) ((0x01001001 * (s + t)) >> 24);
            }
        }

        private void GenerateLookupTable()
        {
            _crc32Table = new uint[256];
            unchecked
            {
                byte i = 0;
                do
                {
                    uint dwCrc = i;
                    for (byte j = 8; j > 0; j--)
                        if ((dwCrc & 1) == 1) dwCrc = (dwCrc >> 1) ^ _dwPolynomial;
                        else dwCrc >>= 1;
                    if (_reverseBits) _crc32Table[ReverseBits(i)] = ReverseBits(dwCrc);
                    else _crc32Table[i] = dwCrc;
                    i++;
                } while (i != 0);
            }
        }

        private uint Gf2_matrix_times(uint[] matrix, uint vec)
        {
            uint sum = 0;
            var i = 0;
            while (vec != 0)
            {
                if ((vec & 0x01) == 0x01)
                    sum ^= matrix[i];
                vec >>= 1;
                i++;
            }

            return sum;
        }

        private void Gf2_matrix_square(uint[] square, uint[] mat)
        {
            for (var i = 0; i < 32; i++)
                square[i] = Gf2_matrix_times(mat, mat[i]);
        }

        public void Combine(int crc, int length)
        {
            var even = new uint[32]; 
            var odd = new uint[32]; 

            if (length == 0)
                return;

            var crc1 = ~_register;
            var crc2 = (uint) crc;

            odd[0] = _dwPolynomial; 
            uint row = 1;
            for (var i = 1; i < 32; i++)
            {
                odd[i] = row;
                row <<= 1;
            }

            Gf2_matrix_square(even, odd);
            Gf2_matrix_square(odd, even);

            var len2 = (uint) length;

            do
            {
                Gf2_matrix_square(even, odd);

                if ((len2 & 1) == 1)
                    crc1 = Gf2_matrix_times(even, crc1);
                len2 >>= 1;

                if (len2 == 0)
                    break;

                Gf2_matrix_square(odd, even);
                if ((len2 & 1) == 1)
                    crc1 = Gf2_matrix_times(odd, crc1);
                len2 >>= 1;
            } while (len2 != 0);

            crc1 ^= crc2;

            _register = ~crc1;
        }

        public void Reset()
        {
            _register = 0xFFFFFFFFU;
        }
    }

    public class CrcCalculatorStream : Stream, IDisposable
    {
        public static readonly long UnsetLengthLimit = -99;
        private readonly Crc32 _crc32;
        private readonly long _lengthLimit;
        public Stream InnerStream;

        public CrcCalculatorStream(Stream stream)
            : this(true, UnsetLengthLimit, stream, null)
        {
        }

        public CrcCalculatorStream(Stream stream, bool leaveOpen)
            : this(leaveOpen, UnsetLengthLimit, stream, null)
        {
        }

        public CrcCalculatorStream(Stream stream, long length)
            : this(true, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException("length");
        }

        public CrcCalculatorStream(Stream stream, long length, bool leaveOpen)
            : this(leaveOpen, length, stream, null)
        {
            if (length < 0)
                throw new ArgumentException("length");
        }

        public CrcCalculatorStream(Stream stream, long length, bool leaveOpen,
            Crc32 crc32)
            : this(leaveOpen, length, stream, crc32)
        {
            if (length < 0)
                throw new ArgumentException("length");
        }

        private CrcCalculatorStream
            (bool leaveOpen, long length, Stream stream, Crc32 crc32)
        {
            InnerStream = stream;
            _crc32 = crc32 ?? new Crc32();
            _lengthLimit = length;
            LeaveOpen = leaveOpen;
        }

        public long TotalBytesSlurped => _crc32.TotalBytesRead;
        public int Crc => _crc32.Crc32Result;
        public bool LeaveOpen { get; set; }
        public override bool CanRead => InnerStream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => InnerStream.CanWrite;
        public override long Length => _lengthLimit == UnsetLengthLimit ? InnerStream.Length : _lengthLimit;

        public override long Position
        {
            get => _crc32.TotalBytesRead;
            set => throw new NotSupportedException();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var bytesToRead = count;

            if (_lengthLimit != UnsetLengthLimit)
            {
                if (_crc32.TotalBytesRead >= _lengthLimit)
                    return 0;

                var bytesRemaining = _lengthLimit - _crc32.TotalBytesRead;
                if (bytesRemaining < count)
                    bytesToRead = (int) bytesRemaining;
            }

            var n = InnerStream.Read(buffer, offset, bytesToRead);
            if (n > 0)
                _crc32.SlurpBlock(buffer, offset, n);
            return n;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > 0)
                _crc32.SlurpBlock(buffer, offset, count);
            InnerStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            InnerStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Close()
        {
            base.Close();
            if (!LeaveOpen)
                InnerStream.Close();
        }
    }
}