using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class OpponentRejoinsMatchNotificationMessage : PiranhaMessage
    {
        public OpponentRejoinsMatchNotificationMessage(Device device) : base(device)
        {
            Id = 20802;
        }
    }
}