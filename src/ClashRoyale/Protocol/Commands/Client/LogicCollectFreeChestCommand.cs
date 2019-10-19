using System;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Chests.Items;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicCollectFreeChestCommand : LogicCommand
    {
        public LogicCollectFreeChestCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override async void Process()
        {
            var home = Device.Player.Home;

            if (!home.IsSecondFreeChestAvailable())
            {
                home.FreeChestTime = home.FreeChestTime.AddHours(4);
            }
            else if (home.IsFirstFreeChestAvailable())
            {
                home.FreeChestTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4));
            }
            else
            {
                Device.Disconnect();
                return;
            }

            await new AvailableServerCommand(Device)
            {
                Command = new ChestDataCommand(Device)
                {
                    Chest = Device.Player.Home.Chests.BuyChest(1, Chest.ChestType.Free)
                }
            }.SendAsync();
        }
    }
}