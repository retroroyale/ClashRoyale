using System;

namespace ClashRoyale.Core.Network
{
    public class Throttler
    {
        private readonly int _interval;
        private readonly int _maxPackets;
        private int _count;
        private DateTime _lastReset = DateTime.UtcNow;

        /// <summary>
        ///     Throttle packets by checking if we can process it or if the ratelimit is reached
        /// </summary>
        /// <param name="maxPackets"></param>
        /// <param name="interval"></param>
        public Throttler(int maxPackets, int interval)
        {
            _maxPackets = maxPackets;
            _interval = interval;
        }

        public bool CanProcess()
        {
            if (_count++ < _maxPackets) return true;
            if (GetMillisecondsSinceLastReset() < _interval) return false;

            _count = 1;
            _lastReset = DateTime.UtcNow;
            return true;
        }

        private int GetMillisecondsSinceLastReset()
        {
            return (int) DateTime.UtcNow.Subtract(_lastReset).TotalMilliseconds;
        }
    }
}