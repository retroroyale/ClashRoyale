using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class CancelMatchmakeMessage : PiranhaMessage
    {
        public CancelMatchmakeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14107;
        }

        public override async void Process()
        {
            if (Resources.Battles.Cancel(Device.Player))
                await new CancelMatchmakeDoneMessage(Device).SendAsync();
        }
    }
}