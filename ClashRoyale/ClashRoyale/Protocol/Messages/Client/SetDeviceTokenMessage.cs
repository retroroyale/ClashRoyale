using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class SetDeviceTokenMessage : PiranhaMessage
    {
        public SetDeviceTokenMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10113;
        }
    }
}