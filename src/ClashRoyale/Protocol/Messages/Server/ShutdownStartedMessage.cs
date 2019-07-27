using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class ShutdownStartedMessage : PiranhaMessage
    {
        public ShutdownStartedMessage(Device device) : base(device)
        {
            Id = 20161;
        }

        public override void Encode()
        {
            Writer.WriteVInt(0);
        }
    }
}