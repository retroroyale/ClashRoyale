using System;

namespace ClashRoyale.Utilities.Utils
{
    public class GameUtils
    {
        public static string GenerateToken
        {
            get
            {
                var random = new Random();
                var token = string.Empty;

                for (var i = 0; i < 40; i++)
                    token += "abcdefghijklmnopqrstuvwxyz0123456789"[random.Next(36)];

                return token;
            }
        }

        public static bool IsHigherRoleThan(int role, int roleToCompare)
        {
            var table = new[] {1, 1, 4, 2, 3};
            return role >= 5 || roleToCompare >= 5 || table[roleToCompare] < table[role];
        }

        // TODO
        public static int Id(int classId, int instanceId)
        {
            if (classId >= 27) instanceId += 58;
            if (classId == 28) instanceId += 14;

            return instanceId + 1;
        }
    }
}