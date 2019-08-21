namespace ClashRoyale.Utilities.Compression.ZLib
{
    public static class ZlibConstants
    {
        public const int WindowBitsMax = 15; 
        public const int WindowBitsDefault = WindowBitsMax;

        public const int ZOk = 0;
        public const int ZStreamEnd = 1;
        public const int ZNeedDict = 2;
        public const int ZStreamError = -2;
        public const int ZDataError = -3;
        public const int ZBufError = -5;

        public const int WorkingBufferSizeDefault = 16384;

        public const int WorkingBufferSizeMin = 1024;
    }
}