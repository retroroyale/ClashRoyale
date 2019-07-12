using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class KeepAliveOkMessage : PiranhaMessage
    {
        public KeepAliveOkMessage(Device device) : base(device)
        {
            Id = 20108;
        }
    }
}