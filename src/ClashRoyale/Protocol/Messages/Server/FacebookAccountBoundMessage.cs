using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class FacebookAccountBoundMessage : PiranhaMessage
    {
        public FacebookAccountBoundMessage(Device device) : base(device)
        {
            Id = 24201;
        }

        public int ResultCode { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(ResultCode);
        }
    }
}