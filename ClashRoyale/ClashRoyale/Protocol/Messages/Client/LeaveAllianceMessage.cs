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
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

            if (alliance != null)
            {
                alliance.Remove(home.Id);
                home.AllianceInfo.Reset();
                Device.Player.Save();

                await new AvailableServerCommand(Device)
                {
                    Command = new LogicLeaveAllianceCommand(Device)
                    {
                        AllianceId = alliance.Id
                    }
                }.SendAsync();

                if (alliance.Members.Count != 0)
                {
                    var entry = new AllianceEventStreamEntry
                    {
                        EventType = AllianceEventStreamEntry.Type.Leave
                    };

                    entry.SetTarget(Device.Player);
                    entry.SetSender(Device.Player);
                    alliance.AddEntry(entry);

                    alliance.Save();
                    alliance.UpdateOnlineCount();
                }
                else
                {
                    await AllianceDb.DeleteAsync(alliance.Id);
                    await Redis.UncacheAllianceAsync(alliance.Id);
                }
            }
        }
    }
}