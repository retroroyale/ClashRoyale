using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using System;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicChestNextCardCommand : LogicCommand
    {
        public LogicChestNextCardCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override void Decode()
        {
            Buffer.ReadVInt();
            Buffer.ReadVInt();
        }
    }
}