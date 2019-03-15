using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForAllianceRankingListMessage : PiranhaMessage
    {
        public AskForAllianceRankingListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14401;
        }

        public override async void Process()
        {
            await new AllianceRankingListMessage(Device).Send();
        }
    }
}