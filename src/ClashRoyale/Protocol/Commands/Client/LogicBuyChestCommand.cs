using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Chests.Items;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicBuyChestCommand : LogicCommand
    {
        public LogicBuyChestCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int InstanceId { get; set; }

        public override void Decode()
        {
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            Reader.ReadVInt(); // ClassId
            InstanceId = Reader.ReadVInt();
        }

        public override async void Process()
        {
            var chest = Device.Player.Home.Chests.BuyChest(InstanceId, Chest.ChestType.Shop);

            await new AvailableServerCommand(Device)
            {
                Command = new ChestDataCommand(Device)
                {
                    Chest = chest
                }
            }.SendAsync();
        }
    }
}