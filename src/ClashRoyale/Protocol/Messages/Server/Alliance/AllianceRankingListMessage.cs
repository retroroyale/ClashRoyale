using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

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
            Writer.WriteVInt(0);
        }
    }
}