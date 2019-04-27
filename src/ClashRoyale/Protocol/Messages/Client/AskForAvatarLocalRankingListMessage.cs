using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForAvatarLocalRankingListMessage : PiranhaMessage
    {
        public AskForAvatarLocalRankingListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14404;
        }

        public override async void Process()
        {
            await new AvatarLocalRankingListMessage(Device).SendAsync();
        }
    }
}