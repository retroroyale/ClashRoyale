using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

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
            Writer.WriteVInt(Count);
            Writer.WriteByte(0);
        }
    }
}