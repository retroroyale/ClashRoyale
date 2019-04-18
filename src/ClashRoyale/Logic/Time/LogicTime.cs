using ClashRoyale.Extensions;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Time
{
    public class LogicTime
    {
        [JsonIgnore] public int ServerTick { get; set; }
        [JsonIgnore] public int ClientTick { get; set; }

        public bool IsClientOffSync => ServerTick + 60 < ClientTick;

        public void IncreaseTick()
        {
            ++ClientTick;
            ++ServerTick;
        }

        public void SetServerTick(int tick)
        {
            ClientTick = tick;
            ServerTick = tick;
        }

        public void Update(float time)
        {
            ClientTick += (int) time * 20;
            ServerTick += (int) time * 20;
        }

        public void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(ClientTick);
        }

        public static int GetSecondsInTicks(int seconds)
        {
            return seconds * 20;
        }
    }
}