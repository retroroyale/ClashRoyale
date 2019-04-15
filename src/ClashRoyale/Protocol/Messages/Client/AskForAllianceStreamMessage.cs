using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForAllianceStreamMessage : PiranhaMessage
    {
        public AskForAllianceStreamMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14304;
        }

        public override async void Process()
        {
            var alliance = await Resources.Alliances.GetAllianceAsync(Device.Player.Home.AllianceInfo.Id);

            if (alliance != null)
                await new AllianceStreamMessage(Device)
                {
                    Entries = alliance.Stream
                }.SendAsync();
        }
    }
}