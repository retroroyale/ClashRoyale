using System;

namespace ClashRoyale.Utilities.Compression.ZLib
{
    internal enum BlockState
    {
        NeedMore = 0, 
        BlockDone, 
        FinishStarted,
        FinishDone 
    }

    internal enum DeflateFlavor
    {
        Store,
        Fast,
        Slow
    }

    internal sealed class DeflateManager
    {
        private const int MemLevelMax = 9;
        private const int MemLevelDefault = 8;

        internal delegate BlockState CompressFunc(FlushType flush);

        internal class Config
        {
            private static readonly Config[] Table;

            internal DeflateFlavor Flavor;
            internal int GoodLength;
            internal int MaxChainLength;
            internal int MaxLazy; 
            internal int NiceLength; 

            static Config()
            {
                Table = new[]
                {
                    new Config(0, 0, 0, 0, DeflateFlavor.Store),
                    new Config(4, 4, 8, 4, DeflateFlavor.Fast),
                    new Config(4, 5, 16, 8, DeflateFlavor.Fast),
                    new Config(4, 6, 32, 32, DeflateFlavor.Fast),
                    new Config(4, 4, 16, 16, DeflateFlavor.Slow),
                    new Config(8, 16, 32, 32, DeflateFlavor.Slow),
                    new Config(8, 16, 128, 128, DeflateFlavor.Slow),
                    new Config(8, 32, 128, 256, DeflateFlavor.Slow),
                    new Config(32, 128, 258, 1024, DeflateFlavor.Slow),
                    new Config(32, 258, 258, 4096, DeflateFlavor.Slow)
                };
            }

            private Config(int goodLength, int maxLazy, int niceLength, int maxChainLength, DeflateFlavor flavor)
            {
                GoodLength = goodLength;
                MaxLazy = maxLazy;
                NiceLength = niceLength;
                MaxChainLength = maxChainLength;
                Flavor = flavor;
            }

            public static Config Lookup(CompressionLevel level)
            {
                return Table[(int) level];
            }
        }

        private CompressFunc _deflateFunction;

        private static readonly string[] ErrorMessage =
        {
            "need dictionary",
            "stream end",
            "",
            "file error",
            "stream error",
            "data error",
            "insufficient memory",
            "buffer error",
            "incompatible version",
            ""
        };

        private const int PresetDict = 0x20;

        private const int InitState = 42;
        private const int BusyState = 113;
        public static readonly int FinishState = 666;

        private const int ZDeflated = 8;

        private const int StoredBlock = 0;
        private const int StaticTrees = 1;
        private const int DynTrees = 2;

        public static readonly int ZBinary = 0;
        private const int ZAscii = 1;
        private const int ZUnknown = 2;

        private const int BufSize = 8 * 2;

        private const int MinMatch = 3;
        private const int MaxMatch = 258;

        private const int MinLookahead = MaxMatch + MinMatch + 1;

        private static readonly int HeapSize = 2 * InternalConstants.L_CODES + 1;

        private const int EndBlock = 256;

        internal ZlibCodec Codec; 
        internal int Status; 
        internal byte[] Pending; 
        internal int NextPending; 
        internal int PendingCount; 

        internal sbyte DataType; 
        internal int LastFlush; 

        internal int WSize; 
        internal int WBits; 
        internal int WMask; 

        internal byte[] Window;
        internal int WindowSize;

        internal short[] Prev;
        internal short[] Head; 

        internal int InsH; 
        internal int HashSize;
        internal int HashBits;
        public int HashMask;
        internal int HashShift;

        internal int BlockStart;

        private Config _config;
        internal int MatchLength; 
        internal int PrevMatch; 
        internal int MatchAvailable; 
        internal int Strstart; 
        internal int MatchStart; 
        internal int Lookahead; 

        internal int PrevLength;

        internal CompressionLevel CompressionLevel; 
        internal CompressionStrategy CompressionStrategy; 

        internal short[] DynLtree; 
        internal short[] DynDtree; 
        internal short[] BlTree;

        internal Tree TreeLiterals = new Tree(); 
        internal Tree TreeDistances = new Tree(); 
        internal Tree TreeBitLengths = new Tree(); 

        internal short[] BlCount = new short[InternalConstants.MAX_BITS + 1];
        internal int[] Heap = new int[2 * InternalConstants.L_CODES + 1];

        internal int HeapLen; 
        internal int HeapMax;

        internal sbyte[] Depth = new sbyte[2 * InternalConstants.L_CODES + 1];

        internal int LengthOffset; 

        internal int LitBufsize;

        internal int LastLit; 

        internal int DistanceOffset; 

        internal int OptLen; 
        internal int StaticLen; 
        internal int Matches; 
        internal int LastEobLen; 

        internal short BiBuf;

        internal int BiValid;

