using ClashRoyale.Extensions;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarRankingListMessage : PiranhaMessage
    {
        public AvatarRankingListMessage(Device device) : base(device)
        {
            Id = 24403;
        }

        public override void Encode()
        {
            Packet.WriteVInt(0);

            Packet.WriteInt(0);
            Packet.WriteInt(TimeUtils.GetSecondsUntilNextMonth);
        }
    }
}