using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarLocalRankingListMessage : PiranhaMessage
    {
        public AvatarLocalRankingListMessage(Device device) : base(device)
        {
            Id = 24404;
        }

        public override void Encode()
        {
            Packet.WriteVInt(0);
        }
    }
}