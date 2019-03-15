using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceListMessage : PiranhaMessage
    {
        public AllianceListMessage(Device device) : base(device)
        {
            Id = 24304;
        }

        public override void Encode()
        {
            Packet.WriteVInt(0);
        }
    }
}