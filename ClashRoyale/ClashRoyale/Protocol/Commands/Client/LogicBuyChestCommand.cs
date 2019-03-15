using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
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
            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt(); // ClassId
            InstanceId = Buffer.ReadVInt();
        }

        public override async void Process()
        {
            /*var chest = Device.Player.Home.Chests.BuyChest(InstanceId, Chest.ChestType.Shop);

            await new AvailableServerCommand(Device)
            {
                Command = new ChestDataCommand(Device)
                {
                    Chest = chest
                }
            }.Send();*/

            await new OutOfSyncMessage(Device).Send();
        }
    }
}