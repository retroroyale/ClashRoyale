using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class OpponentLeftMatchNotificationMessage : PiranhaMessage
    {
        public OpponentLeftMatchNotificationMessage(Device device) : base(device)
        {
            Id = 20801;
        }
    }
}