using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForAvatarStreamMessage : PiranhaMessage
    {
        public AskForAvatarStreamMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14405;
        }
    }
}