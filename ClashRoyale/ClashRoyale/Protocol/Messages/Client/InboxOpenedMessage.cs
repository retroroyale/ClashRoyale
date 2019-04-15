using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class InboxOpenedMessage : PiranhaMessage
    {
        public InboxOpenedMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10905;
        }

        public override async void Process()
        {
            await new InboxListMessage(Device).SendAsync();
        }
    }
}