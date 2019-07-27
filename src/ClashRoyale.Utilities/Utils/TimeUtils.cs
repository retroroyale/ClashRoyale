using System;

namespace ClashRoyale.Utilities.Utils
{
    public class TimeUtils
    {
        public static int GetSecondsUntilNextMonth
        {
            get
            {
                var now = DateTime.UtcNow;

                if (now.Month != 12)
                    return (int) (new DateTime(now.Year, now.Month + 1, 1, now.Hour,
                                      now.Minute, now.Second) - now).TotalSeconds;

                return (int) (new DateTime(now.Year + 1, 1, 1, now.Hour,
                                  now.Minute, now.Second) - now).TotalSeconds;
            }
        }

        public static int GetSecondsUntilTomorrow
        {
            get
            {
                var now = DateTime.UtcNow;
                var tomorrow = now.AddDays(1).Date;

                return (int) (tomorrow - now).TotalSeconds;
            }
        }

        public static int CurrentUnixTimestamp => (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}