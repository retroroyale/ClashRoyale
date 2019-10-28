using System;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Time
{
    public class LogicTimer
    {
        [JsonProperty("remainingTicks")] public int RemainingTicks { get; set; }
        [JsonProperty("totalTicks")] public int TotalTicks { get; set; }
        [JsonProperty("endTimestamp")] public int EndTimestamp { get; set; }

        [JsonIgnore] public bool IsFinished => RemainingTicks <= 0;
        [JsonIgnore] public int RemainingSeconds => RemainingTicks <= 0 ? 0 : Math.Max((RemainingTicks + 19) / 20, 1);

        public void StartTimer(int seconds)
        {
            TotalTicks = LogicTime.GetSecondsInTicks(seconds);
            RemainingTicks = TotalTicks;
            EndTimestamp = TimeUtils.CurrentUnixTimestamp + seconds;
        }

        public void Decode(IByteBuffer packet)
        {
            RemainingTicks = packet.ReadVInt();
            TotalTicks = packet.ReadVInt();
            EndTimestamp = packet.ReadVInt();
        }

        public void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(RemainingTicks);
            packet.WriteVInt(TotalTicks);
            packet.WriteVInt(EndTimestamp);
        }

        public void Tick()
        {
            if (RemainingTicks > 0) RemainingTicks--;
        }

        public void AdjustEndSubTick(int ticks)
        {
            RemainingTicks = RemainingTicks - ticks <= 0 ? 0 : -ticks;
        }

        public void FastForward(int seconds)
        {
            if (RemainingTicks > 0) AdjustEndSubTick(LogicTime.GetSecondsInTicks(seconds));
        }
    }
}