        internal DeflateManager()
        {
            DynLtree = new short[HeapSize * 2];
            DynDtree = new short[(2 * InternalConstants.D_CODES + 1) * 2]; 
            BlTree = new short[(2 * InternalConstants.BL_CODES + 1) * 2];
        }

        private void _InitializeLazyMatch()
        {
            WindowSize = 2 * WSize;

            Array.Clear(Head, 0, HashSize);

            _config = Config.Lookup(CompressionLevel);
            SetDeflater();

            Strstart = 0;
            BlockStart = 0;
            Lookahead = 0;
            MatchLength = PrevLength = MinMatch - 1;
            MatchAvailable = 0;
            InsH = 0;
        }

        private void InitializeTreeData()
        {
            TreeLiterals.DynTree = DynLtree;
            TreeLiterals.StaticTree = StaticTree.Literals;

            TreeDistances.DynTree = DynDtree;
            TreeDistances.StaticTree = StaticTree.Distances;

            TreeBitLengths.DynTree = BlTree;
            TreeBitLengths.StaticTree = StaticTree.BitLengths;

            BiBuf = 0;
            BiValid = 0;
            LastEobLen = 8; 

            InitializeBlocks();
        }

        internal void InitializeBlocks()
        {
            for (var i = 0; i < InternalConstants.L_CODES; i++)
                DynLtree[i * 2] = 0;
            for (var i = 0; i < InternalConstants.D_CODES; i++)
                DynDtree[i * 2] = 0;
            for (var i = 0; i < InternalConstants.BL_CODES; i++)
                BlTree[i * 2] = 0;

            DynLtree[EndBlock * 2] = 1;
            OptLen = StaticLen = 0;
            LastLit = Matches = 0;
        }

        internal void Pqdownheap(short[] tree, int k)
        {
            var v = Heap[k];
            var j = k << 1; 
            while (j <= HeapLen)
            {
                if (j < HeapLen && IsSmaller(tree, Heap[j + 1], Heap[j], Depth)) j++;
                if (IsSmaller(tree, v, Heap[j], Depth))
                    break;

                Heap[k] = Heap[j];
                k = j;
                j <<= 1;
            }

            Heap[k] = v;
        }

        internal static bool IsSmaller(short[] tree, int n, int m, sbyte[] depth)
        {
            var tn2 = tree[n * 2];
            var tm2 = tree[m * 2];
            return tn2 < tm2 || tn2 == tm2 && depth[n] <= depth[m];
        }

        internal void Scan_tree(short[] tree, int maxCode)
        {
            int n; 
            var prevlen = -1; 
            int nextlen = tree[0 * 2 + 1]; 
            var count = 0; 
            var maxCount = 7; 
            var minCount = 4; 

            if (nextlen == 0)
            {
                maxCount = 138;
                minCount = 3;
            }

            tree[(maxCode + 1) * 2 + 1] = 0x7fff; 

            for (n = 0; n <= maxCode; n++)
            {
                var curlen = nextlen; 
                nextlen = tree[(n + 1) * 2 + 1];
                if (++count < maxCount && curlen == nextlen) continue;

                if (count < minCount)
                {
                    BlTree[curlen * 2] = (short) (BlTree[curlen * 2] + count);
                }
                else if (curlen != 0)
                {
                    if (curlen != prevlen)
                        BlTree[curlen * 2]++;
                    BlTree[InternalConstants.REP_3_6 * 2]++;
                }
                else if (count <= 10)
                {
                    BlTree[InternalConstants.REPZ_3_10 * 2]++;
                }
                else
                {
                    BlTree[InternalConstants.REPZ_11_138 * 2]++;
                }

                count = 0;
                prevlen = curlen;
                if (nextlen == 0)
                {
                    maxCount = 138;
                    minCount = 3;
                }
                else if (curlen == nextlen)
                {
                    maxCount = 6;
                    minCount = 3;
                }
                else
                {
                    maxCount = 7;
                    minCount = 4;
                }
            }
        }

        internal int Build_bl_tree()
        {
            Scan_tree(DynLtree, TreeLiterals.MaxCode);
            Scan_tree(DynDtree, TreeDistances.MaxCode);

            TreeBitLengths.Build_tree(this);

            int maxBlindex;

            for (maxBlindex = InternalConstants.BL_CODES - 1; maxBlindex >= 3; maxBlindex--)
                if (BlTree[Tree.BlOrder[maxBlindex] * 2 + 1] != 0)
                    break;

            OptLen += 3 * (maxBlindex + 1) + 5 + 5 + 4;

            return maxBlindex;
        }

        internal void Send_all_trees(int lcodes, int dcodes, int blcodes)
        {
            int rank; 

            Send_bits(lcodes - 257, 5);
            Send_bits(dcodes - 1, 5);
            Send_bits(blcodes - 4, 4); 
            for (rank = 0; rank < blcodes; rank++) Send_bits(BlTree[Tree.BlOrder[rank] * 2 + 1], 3);
            Send_tree(DynLtree, lcodes - 1); 
            Send_tree(DynDtree, dcodes - 1); 
        }

