using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Chests.Items;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;
using ClashRoyale.Protocol.Commands.Server;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicCollectCrownChestCommand : LogicCommand
    {
        public LogicCollectCrownChestCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override async void Process()
        {
            await new AvailableServerCommand(Device)
            {
                Command = new ChestDataCommand(Device)
                {
                    Chest = Device.Player.Home.Chests.BuyChest(1, Chest.ChestType.Crown)
                }
            }.Send();

            Device.Player.Home.Crowns -= 10;
        }
    }
}