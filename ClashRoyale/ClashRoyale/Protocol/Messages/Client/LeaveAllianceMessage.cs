using System;
using ClashRoyale.Database;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class LeaveAllianceMessage : PiranhaMessage
    {
        public LeaveAllianceMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14308;
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var clan = await Resources.Alliances.GetAlliance(home.AllianceInfo.Id);         

            if (clan != null)
            {
                clan.Remove(home.Id);
                home.AllianceInfo.Reset();

                await new AvailableServerCommand(Device)
                {
                    Command = new LogicLeaveAllianceCommand(Device)
                    {
                        AllianceId = clan.Id
                    }
                }.Send();

                if (clan.Members.Count == 0)
                {
                    await AllianceDb.Delete(clan.Id);
                    await Redis.UncacheAlliance(clan.Id);
                    return;
                }

                var entry = new AllianceEventStreamEntry
                {
                    CreationDateTime = DateTime.UtcNow,
                    Id = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                    EventType = AllianceEventStreamEntry.Type.Leave
                };

                entry.SetTarget(Device.Player);
                entry.SetSender(Device.Player);
                clan.AddEntry(entry);

                clan.UpdateOnlineCount();
            }
        }
    }
}