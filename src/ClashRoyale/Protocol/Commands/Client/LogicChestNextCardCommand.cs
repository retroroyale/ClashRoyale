using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicChestNextCardCommand : LogicCommand
    {
        public LogicChestNextCardCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override void Decode()
        {
            base.Decode();

            Buffer.ReadVInt();
            Buffer.ReadVInt();
        }
    }
}