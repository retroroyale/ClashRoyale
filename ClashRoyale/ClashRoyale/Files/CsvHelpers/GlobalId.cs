namespace ClashRoyale.Files.CsvHelpers
{
    public static class GlobalId
    {
        private const int Reference = 1125899907;

        public static int CreateGlobalId(int index, int count)
        {
            return count + 1000000 * index;
        }

        public static int GetClassId(int type)
        {
            type = (int) ((Reference * (long) type) >> 32);
            return (type >> 18) + (type >> 31);
        }

        public static int GetInstanceId(int globalId)
        {
            var referenceT = (int) ((Reference * (long) globalId) >> 32);
            return globalId - 1000000 * ((referenceT >> 18) + (referenceT >> 31));
        }
    }
}