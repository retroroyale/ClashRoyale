using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceCreateFailedMessag : PiranhaMessage
    {
        public AllianceCreateFailedMessag(Device device) : base(device)
        {
            Id = 24332;
        }

        public int Reason { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(Reason);
        }
    }
}