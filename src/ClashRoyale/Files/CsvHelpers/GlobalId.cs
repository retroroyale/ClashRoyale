namespace ClashRoyale.Files.CsvHelpers
{
    public static class GlobalId
    {
        private const int kReference = 1125899907;

        public static int CreateGlobalId(int classId, int instanceId)
        {
            return classId * 1000000 + instanceId;
        }

        public static int GetClassId(int globalId)
        {
            var type = (int) ((kReference * (long) globalId) >> 32);
            return (type >> 18) + (type >> 31);
        }

        public static int GetInstanceId(int globalId)
        {
            var referenceT = (int) ((kReference * (long) globalId) >> 32);
            return globalId - 1000000 * ((referenceT >> 18) + (referenceT >> 31));
        }
    }
}