using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceOnlineStatusUpdatedMessage : PiranhaMessage
    {
        public AllianceOnlineStatusUpdatedMessage(Device device) : base(device)
        {
            Id = 20207;
        }

        public int Count { get; set; }

        public override void Encode()
        {
            Packet.WriteVInt(Count);
            Packet.WriteByte(0);
        }
    }
}