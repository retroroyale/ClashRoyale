namespace ClashRoyale.Files.CsvHelpers
{
    public static class GlobalId
    {
        public static int CreateGlobalId(int classId, int instanceId)
        {
            return classId <= 0 ? 1000000 + instanceId : classId * 1000000 + instanceId;
        }

        public static int GetClassId(int globalId)
        {
            return globalId / 1000000;
        }

        public static int GetInstanceId(int globalId)
        {
            return globalId % 1000000;
        }
    }
}