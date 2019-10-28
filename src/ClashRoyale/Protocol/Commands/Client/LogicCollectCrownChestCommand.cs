using System;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Chests.Items;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicCollectCrownChestCommand : LogicCommand
    {
        public LogicCollectCrownChestCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override async void Process()
        {
            var home = Device.Player.Home;

            await new AvailableServerCommand(Device)
            {
                Command = new ChestDataCommand(Device)
                {
                    Chest = home.Chests.BuyChest(1, Chest.ChestType.Crown)
                }
            }.SendAsync();

            home.Crowns -= Csv.Tables.Get(Csv.Files.Globals).GetData<Globals>("CROWN_CHEST_CROWN_COUNT").NumberValue;
            home.CrownChestCooldown = DateTime.UtcNow.AddHours(Csv.Tables.Get(Csv.Files.Globals).GetData<Globals>("CROWN_CHEST_COOLDOWN_HOURS").NumberValue);
        }
    }
}