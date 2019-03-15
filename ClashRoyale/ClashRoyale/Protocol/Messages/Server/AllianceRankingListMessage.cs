using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceRankingListMessage : PiranhaMessage
    {
        public AllianceRankingListMessage(Device device) : base(device)
        {
            Id = 24401;
        }

        public override void Encode()
        {
            Packet.WriteVInt(0);
        }
    }
}