        internal void Send_tree(short[] tree, int maxCode)
        {
            int n; 
            var prevlen = -1;
            int nextlen = tree[0 * 2 + 1]; 
            var count = 0; 
            var maxCount = 7;
            var minCount = 4; 

            if (nextlen == 0)
            {
                maxCount = 138;
                minCount = 3;
            }

            for (n = 0; n <= maxCode; n++)
            {
                var curlen = nextlen;
                nextlen = tree[(n + 1) * 2 + 1];
                if (++count < maxCount && curlen == nextlen) continue;

                if (count < minCount)
                {
                    do
                    {
                        Send_code(curlen, BlTree);
                    } while (--count != 0);
                }
                else if (curlen != 0)
                {
                    if (curlen != prevlen)
                    {
                        Send_code(curlen, BlTree);
                        count--;
                    }

                    Send_code(InternalConstants.REP_3_6, BlTree);
                    Send_bits(count - 3, 2);
                }
                else if (count <= 10)
                {
                    Send_code(InternalConstants.REPZ_3_10, BlTree);
                    Send_bits(count - 3, 3);
                }
                else
                {
                    Send_code(InternalConstants.REPZ_11_138, BlTree);
                    Send_bits(count - 11, 7);
                }

                count = 0;
                prevlen = curlen;
                if (nextlen == 0)
                {
                    maxCount = 138;
                    minCount = 3;
                }
                else if (curlen == nextlen)
                {
                    maxCount = 6;
                    minCount = 3;
                }
                else
                {
                    maxCount = 7;
                    minCount = 4;
                }
            }
        }

        private void Put_bytes(byte[] p, int start, int len)
        {
            Array.Copy(p, start, Pending, PendingCount, len);
            PendingCount += len;
        }

        internal void Send_code(int c, short[] tree)
        {
            var c2 = c * 2;
            Send_bits(tree[c2] & 0xffff, tree[c2 + 1] & 0xffff);
        }

        internal void Send_bits(int value, int length)
        {
            var len = length;
            unchecked
            {
                if (BiValid > BufSize - len)
                {
                    BiBuf |= (short) ((value << BiValid) & 0xffff);
                    Pending[PendingCount++] = (byte) BiBuf;
                    Pending[PendingCount++] = (byte) (BiBuf >> 8);

                    BiBuf = (short) ((uint) value >> (BufSize - BiValid));
                    BiValid += len - BufSize;
                }
                else
                {
                    BiBuf |= (short) ((value << BiValid) & 0xffff);
                    BiValid += len;
                }
            }
        }

        internal void Tr_align()
        {
            Send_bits(StaticTrees << 1, 3);
            Send_code(EndBlock, StaticTree.lengthAndLiteralsTreeCodes);

            Bi_flush();

            if (1 + LastEobLen + 10 - BiValid < 9)
            {
                Send_bits(StaticTrees << 1, 3);
                Send_code(EndBlock, StaticTree.lengthAndLiteralsTreeCodes);
                Bi_flush();
            }

            LastEobLen = 7;
        }

        internal bool Tr_tally(int dist, int lc)
        {
            Pending[DistanceOffset + LastLit * 2] = unchecked((byte) ((uint) dist >> 8));
            Pending[DistanceOffset + LastLit * 2 + 1] = unchecked((byte) dist);
            Pending[LengthOffset + LastLit] = unchecked((byte) lc);
            LastLit++;

            if (dist == 0)
            {
                DynLtree[lc * 2]++;
            }
            else
            {
                Matches++;
                dist--;
                DynLtree[(Tree.LengthCode[lc] + InternalConstants.LITERALS + 1) * 2]++;
                DynDtree[Tree.DistanceCode(dist) * 2]++;
            }

            if ((LastLit & 0x1fff) != 0 || (int) CompressionLevel <= 2)
                return LastLit == LitBufsize - 1 || LastLit == LitBufsize;

            var outLength = LastLit << 3;
            var inLength = Strstart - BlockStart;
            int dcode;
            for (dcode = 0; dcode < InternalConstants.D_CODES; dcode++)
                outLength = (int) (outLength + DynDtree[dcode * 2] * (5L + Tree.ExtraDistanceBits[dcode]));
            outLength >>= 3;
            if (Matches < LastLit / 2 && outLength < inLength / 2)
                return true;

            return LastLit == LitBufsize - 1 || LastLit == LitBufsize;
        }

