using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class DisconnectedMessage : PiranhaMessage
    {
        public DisconnectedMessage(Device device) : base(device)
        {
            Id = 25892;
        }

        public override void Encode()
        {
            Writer.WriteVInt(1);
        }
    }
}