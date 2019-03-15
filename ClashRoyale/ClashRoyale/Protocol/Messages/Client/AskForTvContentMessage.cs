using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForTvContentMessage : PiranhaMessage
    {
        public AskForTvContentMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14402;
        }
    }
}