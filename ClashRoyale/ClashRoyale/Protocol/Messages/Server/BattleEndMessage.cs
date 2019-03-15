using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class BattleEndMessage : PiranhaMessage
    {
        public BattleEndMessage(Device device) : base(device)
        {
            Id = 20225;
        }

        public override void Encode()
        {
            Packet.WriteHex("011f001f003f0000030013a1030000042fac138d140bac133acd1501");
        }
    }
}