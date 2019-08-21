using System;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    internal static class InternalInflateConstants
    {
        internal static readonly int[] InflateMask =
        {
            0x00000000, 0x00000001, 0x00000003, 0x00000007,
            0x0000000f, 0x0000001f, 0x0000003f, 0x0000007f,
            0x000000ff, 0x000001ff, 0x000003ff, 0x000007ff,
            0x00000fff, 0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff
        };
    }

    internal sealed class InflateBlocks
    {
        private const int MANY = 1440;

        internal static readonly int[] border = {16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15};

        internal ZlibCodec _codec;
        internal int[] bb = new int[1];
        internal int bitb;
        internal int bitk;
        internal int[] blens;
        internal uint check;
        internal object checkfn;
        internal InflateCodes codes = new InflateCodes();
        internal int end;
        internal int[] hufts;
        internal int index;
        internal InfTree inftree = new InfTree();
        internal int last;
        internal int left;
        private InflateBlockMode mode;
        internal int readAt;
        internal int table;
        internal int[] tb = new int[1];
        internal byte[] window;
        internal int writeAt;

        internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
        {
            _codec = codec;
            hufts = new int[MANY * 3];
            window = new byte[w];
            end = w;
            this.checkfn = checkfn;
            mode = InflateBlockMode.TYPE;
            Reset();
        }

        internal int Flush(int r)
        {
            for (var pass = 0; pass < 2; pass++)
            {
                int nBytes;
                if (pass == 0)
                    nBytes = (readAt <= writeAt ? writeAt : end) - readAt;
                else
                    nBytes = writeAt - readAt;

                if (nBytes == 0)
                {
                    if (r == ZlibConstants.ZBufError)
                        r = ZlibConstants.ZOk;
                    return r;
                }

                if (nBytes > _codec.AvailableBytesOut)
                    nBytes = _codec.AvailableBytesOut;

                if (nBytes != 0 && r == ZlibConstants.ZBufError)
                    r = ZlibConstants.ZOk;

                _codec.AvailableBytesOut -= nBytes;
                _codec.TotalBytesOut += nBytes;

                if (checkfn != null)
                    _codec._Adler32 = check = Adler.Adler32(check, window, readAt, nBytes);

                Array.Copy(window, readAt, _codec.OutputBuffer, _codec.NextOut, nBytes);
                _codec.NextOut += nBytes;
                readAt += nBytes;

                if (readAt == end && pass == 0)
                {
                    readAt = 0;
                    if (writeAt == end)
                        writeAt = 0;
                }
                else
                {
                    pass++;
                }
            }

            return r;
        }

        internal void Free()
        {
            Reset();
            window = null;
            hufts = null;
        }

        internal int Process(int r)
        {
            int t;

            var p = _codec.NextIn;
            var n = _codec.AvailableBytesIn;
            var b = bitb;
            var k = bitk;

            var q = writeAt;
            var m = q < readAt ? readAt - q - 1 : end - q;

            while (true)
                switch (mode)
                {
                    case InflateBlockMode.TYPE:

                        while (k < 3)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.ZOk;
                            }
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        t = b & 7;
                        last = t & 1;

                        switch ((uint) t >> 1)
                        {
                            case 0: 
                                b >>= 3;
                                k -= 3;
                                t = k & 7; 
                                b >>= t;
                                k -= t;
                                mode = InflateBlockMode.LENS; 
                                break;

                            case 1:
                                var bl = new int[1];
                                var bd = new int[1];
                                var tl = new int[1][];
                                var td = new int[1][];
                                InfTree.Inflate_trees_fixed(bl, bd, tl, td, _codec);
                                codes.Init(bl[0], bd[0], tl[0], 0, td[0], 0);
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.CODES;
                                break;

                            case 2:
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.TABLE;
                                break;

                            case 3:
                                b >>= 3;
                                k -= 3;
                                mode = InflateBlockMode.BAD;
                                _codec.Message = "invalid block type";
                                r = ZlibConstants.ZDataError;
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                        }

                        break;

                    case InflateBlockMode.LENS:

                        while (k < 32)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.ZOk;
                            }
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            ;
                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        if (((~b >> 16) & 0xffff) != (b & 0xffff))
                        {
                            mode = InflateBlockMode.BAD;
                            _codec.Message = "invalid stored block lengths";
                            r = ZlibConstants.ZDataError;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        left = b & 0xffff;
                        b = k = 0;
                        mode = left != 0
                            ? InflateBlockMode.STORED
                            : last != 0
                                ? InflateBlockMode.DRY
                                : InflateBlockMode.TYPE;
                        break;

                    case InflateBlockMode.STORED:
                        if (n == 0)
                        {
                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        if (m == 0)
                        {
                            if (q == end && readAt != 0)
                            {
                                q = 0;
                                m = q < readAt ? readAt - q - 1 : end - q;
                            }

                            if (m == 0)
                            {
                                writeAt = q;
                                r = Flush(r);
                                q = writeAt;
                                m = q < readAt ? readAt - q - 1 : end - q;
                                if (q == end && readAt != 0)
                                {
                                    q = 0;
                                    m = q < readAt ? readAt - q - 1 : end - q;
                                }

                                if (m == 0)
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }
                            }
                        }

                        r = ZlibConstants.ZOk;

                        t = left;
                        if (t > n)
                            t = n;
                        if (t > m)
                            t = m;
                        Array.Copy(_codec.InputBuffer, p, window, q, t);
                        p += t;
                        n -= t;
                        q += t;
                        m -= t;
                        if ((left -= t) != 0)
                            break;

                        mode = last != 0 ? InflateBlockMode.DRY : InflateBlockMode.TYPE;
                        break;

                    case InflateBlockMode.TABLE:

                        while (k < 14)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.ZOk;
                            }
                            else
                            {
                                bitb = b;
                                bitk = k;
                                _codec.AvailableBytesIn = n;
                                _codec.TotalBytesIn += p - _codec.NextIn;
                                _codec.NextIn = p;
                                writeAt = q;
                                return Flush(r);
                            }

                            n--;
                            b |= (_codec.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        table = t = b & 0x3fff;
                        if ((t & 0x1f) > 29 || ((t >> 5) & 0x1f) > 29)
                        {
                            mode = InflateBlockMode.BAD;
                            _codec.Message = "too many length or distance symbols";
                            r = ZlibConstants.ZDataError;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        t = 258 + (t & 0x1f) + ((t >> 5) & 0x1f);
                        if (blens == null || blens.Length < t) blens = new int[t];
                        else
                            Array.Clear(blens, 0, t);

                        b >>= 14;
                        k -= 14;

                        index = 0;
                        mode = InflateBlockMode.BTREE;
                        goto case InflateBlockMode.BTREE;

                    case InflateBlockMode.BTREE:
                        while (index < 4 + (table >> 10))
                        {
                            while (k < 3)
                            {
                                if (n != 0)
                                {
                                    r = ZlibConstants.ZOk;
                                }
                                else
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                n--;
                                b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            blens[border[index++]] = b & 7;

                            b >>= 3;
                            k -= 3;
                        }

                        while (index < 19) blens[border[index++]] = 0;

                        bb[0] = 7;
                        t = inftree.Inflate_trees_bits(blens, bb, tb, hufts, _codec);
                        if (t != ZlibConstants.ZOk)
                        {
                            r = t;
                            if (r == ZlibConstants.ZDataError)
                            {
                                blens = null;
                                mode = InflateBlockMode.BAD;
                            }

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        index = 0;
                        mode = InflateBlockMode.DTREE;
                        goto case InflateBlockMode.DTREE;

                    case InflateBlockMode.DTREE:
                        while (true)
                        {
                            t = table;
                            if (!(index < 258 + (t & 0x1f) + ((t >> 5) & 0x1f))) break;

                            t = bb[0];

                            while (k < t)
                            {
                                if (n != 0)
                                {
                                    r = ZlibConstants.ZOk;
                                }
                                else
                                {
                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                n--;
                                b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                k += 8;
                            }

                            t = hufts[(tb[0] + (b & InternalInflateConstants.InflateMask[t])) * 3 + 1];
                            var c = hufts[(tb[0] + (b & InternalInflateConstants.InflateMask[t])) * 3 + 2];

                            if (c < 16)
                            {
                                b >>= t;
                                k -= t;
                                blens[index++] = c;
                            }
                            else
                            {
                                var i = c == 18 ? 7 : c - 14;
                                var j = c == 18 ? 11 : 3;

                                while (k < t + i)
                                {
                                    if (n != 0)
                                    {
                                        r = ZlibConstants.ZOk;
                                    }
                                    else
                                    {
                                        bitb = b;
                                        bitk = k;
                                        _codec.AvailableBytesIn = n;
                                        _codec.TotalBytesIn += p - _codec.NextIn;
                                        _codec.NextIn = p;
                                        writeAt = q;
                                        return Flush(r);
                                    }

                                    n--;
                                    b |= (_codec.InputBuffer[p++] & 0xff) << k;
                                    k += 8;
                                }

                                b >>= t;
                                k -= t;

                                j += b & InternalInflateConstants.InflateMask[i];

                                b >>= i;
                                k -= i;

                                i = index;
                                t = table;
                                if (i + j > 258 + (t & 0x1f) + ((t >> 5) & 0x1f) || c == 16 && i < 1)
                                {
                                    blens = null;
                                    mode = InflateBlockMode.BAD;
                                    _codec.Message = "invalid bit length repeat";
                                    r = ZlibConstants.ZDataError;

                                    bitb = b;
                                    bitk = k;
                                    _codec.AvailableBytesIn = n;
                                    _codec.TotalBytesIn += p - _codec.NextIn;
                                    _codec.NextIn = p;
                                    writeAt = q;
                                    return Flush(r);
                                }

                                c = c == 16 ? blens[i - 1] : 0;
                                do
                                {
                                    blens[i++] = c;
                                } while (--j != 0);

                                index = i;
                            }
                        }

                        tb[0] = -1;
                    {
                        int[] bl = {9};
                        int[] bd = {6};
                        var tl = new int[1];
                        var td = new int[1];

                        t = table;
                        t = inftree.Inflate_trees_dynamic(257 + (t & 0x1f), 1 + ((t >> 5) & 0x1f), blens, bl, bd, tl,
                            td,
                            hufts, _codec);

                        if (t != ZlibConstants.ZOk)
                        {
                            if (t == ZlibConstants.ZDataError)
                            {
                                blens = null;
                                mode = InflateBlockMode.BAD;
                            }

                            r = t;

                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        codes.Init(bl[0], bd[0], hufts, tl[0], hufts, td[0]);
                    }
                        mode = InflateBlockMode.CODES;
                        goto case InflateBlockMode.CODES;

                    case InflateBlockMode.CODES:
                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;

                        r = codes.Process(this, r);
                        if (r != ZlibConstants.ZStreamEnd) return Flush(r);

                        r = ZlibConstants.ZOk;
                        p = _codec.NextIn;
                        n = _codec.AvailableBytesIn;
                        b = bitb;
                        k = bitk;
                        q = writeAt;
                        m = q < readAt ? readAt - q - 1 : end - q;

                        if (last == 0)
                        {
                            mode = InflateBlockMode.TYPE;
                            break;
                        }

                        mode = InflateBlockMode.DRY;
                        goto case InflateBlockMode.DRY;

                    case InflateBlockMode.DRY:
                        writeAt = q;
                        r = Flush(r);
                        q = writeAt;

                        if (readAt != writeAt)
                        {
                            bitb = b;
                            bitk = k;
                            _codec.AvailableBytesIn = n;
                            _codec.TotalBytesIn += p - _codec.NextIn;
                            _codec.NextIn = p;
                            writeAt = q;
                            return Flush(r);
                        }

                        mode = InflateBlockMode.DONE;
                        goto case InflateBlockMode.DONE;

                    case InflateBlockMode.DONE:
                        r = ZlibConstants.ZStreamEnd;
                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);

                    case InflateBlockMode.BAD:
                        r = ZlibConstants.ZDataError;

                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);

                    default:
                        r = ZlibConstants.ZStreamError;

                        bitb = b;
                        bitk = k;
                        _codec.AvailableBytesIn = n;
                        _codec.TotalBytesIn += p - _codec.NextIn;
                        _codec.NextIn = p;
                        writeAt = q;
                        return Flush(r);
                }
        }

        internal uint Reset()
        {
            var oldCheck = check;
            mode = InflateBlockMode.TYPE;
            bitk = 0;
            bitb = 0;
            readAt = writeAt = 0;

            if (checkfn != null)
                _codec._Adler32 = check = Adler.Adler32(0, null, 0, 0);
            return oldCheck;
        }

        internal void SetDictionary(byte[] d, int start, int n)
        {
            Array.Copy(d, start, window, 0, n);
            readAt = writeAt = n;
        }

        internal int SyncPoint()
        {
            return mode == InflateBlockMode.LENS ? 1 : 0;
        }

        private enum InflateBlockMode
        {
            TYPE = 0, // get type bits (3, including end bit)
            LENS = 1, // get lengths for stored
            STORED = 2, // processing stored block
            TABLE = 3, // get table lengths
            BTREE = 4, // get bit lengths tree for a dynamic block
            DTREE = 5, // get length, distance trees for a dynamic block
            CODES = 6, // processing fixed or dynamic block
            DRY = 7, // output remaining window bytes
            DONE = 8, // finished last block, done
            BAD = 9 // ot a data error--stuck here
        }
    }

    internal sealed class InflateCodes
    {
        internal int bitsToGet;
        internal byte dbits;
        internal int dist;
        internal int[] dtree;
        internal int dtree_index;
        internal byte lbits;
        internal int len;
        internal int lit;
        internal int[] ltree;
        internal int ltree_index;
        internal int mode;
        internal int need;
        internal int[] tree;
        internal int tree_index;

        private const int BADCODE = 9;
        private const int COPY = 5;
        private const int DIST = 3;
        private const int DISTEXT = 4;
        private const int END = 8;
        private const int LEN = 1;
        private const int LENEXT = 2;
        private const int LIT = 6;
        private const int START = 0;
        private const int WASH = 7;

        internal int InflateFast(int bl, int bd, int[] tl, int tlIndex, int[] td, int tdIndex, InflateBlocks s,
            ZlibCodec z)
        {
            int c; 

            var p = z.NextIn;
            var n = z.AvailableBytesIn;
            var b = s.bitb;
            var k = s.bitk;
            var q = s.writeAt;
            var m = q < s.readAt ? s.readAt - q - 1 : s.end - q;

            var ml = InternalInflateConstants.InflateMask[bl];
            var md = InternalInflateConstants.InflateMask[bd];

            do
            {
                while (k < 20)
                {
                    n--;
                    b |= (z.InputBuffer[p++] & 0xff) << k;
                    k += 8;
                }

                var t = b & ml;
                var tp = tl;
                var tpIndex = tlIndex;
                var tpIndexT3 = (tpIndex + t) * 3;
                int e;
                if ((e = tp[tpIndexT3]) == 0)
                {
                    b >>= tp[tpIndexT3 + 1];
                    k -= tp[tpIndexT3 + 1];

                    s.window[q++] = (byte) tp[tpIndexT3 + 2];
                    m--;
                    continue;
                }

                do
                {
                    b >>= tp[tpIndexT3 + 1];
                    k -= tp[tpIndexT3 + 1];

                    if ((e & 16) != 0)
                    {
                        e &= 15;
                        c = tp[tpIndexT3 + 2] + (b & InternalInflateConstants.InflateMask[e]);

                        b >>= e;
                        k -= e;

                        while (k < 15)
                        {
                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        t = b & md;
                        tp = td;
                        tpIndex = tdIndex;
                        tpIndexT3 = (tpIndex + t) * 3;
                        e = tp[tpIndexT3];

                        do
                        {
                            b >>= tp[tpIndexT3 + 1];
                            k -= tp[tpIndexT3 + 1];

                            if ((e & 16) != 0)
                            {
                                e &= 15;
                                while (k < e)
                                {
                                    n--;
                                    b |= (z.InputBuffer[p++] & 0xff) << k;
                                    k += 8;
                                }

                                var d = tp[tpIndexT3 + 2] + (b & InternalInflateConstants.InflateMask[e]);

                                b >>= e;
                                k -= e;

                                m -= c;
                                int r;
                                if (q >= d)
                                {
                                    r = q - d;
                                    if (q - r > 0 && 2 > q - r)
                                    {
                                        s.window[q++] = s.window[r++]; 
                                        s.window[q++] = s.window[r++]; 
                                        c -= 2;
                                    }
                                    else
                                    {
                                        Array.Copy(s.window, r, s.window, q, 2);
                                        q += 2;
                                        r += 2;
                                        c -= 2;
                                    }
                                }
                                else
                                {
                                    r = q - d;
                                    do
                                    {
                                        r += s.end; 
                                    } while (r < 0); 

                                    e = s.end - r;
                                    if (c > e)
                                    {
                                        c -= e; 
                                        if (q - r > 0 && e > q - r)
                                        {
                                            do
                                            {
                                                s.window[q++] = s.window[r++];
                                            } while (--e != 0);
                                        }
                                        else
                                        {
                                            Array.Copy(s.window, r, s.window, q, e);
                                            q += e;
                                        }

                                        r = 0; 
                                    }
                                }

                                if (q - r > 0 && c > q - r)
                                {
                                    do
                                    {
                                        s.window[q++] = s.window[r++];
                                    } while (--c != 0);
                                }
                                else
                                {
                                    Array.Copy(s.window, r, s.window, q, c);
                                    q += c;
                                }

                                break;
                            }

                            if ((e & 64) == 0)
                            {
                                t += tp[tpIndexT3 + 2];
                                t += b & InternalInflateConstants.InflateMask[e];
                                tpIndexT3 = (tpIndex + t) * 3;
                                e = tp[tpIndexT3];
                            }
                            else
                            {
                                z.Message = "invalid distance code";

                                c = z.AvailableBytesIn - n;
                                c = k >> 3 < c ? k >> 3 : c;
                                n += c;
                                p -= c;
                                k -= c << 3;

                                s.bitb = b;
                                s.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                s.writeAt = q;

                                return ZlibConstants.ZDataError;
                            }
                        } while (true);

                        break;
                    }

                    if ((e & 64) == 0)
                    {
                        t += tp[tpIndexT3 + 2];
                        t += b & InternalInflateConstants.InflateMask[e];
                        tpIndexT3 = (tpIndex + t) * 3;
                        if ((e = tp[tpIndexT3]) != 0) continue;

                        b >>= tp[tpIndexT3 + 1];
                        k -= tp[tpIndexT3 + 1];
                        s.window[q++] = (byte) tp[tpIndexT3 + 2];
                        m--;
                        break;
                    }

                    if ((e & 32) != 0)
                    {
                        c = z.AvailableBytesIn - n;
                        c = k >> 3 < c ? k >> 3 : c;
                        n += c;
                        p -= c;
                        k -= c << 3;

                        s.bitb = b;
                        s.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        s.writeAt = q;

                        return ZlibConstants.ZStreamEnd;
                    }

                    z.Message = "invalid literal/length code";

                    c = z.AvailableBytesIn - n;
                    c = k >> 3 < c ? k >> 3 : c;
                    n += c;
                    p -= c;
                    k -= c << 3;

                    s.bitb = b;
                    s.bitk = k;
                    z.AvailableBytesIn = n;
                    z.TotalBytesIn += p - z.NextIn;
                    z.NextIn = p;
                    s.writeAt = q;

                    return ZlibConstants.ZDataError;
                } while (true);
            } while (m >= 258 && n >= 10);

            c = z.AvailableBytesIn - n;
            c = k >> 3 < c ? k >> 3 : c;
            n += c;
            p -= c;
            k -= c << 3;

            s.bitb = b;
            s.bitk = k;
            z.AvailableBytesIn = n;
            z.TotalBytesIn += p - z.NextIn;
            z.NextIn = p;
            s.writeAt = q;

            return ZlibConstants.ZOk;
        }

        internal void Init(int bl, int bd, int[] tl, int tlIndex, int[] td, int tdIndex)
        {
            mode = START;
            lbits = (byte) bl;
            dbits = (byte) bd;
            ltree = tl;
            ltree_index = tlIndex;
            dtree = td;
            dtree_index = tdIndex;
            tree = null;
        }

        internal int Process(InflateBlocks blocks, int r)
        {
            int j; 
            int tindex;
            int e; 

            var z = blocks._codec;

            var p = z.NextIn;
            var n = z.AvailableBytesIn;
            var b = blocks.bitb;
            var k = blocks.bitk;
            var q = blocks.writeAt;
            var m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

            while (true)
                switch (mode)
                {
                    case START:
                        if (m >= 258 && n >= 10)
                        {
                            blocks.bitb = b;
                            blocks.bitk = k;
                            z.AvailableBytesIn = n;
                            z.TotalBytesIn += p - z.NextIn;
                            z.NextIn = p;
                            blocks.writeAt = q;
                            r = InflateFast(lbits, dbits, ltree, ltree_index, dtree, dtree_index, blocks, z);

                            p = z.NextIn;
                            n = z.AvailableBytesIn;
                            b = blocks.bitb;
                            k = blocks.bitk;
                            q = blocks.writeAt;
                            m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

                            if (r != ZlibConstants.ZOk)
                            {
                                mode = r == ZlibConstants.ZStreamEnd ? WASH : BADCODE;
                                break;
                            }
                        }

                        need = lbits;
                        tree = ltree;
                        tree_index = ltree_index;

                        mode = LEN;
                        goto case LEN;

                    case LEN:
                        j = need;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.ZOk;
                            }
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        tindex = (tree_index + (b & InternalInflateConstants.InflateMask[j])) * 3;

                        b >>= tree[tindex + 1];
                        k -= tree[tindex + 1];

                        e = tree[tindex];

                        if (e == 0)
                        {
                            lit = tree[tindex + 2];
                            mode = LIT;
                            break;
                        }

                        if ((e & 16) != 0)
                        {
                            bitsToGet = e & 15;
                            len = tree[tindex + 2];
                            mode = LENEXT;
                            break;
                        }

                        if ((e & 64) == 0)
                        {
                            need = e;
                            tree_index = tindex / 3 + tree[tindex + 2];
                            break;
                        }

                        if ((e & 32) != 0)
                        {
                            mode = WASH;
                            break;
                        }

                        mode = BADCODE; 
                        z.Message = "invalid literal/length code";
                        r = ZlibConstants.ZDataError;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    case LENEXT: 
                        j = bitsToGet;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.ZOk;
                            }
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        len += b & InternalInflateConstants.InflateMask[j];

                        b >>= j;
                        k -= j;

                        need = dbits;
                        tree = dtree;
                        tree_index = dtree_index;
                        mode = DIST;
                        goto case DIST;

                    case DIST: 
                        j = need;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.ZOk;
                            }
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        tindex = (tree_index + (b & InternalInflateConstants.InflateMask[j])) * 3;

                        b >>= tree[tindex + 1];
                        k -= tree[tindex + 1];

                        e = tree[tindex];
                        if ((e & 0x10) != 0)
                        {
                            bitsToGet = e & 15;
                            dist = tree[tindex + 2];
                            mode = DISTEXT;
                            break;
                        }

                        if ((e & 64) == 0)
                        {
                            need = e;
                            tree_index = tindex / 3 + tree[tindex + 2];
                            break;
                        }

                        mode = BADCODE; 
                        z.Message = "invalid distance code";
                        r = ZlibConstants.ZDataError;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    case DISTEXT: 
                        j = bitsToGet;

                        while (k < j)
                        {
                            if (n != 0)
                            {
                                r = ZlibConstants.ZOk;
                            }
                            else
                            {
                                blocks.bitb = b;
                                blocks.bitk = k;
                                z.AvailableBytesIn = n;
                                z.TotalBytesIn += p - z.NextIn;
                                z.NextIn = p;
                                blocks.writeAt = q;
                                return blocks.Flush(r);
                            }

                            n--;
                            b |= (z.InputBuffer[p++] & 0xff) << k;
                            k += 8;
                        }

                        dist += b & InternalInflateConstants.InflateMask[j];

                        b >>= j;
                        k -= j;

                        mode = COPY;
                        goto case COPY;

                    case COPY:
                        var f = q - dist;
                        while (f < 0)
                            f += blocks.end; 
                        while (len != 0)
                        {
                            if (m == 0)
                            {
                                if (q == blocks.end && blocks.readAt != 0)
                                {
                                    q = 0;
                                    m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                                }

                                if (m == 0)
                                {
                                    blocks.writeAt = q;
                                    r = blocks.Flush(r);
                                    q = blocks.writeAt;
                                    m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

                                    if (q == blocks.end && blocks.readAt != 0)
                                    {
                                        q = 0;
                                        m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                                    }

                                    if (m == 0)
                                    {
                                        blocks.bitb = b;
                                        blocks.bitk = k;
                                        z.AvailableBytesIn = n;
                                        z.TotalBytesIn += p - z.NextIn;
                                        z.NextIn = p;
                                        blocks.writeAt = q;
                                        return blocks.Flush(r);
                                    }
                                }
                            }

                            blocks.window[q++] = blocks.window[f++];
                            m--;

                            if (f == blocks.end)
                                f = 0;
                            len--;
                        }

                        mode = START;
                        break;

                    case LIT: 
                        if (m == 0)
                        {
                            if (q == blocks.end && blocks.readAt != 0)
                            {
                                q = 0;
                                m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                            }

                            if (m == 0)
                            {
                                blocks.writeAt = q;
                                r = blocks.Flush(r);
                                q = blocks.writeAt;
                                m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;

                                if (q == blocks.end && blocks.readAt != 0)
                                {
                                    q = 0;
                                    m = q < blocks.readAt ? blocks.readAt - q - 1 : blocks.end - q;
                                }

                                if (m == 0)
                                {
                                    blocks.bitb = b;
                                    blocks.bitk = k;
                                    z.AvailableBytesIn = n;
                                    z.TotalBytesIn += p - z.NextIn;
                                    z.NextIn = p;
                                    blocks.writeAt = q;
                                    return blocks.Flush(r);
                                }
                            }
                        }

                        r = ZlibConstants.ZOk;

                        blocks.window[q++] = (byte) lit;
                        m--;

                        mode = START;
                        break;

                    case WASH: 
                        if (k > 7)
                        {
                            k -= 8;
                            n++;
                            p--; 
                        }

                        blocks.writeAt = q;
                        r = blocks.Flush(r);
                        q = blocks.writeAt;

                        if (blocks.readAt != blocks.writeAt)
                        {
                            blocks.bitb = b;
                            blocks.bitk = k;
                            z.AvailableBytesIn = n;
                            z.TotalBytesIn += p - z.NextIn;
                            z.NextIn = p;
                            blocks.writeAt = q;
                            return blocks.Flush(r);
                        }

                        mode = END;
                        goto case END;

                    case END:
                        r = ZlibConstants.ZStreamEnd;
                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    case BADCODE: 

                        r = ZlibConstants.ZDataError;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);

                    default:
                        r = ZlibConstants.ZStreamError;

                        blocks.bitb = b;
                        blocks.bitk = k;
                        z.AvailableBytesIn = n;
                        z.TotalBytesIn += p - z.NextIn;
                        z.NextIn = p;
                        blocks.writeAt = q;
                        return blocks.Flush(r);
                }
        }
    }

    internal sealed class InflateManager
    {
        internal bool HandleRfc1950HeaderBytes { get; set; } = true;

        private enum InflateManagerMode
        {
            Method = 0,
            Flag = 1, 
            Dict4 = 2, 
            Dict3 = 3, 
            Dict2 = 4, 
            Dict1 = 5, 
            Dict0 = 6, 
            Blocks = 7, 
            Check4 = 8, 
            Check3 = 9, 
            Check2 = 10, 
            Check1 = 11,
            Done = 12, 
            Bad = 13 
        }

        internal ZlibCodec Codec;
        internal InflateBlocks Blocks;
        internal uint ComputedCheck;
        internal uint ExpectedCheck;
        internal int Marker;
        internal int Method;
        internal int Wbits;

        private const int PresetDict = 0x20;

        private const int ZDeflated = 8;

        private static readonly byte[] Mark = {0, 0, 0xff, 0xff};

        private InflateManagerMode _mode;

        public InflateManager()
        {
        }

        public InflateManager(bool expectRfc1950HeaderBytes)
        {
            HandleRfc1950HeaderBytes = expectRfc1950HeaderBytes;
        }

        internal int End()
        {
            Blocks?.Free();
            Blocks = null;
            return ZlibConstants.ZOk;
        }

        internal int Inflate(FlushType flush)
        {
            if (Codec.InputBuffer == null)
                throw new ZlibException("InputBuffer is null. ");

            const int f = ZlibConstants.ZOk;
            var r = ZlibConstants.ZBufError;

            while (true)
                switch (_mode)
                {
                    case InflateManagerMode.Method:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        if (((Method = Codec.InputBuffer[Codec.NextIn++]) & 0xf) != ZDeflated)
                        {
                            _mode = InflateManagerMode.Bad;
                            Codec.Message = $"unknown compression method (0x{Method:X2})";
                            Marker = 5; 
                            break;
                        }

                        if ((Method >> 4) + 8 > Wbits)
                        {
                            _mode = InflateManagerMode.Bad;
                            Codec.Message = $"invalid window size ({(Method >> 4) + 8})";
                            Marker = 5;
                            break;
                        }

                        _mode = InflateManagerMode.Flag;
                        break;

                    case InflateManagerMode.Flag:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        var b = Codec.InputBuffer[Codec.NextIn++] & 0xff;

                        if (((Method << 8) + b) % 31 != 0)
                        {
                            _mode = InflateManagerMode.Bad;
                            Codec.Message = "incorrect header check";
                            Marker = 5; 
                            break;
                        }

                        _mode = (b & PresetDict) == 0
                            ? InflateManagerMode.Blocks
                            : InflateManagerMode.Dict4;
                        break;

                    case InflateManagerMode.Dict4:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck = (uint) ((Codec.InputBuffer[Codec.NextIn++] << 24) & 0xff000000);
                        _mode = InflateManagerMode.Dict3;
                        break;

                    case InflateManagerMode.Dict3:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck += (uint) ((Codec.InputBuffer[Codec.NextIn++] << 16) & 0x00ff0000);
                        _mode = InflateManagerMode.Dict2;
                        break;

                    case InflateManagerMode.Dict2:

                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck += (uint) ((Codec.InputBuffer[Codec.NextIn++] << 8) & 0x0000ff00);
                        _mode = InflateManagerMode.Dict1;
                        break;

                    case InflateManagerMode.Dict1:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck += (uint) (Codec.InputBuffer[Codec.NextIn++] & 0x000000ff);
                        Codec._Adler32 = ExpectedCheck;
                        _mode = InflateManagerMode.Dict0;
                        return ZlibConstants.ZNeedDict;

                    case InflateManagerMode.Dict0:
                        _mode = InflateManagerMode.Bad;
                        Codec.Message = "need dictionary";
                        Marker = 0; 
                        return ZlibConstants.ZStreamError;

                    case InflateManagerMode.Blocks:
                        r = Blocks.Process(r);
                        if (r == ZlibConstants.ZDataError)
                        {
                            _mode = InflateManagerMode.Bad;
                            Marker = 0; 
                            break;
                        }

                        if (r == ZlibConstants.ZOk)
                            r = f;

                        if (r != ZlibConstants.ZStreamEnd)
                            return r;

                        r = f;
                        ComputedCheck = Blocks.Reset();
                        if (!HandleRfc1950HeaderBytes)
                        {
                            _mode = InflateManagerMode.Done;
                            return ZlibConstants.ZStreamEnd;
                        }

                        _mode = InflateManagerMode.Check4;
                        break;

                    case InflateManagerMode.Check4:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck = (uint) ((Codec.InputBuffer[Codec.NextIn++] << 24) & 0xff000000);
                        _mode = InflateManagerMode.Check3;
                        break;

                    case InflateManagerMode.Check3:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck += (uint) ((Codec.InputBuffer[Codec.NextIn++] << 16) & 0x00ff0000);
                        _mode = InflateManagerMode.Check2;
                        break;

                    case InflateManagerMode.Check2:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck += (uint) ((Codec.InputBuffer[Codec.NextIn++] << 8) & 0x0000ff00);
                        _mode = InflateManagerMode.Check1;
                        break;

                    case InflateManagerMode.Check1:
                        if (Codec.AvailableBytesIn == 0)
                            return r;

                        r = f;
                        Codec.AvailableBytesIn--;
                        Codec.TotalBytesIn++;
                        ExpectedCheck += (uint) (Codec.InputBuffer[Codec.NextIn++] & 0x000000ff);
                        if (ComputedCheck != ExpectedCheck)
                        {
                            _mode = InflateManagerMode.Bad;
                            Codec.Message = "incorrect data check";
                            Marker = 5; 
                            break;
                        }

                        _mode = InflateManagerMode.Done;
                        return ZlibConstants.ZStreamEnd;

                    case InflateManagerMode.Done:
                        return ZlibConstants.ZStreamEnd;

                    case InflateManagerMode.Bad:
                        throw new ZlibException($"Bad state ({Codec.Message})");

                    default:
                        throw new ZlibException("Stream error.");
                }
        }

        internal int Initialize(ZlibCodec codec, int w)
        {
            Codec = codec;
            Codec.Message = null;
            Blocks = null;

            if (w < 8 || w > 15)
            {
                End();
                throw new ZlibException("Bad window size.");
            }

            Wbits = w;

            Blocks = new InflateBlocks(codec,
                HandleRfc1950HeaderBytes ? this : null,
                1 << w);

            Reset();
            return ZlibConstants.ZOk;
        }

        internal int Reset()
        {
            Codec.TotalBytesIn = Codec.TotalBytesOut = 0;
            Codec.Message = null;
            _mode = HandleRfc1950HeaderBytes ? InflateManagerMode.Method : InflateManagerMode.Blocks;
            Blocks.Reset();
            return ZlibConstants.ZOk;
        }

        internal int SetDictionary(byte[] dictionary)
        {
            var index = 0;
            var length = dictionary.Length;
            if (_mode != InflateManagerMode.Dict0)
                throw new ZlibException("Stream error.");

            if (Adler.Adler32(1, dictionary, 0, dictionary.Length) != Codec._Adler32)
                return ZlibConstants.ZDataError;

            Codec._Adler32 = Adler.Adler32(0, null, 0, 0);

            if (length >= 1 << Wbits)
            {
                length = (1 << Wbits) - 1;
                index = dictionary.Length - length;
            }

            Blocks.SetDictionary(dictionary, index, length);
            _mode = InflateManagerMode.Blocks;
            return ZlibConstants.ZOk;
        }

        internal int Sync()
        {
            int n;

            if (_mode != InflateManagerMode.Bad)
            {
                _mode = InflateManagerMode.Bad;
                Marker = 0;
            }

            if ((n = Codec.AvailableBytesIn) == 0)
                return ZlibConstants.ZBufError;

            var p = Codec.NextIn;
            var m = Marker;

            while (n != 0 && m < 4)
            {
                if (Codec.InputBuffer[p] == Mark[m]) m++;
                else if (Codec.InputBuffer[p] != 0) m = 0;
                else m = 4 - m;
                p++;
                n--;
            }

            Codec.TotalBytesIn += p - Codec.NextIn;
            Codec.NextIn = p;
            Codec.AvailableBytesIn = n;
            Marker = m;

            if (m != 4) return ZlibConstants.ZDataError;

            var r = Codec.TotalBytesIn;
            var w = Codec.TotalBytesOut;
            Reset();
            Codec.TotalBytesIn = r;
            Codec.TotalBytesOut = w;
            _mode = InflateManagerMode.Blocks;
            return ZlibConstants.ZOk;
        }

        internal int SyncPoint(ZlibCodec z)
        {
            return Blocks.SyncPoint();
        }
    }
}