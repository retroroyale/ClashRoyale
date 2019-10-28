using System;
using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class UnknownCommand : LogicCommand
    {
        public UnknownCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override void Decode()
        {
            base.Decode();

            Console.WriteLine(Reader.ReadVInt()); // 0
            Console.WriteLine(Reader.ReadVInt()); // 7
            Console.WriteLine(Reader.ReadVInt()); // 0
            Console.WriteLine(Reader.ReadVInt()); // 1
        }
    }
}