        internal void send_compressed_block(short[] ltree, short[] dtree)
        {
            var lx = 0;

            if (LastLit != 0)
                do
                {
                    var ix = DistanceOffset + lx * 2;
                    var distance = ((Pending[ix] << 8) & 0xff00) |
                                   (Pending[ix + 1] & 0xff);
                    var lc = Pending[LengthOffset + lx] & 0xff;
                    lx++;

                    if (distance == 0)
                    {
                        Send_code(lc, ltree); 
                    }
                    else
                    {
                        int code = Tree.LengthCode[lc];

                        Send_code(code + InternalConstants.LITERALS + 1, ltree);
                        var extra = Tree.ExtraLengthBits[code];
                        if (extra != 0)
                        {
                            lc -= Tree.LengthBase[code];
                            Send_bits(lc, extra);
                        }

                        distance--; 
                        code = Tree.DistanceCode(distance);

                        Send_code(code, dtree);

                        extra = Tree.ExtraDistanceBits[code];
                        if (extra == 0) continue;

                        distance -= Tree.DistanceBase[code];
                        Send_bits(distance, extra);
                    }
                } while (lx < LastLit);

            Send_code(EndBlock, ltree);
            LastEobLen = ltree[EndBlock * 2 + 1];
        }

        internal void Set_data_type()
        {
            var n = 0;
            var asciiFreq = 0;
            var binFreq = 0;
            while (n < 7)
            {
                binFreq += DynLtree[n * 2];
                n++;
            }

            while (n < 128)
            {
                asciiFreq += DynLtree[n * 2];
                n++;
            }

            while (n < InternalConstants.LITERALS)
            {
                binFreq += DynLtree[n * 2];
                n++;
            }

            DataType = (sbyte) (binFreq > asciiFreq >> 2 ? ZBinary : ZAscii);
        }

        internal void Bi_flush()
        {
            if (BiValid == 16)
            {
                Pending[PendingCount++] = (byte) BiBuf;
                Pending[PendingCount++] = (byte) (BiBuf >> 8);
                BiBuf = 0;
                BiValid = 0;
            }
            else if (BiValid >= 8)
            {
                Pending[PendingCount++] = (byte) BiBuf;
                BiBuf >>= 8;
                BiValid -= 8;
            }
        }

        internal void Bi_windup()
        {
            if (BiValid > 8)
            {
                Pending[PendingCount++] = (byte) BiBuf;
                Pending[PendingCount++] = (byte) (BiBuf >> 8);
            }
            else if (BiValid > 0)
            {
                Pending[PendingCount++] = (byte) BiBuf;
            }

            BiBuf = 0;
            BiValid = 0;
        }

        internal void Copy_block(int buf, int len, bool header)
        {
            Bi_windup(); 
            LastEobLen = 8; 

            if (header)
                unchecked
                {
                    Pending[PendingCount++] = (byte) len;
                    Pending[PendingCount++] = (byte) (len >> 8);
                    Pending[PendingCount++] = (byte) ~len;
                    Pending[PendingCount++] = (byte) (~len >> 8);
                }

            Put_bytes(Window, buf, len);
        }

        internal void Flush_block_only(bool eof)
        {
            Tr_flush_block(BlockStart >= 0 ? BlockStart : -1, Strstart - BlockStart, eof);
            BlockStart = Strstart;
            Codec.Flush_pending();
        }

        internal BlockState DeflateNone(FlushType flush)
        {
            var maxBlockSize = 0xffff;

            if (maxBlockSize > Pending.Length - 5) maxBlockSize = Pending.Length - 5;

            while (true)
            {
                if (Lookahead <= 1)
                {
                    FillWindow();
                    if (Lookahead == 0 && flush == FlushType.None)
                        return BlockState.NeedMore;

                    if (Lookahead == 0)
                        break;
                }

                Strstart += Lookahead;
                Lookahead = 0;

                var maxStart = BlockStart + maxBlockSize;
                if (Strstart == 0 || Strstart >= maxStart)
                {
                    Lookahead = Strstart - maxStart;
                    Strstart = maxStart;

                    Flush_block_only(false);
                    if (Codec.AvailableBytesOut == 0)
                        return BlockState.NeedMore;
                }

                if (Strstart - BlockStart < WSize - MinLookahead) continue;

                Flush_block_only(false);
                if (Codec.AvailableBytesOut == 0)
                    return BlockState.NeedMore;
            }

            Flush_block_only(flush == FlushType.Finish);
            if (Codec.AvailableBytesOut == 0)
                return flush == FlushType.Finish ? BlockState.FinishStarted : BlockState.NeedMore;

            return flush == FlushType.Finish ? BlockState.FinishDone : BlockState.BlockDone;
        }

        internal void Tr_stored_block(int buf, int storedLen, bool eof)
        {
            Send_bits((StoredBlock << 1) + (eof ? 1 : 0), 3); 
            Copy_block(buf, storedLen, true); 
        }

