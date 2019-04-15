using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForAvatarRankingListMessage : PiranhaMessage
    {
        public AskForAvatarRankingListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14403;
        }

        public override async void Process()
        {
            await new AvatarRankingListMessage(Device).SendAsync();
        }
    }
}