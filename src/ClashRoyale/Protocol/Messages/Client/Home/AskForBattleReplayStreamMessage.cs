using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Home
{
    public class AskForBattleReplayStreamMessage : PiranhaMessage
    {
        public AskForBattleReplayStreamMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14406;
        }
    }
}