        internal void Tr_flush_block(int buf, int storedLen, bool eof)
        {
            int optLenb, staticLenb;
            var maxBlindex = 0; 

            if (CompressionLevel > 0)
            {
                if (DataType == ZUnknown)
                    Set_data_type();

                TreeLiterals.Build_tree(this);
                TreeDistances.Build_tree(this);

                maxBlindex = Build_bl_tree();

                optLenb = (OptLen + 3 + 7) >> 3;
                staticLenb = (StaticLen + 3 + 7) >> 3;

                if (staticLenb <= optLenb)
                    optLenb = staticLenb;
            }
            else
            {
                optLenb = staticLenb = storedLen + 5; 
            }

            if (storedLen + 4 <= optLenb && buf != -1)
            {
                Tr_stored_block(buf, storedLen, eof);
            }
            else if (staticLenb == optLenb)
            {
                Send_bits((StaticTrees << 1) + (eof ? 1 : 0), 3);
                send_compressed_block(StaticTree.lengthAndLiteralsTreeCodes, StaticTree.distTreeCodes);
            }
            else
            {
                Send_bits((DynTrees << 1) + (eof ? 1 : 0), 3);
                Send_all_trees(TreeLiterals.MaxCode + 1, TreeDistances.MaxCode + 1, maxBlindex + 1);
                send_compressed_block(DynLtree, DynDtree);
            }

            InitializeBlocks();

            if (eof) Bi_windup();
        }

        private void FillWindow()
        {
            do
            {
                var more = WindowSize - Lookahead - Strstart;

                int n;
                switch (more)
                {
                    case 0 when Strstart == 0 && Lookahead == 0:
                        more = WSize;
                        break;
                    case -1:
                        more--;
                        break;
                    default:
                    {
                        if (Strstart >= WSize + WSize - MinLookahead)
                        {
                            Array.Copy(Window, WSize, Window, 0, WSize);
                            MatchStart -= WSize;
                            Strstart -= WSize; 
                            BlockStart -= WSize;

                            n = HashSize;
                            var p = n;
                            int m;
                            do
                            {
                                m = Head[--p] & 0xffff;
                                Head[p] = (short) (m >= WSize ? m - WSize : 0);
                            } while (--n != 0);

                            n = WSize;
                            p = n;
                            do
                            {
                                m = Prev[--p] & 0xffff;
                                Prev[p] = (short) (m >= WSize ? m - WSize : 0);
                            } while (--n != 0);

                            more += WSize;
                        }

                        break;
                    }
                }

                if (Codec.AvailableBytesIn == 0)
                    return;

                n = Codec.Read_buf(Window, Strstart + Lookahead, more);
                Lookahead += n;

                if (Lookahead < MinMatch) continue;

                InsH = Window[Strstart] & 0xff;
                InsH = ((InsH << HashShift) ^ (Window[Strstart + 1] & 0xff)) & HashMask;
            } while (Lookahead < MinLookahead && Codec.AvailableBytesIn != 0);
        }

        internal BlockState DeflateFast(FlushType flush)
        {
            var hashHead = 0; 

            while (true)
            {
                if (Lookahead < MinLookahead)
                {
                    FillWindow();
                    if (Lookahead < MinLookahead && flush == FlushType.None) return BlockState.NeedMore;

                    if (Lookahead == 0)
                        break; 
                }

                if (Lookahead >= MinMatch)
                {
                    InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;
                    hashHead = Head[InsH] & 0xffff;
                    Prev[Strstart & WMask] = Head[InsH];
                    Head[InsH] = unchecked((short) Strstart);
                }

                if (hashHead != 0L && ((Strstart - hashHead) & 0xffff) <= WSize - MinLookahead)
                    if (CompressionStrategy != CompressionStrategy.HuffmanOnly)
                        MatchLength = Longest_match(hashHead);
                bool bflush;
                if (MatchLength >= MinMatch)
                {
                    bflush = Tr_tally(Strstart - MatchStart, MatchLength - MinMatch);

                    Lookahead -= MatchLength;

                    if (MatchLength <= _config.MaxLazy && Lookahead >= MinMatch)
                    {
                        MatchLength--; 
                        do
                        {
                            Strstart++;

                            InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;
                            hashHead = Head[InsH] & 0xffff;
                            Prev[Strstart & WMask] = Head[InsH];
                            Head[InsH] = unchecked((short) Strstart);
                        } while (--MatchLength != 0);

                        Strstart++;
                    }
                    else
                    {
                        Strstart += MatchLength;
                        MatchLength = 0;
                        InsH = Window[Strstart] & 0xff;

                        InsH = ((InsH << HashShift) ^ (Window[Strstart + 1] & 0xff)) & HashMask;
                    }
                }
                else
                {
                    bflush = Tr_tally(0, Window[Strstart] & 0xff);
                    Lookahead--;
                    Strstart++;
                }

                if (!bflush) continue;

                Flush_block_only(false);
                if (Codec.AvailableBytesOut == 0)
                    return BlockState.NeedMore;
            }

            Flush_block_only(flush == FlushType.Finish);
            if (Codec.AvailableBytesOut != 0)
                return flush == FlushType.Finish ? BlockState.FinishDone : BlockState.BlockDone;

            return flush == FlushType.Finish ? BlockState.FinishStarted : BlockState.NeedMore;
        }

