using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class CancelMatchmakeDoneMessage : PiranhaMessage
    {
        public CancelMatchmakeDoneMessage(Device device) : base(device)
        {
            Id = 24125;
        }
    }
}