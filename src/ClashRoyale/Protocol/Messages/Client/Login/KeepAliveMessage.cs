using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Login
{
    public class KeepAliveMessage : PiranhaMessage
    {
        public KeepAliveMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10108;
            RequiredState = Device.State.NotDefinied;
        }

        public override async void Process()
        {
            await new KeepAliveOkMessage(Device).SendAsync();
        }
    }
}