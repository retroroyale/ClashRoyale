using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class ServerErrorMessage : PiranhaMessage
    {
        public ServerErrorMessage(Device device) : base(device)
        {
            Id = 24115;
        }

        public string Message { get; set; }

        public override void Encode()
        {
            Writer.WriteScString(Message);
        }
    }
}