        internal BlockState DeflateSlow(FlushType flush)
        {
            var hashHead = 0;

            while (true)
            {
                if (Lookahead < MinLookahead)
                {
                    FillWindow();
                    if (Lookahead < MinLookahead && flush == FlushType.None)
                        return BlockState.NeedMore;

                    if (Lookahead == 0)
                        break; 
                }

                if (Lookahead >= MinMatch)
                {
                    InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;
                    hashHead = Head[InsH] & 0xffff;
                    Prev[Strstart & WMask] = Head[InsH];
                    Head[InsH] = unchecked((short) Strstart);
                }

                PrevLength = MatchLength;
                PrevMatch = MatchStart;
                MatchLength = MinMatch - 1;

                if (hashHead != 0 && PrevLength < _config.MaxLazy &&
                    ((Strstart - hashHead) & 0xffff) <= WSize - MinLookahead)
                {
                    if (CompressionStrategy != CompressionStrategy.HuffmanOnly) MatchLength = Longest_match(hashHead);

                    if (MatchLength <= 5 && (CompressionStrategy == CompressionStrategy.Filtered ||
                                              MatchLength == MinMatch && Strstart - MatchStart > 4096))
                        MatchLength = MinMatch - 1;
                }

                bool bflush;
                if (PrevLength >= MinMatch && MatchLength <= PrevLength)
                {
                    var maxInsert = Strstart + Lookahead - MinMatch;
                    bflush = Tr_tally(Strstart - 1 - PrevMatch, PrevLength - MinMatch);

                    Lookahead -= PrevLength - 1;
                    PrevLength -= 2;
                    do
                    {
                        if (++Strstart > maxInsert) continue;

                        InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;
                        hashHead = Head[InsH] & 0xffff;
                        Prev[Strstart & WMask] = Head[InsH];
                        Head[InsH] = unchecked((short) Strstart);
                    } while (--PrevLength != 0);

                    MatchAvailable = 0;
                    MatchLength = MinMatch - 1;
                    Strstart++;

                    if (!bflush) continue;

                    Flush_block_only(false);
                    if (Codec.AvailableBytesOut == 0)
                        return BlockState.NeedMore;
                }
                else if (MatchAvailable != 0)
                {
                    bflush = Tr_tally(0, Window[Strstart - 1] & 0xff);

                    if (bflush) Flush_block_only(false);
                    Strstart++;
                    Lookahead--;
                    if (Codec.AvailableBytesOut == 0)
                        return BlockState.NeedMore;
                }
                else
                {
                    MatchAvailable = 1;
                    Strstart++;
                    Lookahead--;
                }
            }

            if (MatchAvailable != 0)
            {
                Tr_tally(0, Window[Strstart - 1] & 0xff);
                MatchAvailable = 0;
            }

            Flush_block_only(flush == FlushType.Finish);

            if (Codec.AvailableBytesOut != 0)
                return flush == FlushType.Finish ? BlockState.FinishDone : BlockState.BlockDone;

            return flush == FlushType.Finish ? BlockState.FinishStarted : BlockState.NeedMore;
        }

        internal int Longest_match(int curMatch)
        {
            var chainLength = _config.MaxChainLength; 
            var scan = Strstart;
            var bestLen = PrevLength; 
            var limit = Strstart > WSize - MinLookahead ? Strstart - (WSize - MinLookahead) : 0;

            var niceLength = _config.NiceLength;

            var wmask = WMask;

            var strend = Strstart + MaxMatch;
            var scanEnd1 = Window[scan + bestLen - 1];
            var scanEnd = Window[scan + bestLen];

            if (PrevLength >= _config.GoodLength) chainLength >>= 2;

            if (niceLength > Lookahead)
                niceLength = Lookahead;

            do
            {
                var match = curMatch;

                if (Window[match + bestLen] != scanEnd ||
                    Window[match + bestLen - 1] != scanEnd1 ||
                    Window[match] != Window[scan] ||
                    Window[++match] != Window[scan + 1])
                    continue;

                scan += 2;
                match++;

                do
                {
                } while (Window[++scan] == Window[++match] &&
                         Window[++scan] == Window[++match] &&
                         Window[++scan] == Window[++match] &&
                         Window[++scan] == Window[++match] &&
                         Window[++scan] == Window[++match] &&
                         Window[++scan] == Window[++match] &&
                         Window[++scan] == Window[++match] &&
                         Window[++scan] == Window[++match] && scan < strend);

                var len = MaxMatch - (strend - scan);
                scan = strend - MaxMatch;

                if (len <= bestLen) continue;

                MatchStart = curMatch;
                bestLen = len;
                if (len >= niceLength)
                    break;

                scanEnd1 = Window[scan + bestLen - 1];
                scanEnd = Window[scan + bestLen];
            } while ((curMatch = Prev[curMatch & wmask] & 0xffff) > limit && --chainLength != 0);

            return bestLen <= Lookahead ? bestLen : Lookahead;
        }

