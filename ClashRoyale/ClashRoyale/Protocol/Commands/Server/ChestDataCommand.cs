using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Chests.Items;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class ChestDataCommand : LogicCommand
    {
        public ChestDataCommand(Device device) : base(device)
        {
            Type = 210;
        }

        public ChestDataCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Type = 210;
        }

        public Chest Chest { get; set; }

        public override void Encode()
        {
            Chest.Encode(Data);
        }
    }
}