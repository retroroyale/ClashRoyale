using System;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicBuyResourcePackCommand : LogicCommand
    {
        public LogicBuyResourcePackCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override void Decode()
        {
            Console.WriteLine(Buffer.ReadVInt());
            Console.WriteLine(Buffer.ReadVInt());
            Console.WriteLine(Buffer.ReadVInt());
        }

        public override void Process()
        {
        }
    }
}