        private bool _rfc1950BytesEmitted;
        internal bool WantRfc1950HeaderBytes { get; set; } = true;

        internal int Initialize(ZlibCodec codec, CompressionLevel level)
        {
            return Initialize(codec, level, ZlibConstants.WindowBitsMax);
        }

        internal int Initialize(ZlibCodec codec, CompressionLevel level, int bits)
        {
            return Initialize(codec, level, bits, MemLevelDefault, CompressionStrategy.Default);
        }

        internal int Initialize(ZlibCodec codec, CompressionLevel level, int bits,
            CompressionStrategy compressionStrategy)
        {
            return Initialize(codec, level, bits, MemLevelDefault, compressionStrategy);
        }

        internal int Initialize(ZlibCodec codec, CompressionLevel level, int windowBits, int memLevel,
            CompressionStrategy strategy)
        {
            Codec = codec;
            Codec.Message = null;

            if (windowBits < 9 || windowBits > 15)
                throw new ZlibException("windowBits must be in the range 9..15.");

            if (memLevel < 1 || memLevel > MemLevelMax)
                throw new ZlibException($"memLevel must be in the range 1.. {MemLevelMax}");

            Codec.Dstate = this;

            WBits = windowBits;
            WSize = 1 << WBits;
            WMask = WSize - 1;

            HashBits = memLevel + 7;
            HashSize = 1 << HashBits;
            HashMask = HashSize - 1;
            HashShift = (HashBits + MinMatch - 1) / MinMatch;

            Window = new byte[WSize * 2];
            Prev = new short[WSize];
            Head = new short[HashSize];

            LitBufsize = 1 << (memLevel + 6);

            Pending = new byte[LitBufsize * 4];
            DistanceOffset = LitBufsize;
            LengthOffset = (1 + 2) * LitBufsize;

            CompressionLevel = level;
            CompressionStrategy = strategy;

            Reset();
            return ZlibConstants.ZOk;
        }

        internal void Reset()
        {
            Codec.TotalBytesIn = Codec.TotalBytesOut = 0;
            Codec.Message = null;

            PendingCount = 0;
            NextPending = 0;

            _rfc1950BytesEmitted = false;

            Status = WantRfc1950HeaderBytes ? InitState : BusyState;
            Codec._Adler32 = Adler.Adler32(0, null, 0, 0);

            LastFlush = (int) FlushType.None;

            InitializeTreeData();
            _InitializeLazyMatch();
        }

        internal int End()
        {
            if (Status != InitState && Status != BusyState && Status != FinishState)
                return ZlibConstants.ZStreamError;

            Pending = null;
            Head = null;
            Prev = null;
            Window = null;
            return Status == BusyState ? ZlibConstants.ZDataError : ZlibConstants.ZOk;
        }

        private void SetDeflater()
        {
            switch (_config.Flavor)
            {
                case DeflateFlavor.Store:
                    _deflateFunction = DeflateNone;
                    break;

                case DeflateFlavor.Fast:
                    _deflateFunction = DeflateFast;
                    break;

                case DeflateFlavor.Slow:
                    _deflateFunction = DeflateSlow;
                    break;
            }
        }

        internal int SetParams(CompressionLevel level, CompressionStrategy strategy)
        {
            var result = ZlibConstants.ZOk;

            if (CompressionLevel != level)
            {
                var newConfig = Config.Lookup(level);

                if (newConfig.Flavor != _config.Flavor && Codec.TotalBytesIn != 0)
                    result = Codec.Deflate(FlushType.Partial);

                CompressionLevel = level;
                _config = newConfig;
                SetDeflater();
            }

            CompressionStrategy = strategy;

            return result;
        }

        internal int SetDictionary(byte[] dictionary)
        {
            var length = dictionary.Length;
            var index = 0;

            if (dictionary == null || Status != InitState)
                throw new ZlibException("Stream error.");

            Codec._Adler32 = Adler.Adler32(Codec._Adler32, dictionary, 0, dictionary.Length);

            if (length < MinMatch)
                return ZlibConstants.ZOk;

            if (length > WSize - MinLookahead)
            {
                length = WSize - MinLookahead;
                index = dictionary.Length - length; 
            }

            Array.Copy(dictionary, index, Window, 0, length);
            Strstart = length;
            BlockStart = length;

            InsH = Window[0] & 0xff;
            InsH = ((InsH << HashShift) ^ (Window[1] & 0xff)) & HashMask;

            for (var n = 0; n <= length - MinMatch; n++)
            {
                InsH = ((InsH << HashShift) ^ (Window[n + (MinMatch - 1)] & 0xff)) & HashMask;
                Prev[n & WMask] = Head[InsH];
                Head[InsH] = (short) n;
            }

            return ZlibConstants.ZOk;
        }

