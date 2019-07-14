using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class StopHomeLogicMessage : PiranhaMessage
    {
        public StopHomeLogicMessage(Device device) : base(device)
        {
            Id = 24106;
        }
    }
}