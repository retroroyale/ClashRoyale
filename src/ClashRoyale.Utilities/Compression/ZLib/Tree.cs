using System;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    internal sealed class Tree
    {
        internal static readonly sbyte[] BlOrder = {16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15};

        internal static readonly int[] DistanceBase =
        {
            0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192,
            256, 384, 512, 768, 1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576
        };

        internal static readonly int[] ExtraBlbits = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7};

        internal static readonly int[] ExtraDistanceBits =
        {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6,
            7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
        };

        internal static readonly int[] ExtraLengthBits =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
            3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
        };

        internal static readonly int[] LengthBase =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 14, 16, 20, 24, 28,
            32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 0
        };

        internal static readonly sbyte[] LengthCode =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 9, 10, 10, 11, 11,
            12, 12, 12, 12, 13, 13, 13, 13, 14, 14, 14, 14, 15, 15, 15, 15,
            16, 16, 16, 16, 16, 16, 16, 16, 17, 17, 17, 17, 17, 17, 17, 17,
            18, 18, 18, 18, 18, 18, 18, 18, 19, 19, 19, 19, 19, 19, 19, 19,
            20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20,
            21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
            22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
            23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28
        };

        internal short[] DynTree;

        internal int MaxCode;

        internal StaticTree StaticTree;

        private static readonly sbyte[] DistCode =
        {
            0, 1, 2, 3, 4, 4, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7,
            8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 9, 9, 9, 9,
            10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
            11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
            12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
            12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
            13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
            13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            0, 0, 16, 17, 18, 18, 19, 19, 20, 20, 20, 20, 21, 21, 21, 21,
            22, 22, 22, 22, 22, 22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23,
            24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
            25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
            27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
            29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29
        };

        private static readonly int HeapSize = 2 * InternalConstants.L_CODES + 1;

        internal static int Bi_reverse(int code, int len)
        {
            var res = 0;
            do
            {
                res |= code & 1;
                code >>= 1; 
                res <<= 1;
            } while (--len > 0);

            return res >> 1;
        }

        internal static int DistanceCode(int dist)
        {
            return dist < 256
                ? DistCode[dist]
                : DistCode[256 + SharedUtils.UrShift(dist, 7)];
        }

        internal static void Gen_codes(short[] tree, int maxCode, short[] blCount)
        {
            var nextCode = new short[InternalConstants.MAX_BITS + 1];
            short code = 0; 
            int bits; 
            int n; 

            for (bits = 1; bits <= InternalConstants.MAX_BITS; bits++)
                unchecked
                {
                    nextCode[bits] = code = (short) ((code + blCount[bits - 1]) << 1);
                }

            for (n = 0; n <= maxCode; n++)
            {
                int len = tree[n * 2 + 1];
                if (len == 0)
                    continue;

                tree[n * 2] = unchecked((short) Bi_reverse(nextCode[len]++, len));
            }
        }

        internal void Build_tree(DeflateManager s)
        {
            var tree = DynTree;
            var stree = StaticTree.treeCodes;
            var elems = StaticTree.elems;
            int n;
            var maxCode = -1;
            int node; 

            s.HeapLen = 0;
            s.HeapMax = HeapSize;

            for (n = 0; n < elems; n++)
                if (tree[n * 2] != 0)
                {
                    s.Heap[++s.HeapLen] = maxCode = n;
                    s.Depth[n] = 0;
                }
                else
                {
                    tree[n * 2 + 1] = 0;
                }

            while (s.HeapLen < 2)
            {
                node = s.Heap[++s.HeapLen] = maxCode < 2 ? ++maxCode : 0;
                tree[node * 2] = 1;
                s.Depth[node] = 0;
                s.OptLen--;
                if (stree != null)
                    s.StaticLen -= stree[node * 2 + 1];
            }

            MaxCode = maxCode;

            for (n = s.HeapLen / 2; n >= 1; n--)
                s.Pqdownheap(tree, n);

            node = elems; 
            do
            {
                n = s.Heap[1];
                s.Heap[1] = s.Heap[s.HeapLen--];
                s.Pqdownheap(tree, 1);
                var m = s.Heap[1];

                s.Heap[--s.HeapMax] = n; 
                s.Heap[--s.HeapMax] = m;

                tree[node * 2] = unchecked((short) (tree[n * 2] + tree[m * 2]));
                s.Depth[node] = (sbyte) (Math.Max((byte) s.Depth[n], (byte) s.Depth[m]) + 1);
                tree[n * 2 + 1] = tree[m * 2 + 1] = (short) node;

                s.Heap[1] = node++;
                s.Pqdownheap(tree, 1);
            } while (s.HeapLen >= 2);

            s.Heap[--s.HeapMax] = s.Heap[1];

            Gen_bitlen(s);
            Gen_codes(tree, maxCode, s.BlCount);
        }

        internal void Gen_bitlen(DeflateManager s)
        {
            var tree = DynTree;
            var stree = StaticTree.treeCodes;
            var extra = StaticTree.extraBits;
            var baseRenamed = StaticTree.extraBase;
            var maxLength = StaticTree.maxLength;
            int h;
            int n; 
            int bits;
            var overflow = 0; 

            for (bits = 0; bits <= InternalConstants.MAX_BITS; bits++)
                s.BlCount[bits] = 0;

            tree[s.Heap[s.HeapMax] * 2 + 1] = 0; 

            for (h = s.HeapMax + 1; h < HeapSize; h++)
            {
                n = s.Heap[h];
                bits = tree[tree[n * 2 + 1] * 2 + 1] + 1;
                if (bits > maxLength)
                {
                    bits = maxLength;
                    overflow++;
                }

                tree[n * 2 + 1] = (short) bits;

                if (n > MaxCode)
                    continue; 

                s.BlCount[bits]++;
                var xbits = 0;
                if (n >= baseRenamed)
                    xbits = extra[n - baseRenamed];
                var f = tree[n * 2]; 
                s.OptLen += f * (bits + xbits);
                if (stree != null)
                    s.StaticLen += f * (stree[n * 2 + 1] + xbits);
            }

            if (overflow == 0)
                return;

            do
            {
                bits = maxLength - 1;
                while (s.BlCount[bits] == 0)
                    bits--;
                s.BlCount[bits]--; 
                s.BlCount[bits + 1] = (short) (s.BlCount[bits + 1] + 2); 
                s.BlCount[maxLength]--;
                overflow -= 2;
            } while (overflow > 0);

            for (bits = maxLength; bits != 0; bits--)
            {
                n = s.BlCount[bits];
                while (n != 0)
                {
                    var m = s.Heap[--h]; 
                    if (m > MaxCode)
                        continue;

                    if (tree[m * 2 + 1] != bits)
                    {
                        s.OptLen = (int) (s.OptLen + (bits - (long) tree[m * 2 + 1]) * tree[m * 2]);
                        tree[m * 2 + 1] = (short) bits;
                    }

                    n--;
                }
            }
        }
    }
}