        internal int Deflate(FlushType flush)
        {
            if (Codec.OutputBuffer == null ||
                Codec.InputBuffer == null && Codec.AvailableBytesIn != 0 ||
                Status == FinishState && flush != FlushType.Finish)
            {
                Codec.Message = ErrorMessage[ZlibConstants.ZNeedDict - ZlibConstants.ZStreamError];
                throw new ZlibException($"Something is fishy. [{Codec.Message}]");
            }

            if (Codec.AvailableBytesOut == 0)
            {
                Codec.Message = ErrorMessage[ZlibConstants.ZNeedDict - ZlibConstants.ZBufError];
                throw new ZlibException("OutputBuffer is full (AvailableBytesOut == 0)");
            }

            var oldFlush = LastFlush;
            LastFlush = (int) flush;

            if (Status == InitState)
            {
                var header = (ZDeflated + ((WBits - 8) << 4)) << 8;
                var levelFlags = (((int) CompressionLevel - 1) & 0xff) >> 1;

                if (levelFlags > 3)
                    levelFlags = 3;
                header |= levelFlags << 6;
                if (Strstart != 0)
                    header |= PresetDict;
                header += 31 - header % 31;

                Status = BusyState;
                unchecked
                {
                    Pending[PendingCount++] = (byte) (header >> 8);
                    Pending[PendingCount++] = (byte) header;
                }

                if (Strstart != 0)
                {
                    Pending[PendingCount++] = (byte) ((Codec._Adler32 & 0xFF000000) >> 24);
                    Pending[PendingCount++] = (byte) ((Codec._Adler32 & 0x00FF0000) >> 16);
                    Pending[PendingCount++] = (byte) ((Codec._Adler32 & 0x0000FF00) >> 8);
                    Pending[PendingCount++] = (byte) (Codec._Adler32 & 0x000000FF);
                }

                Codec._Adler32 = Adler.Adler32(0, null, 0, 0);
            }

            if (PendingCount != 0)
            {
                Codec.Flush_pending();
                if (Codec.AvailableBytesOut == 0)
                {
                    LastFlush = -1;
                    return ZlibConstants.ZOk;
                }
            }
            else if (Codec.AvailableBytesIn == 0 &&
                     (int) flush <= oldFlush &&
                     flush != FlushType.Finish)
            {
                return ZlibConstants.ZOk;
            }

            if (Status == FinishState && Codec.AvailableBytesIn != 0)
            {
                Codec.Message = ErrorMessage[ZlibConstants.ZNeedDict - ZlibConstants.ZBufError];
                throw new ZlibException("status == FINISH_STATE && _codec.AvailableBytesIn != 0");
            }

            if (Codec.AvailableBytesIn != 0 || Lookahead != 0 || flush != FlushType.None && Status != FinishState)
            {
                var bstate = _deflateFunction(flush);

                if (bstate == BlockState.FinishStarted || bstate == BlockState.FinishDone) Status = FinishState;
                switch (bstate)
                {
                    case BlockState.NeedMore:
                    case BlockState.FinishStarted:
                    {
                        if (Codec.AvailableBytesOut == 0) LastFlush = -1; 
                        return ZlibConstants.ZOk;
                    }

                    case BlockState.BlockDone:
                    {
                        if (flush == FlushType.Partial)
                        {
                            Tr_align();
                        }
                        else
                        {
                            Tr_stored_block(0, 0, false);
                            if (flush == FlushType.Full)
                                for (var i = 0; i < HashSize; i++)
                                    Head[i] = 0;
                        }

                        Codec.Flush_pending();
                        if (Codec.AvailableBytesOut == 0)
                        {
                            LastFlush = -1; 
                            return ZlibConstants.ZOk;
                        }

                        break;
                    }
                }
            }

            if (flush != FlushType.Finish)
                return ZlibConstants.ZOk;

            if (!WantRfc1950HeaderBytes || _rfc1950BytesEmitted)
                return ZlibConstants.ZStreamEnd;

            Pending[PendingCount++] = (byte) ((Codec._Adler32 & 0xFF000000) >> 24);
            Pending[PendingCount++] = (byte) ((Codec._Adler32 & 0x00FF0000) >> 16);
            Pending[PendingCount++] = (byte) ((Codec._Adler32 & 0x0000FF00) >> 8);
            Pending[PendingCount++] = (byte) (Codec._Adler32 & 0x000000FF);

            Codec.Flush_pending();

            _rfc1950BytesEmitted = true; 

            return PendingCount != 0 ? ZlibConstants.ZOk : ZlibConstants.ZStreamEnd;
        }
    }
}