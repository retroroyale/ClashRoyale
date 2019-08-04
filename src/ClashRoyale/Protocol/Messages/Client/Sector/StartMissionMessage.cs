using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Sector
{
    public class StartMissionMessage : PiranhaMessage
    {
        public StartMissionMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14104;
            RequiredState = Device.State.Home;
        }

        public override async void Process()
        {
            await new NpcSectorStateMessage(Device).SendAsync();
        }
    }
}