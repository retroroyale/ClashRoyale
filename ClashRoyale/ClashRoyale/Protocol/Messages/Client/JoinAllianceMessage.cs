using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class JoinAllianceMessage : PiranhaMessage
    {
        public JoinAllianceMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14305;
        }

        public long AllianceId { get; set; }

        public override void Decode()
        {
            AllianceId = Reader.ReadLong();
        }

        public override async void Process()
        {
            var alliance = await Resources.Alliances.GetAlliance(AllianceId);
            var home = Device.Player.Home;

            if (alliance != null)
            {
                if (alliance.Members.Count <= 0 || alliance.Members.Count >= 50)
                {
                    await new AllianceJoinFailedMessage(Device).Send();
                }
                else
                {
                    alliance.Add(new AllianceMember(Device.Player, Alliance.Role.Member));

                    home.AllianceInfo = alliance.GetAllianceInfo(home.Id);

                    await new AvailableServerCommand(Device)
                    {
                        Command = new LogicJoinAllianceCommand(Device)
                        {
                            AllianceId = alliance.Id,
                            AllianceName = alliance.Name,
                            AllianceBadge = alliance.Badge
                        }
                    }.Send();

                    await new AllianceStreamMessage(Device)
                    {
                        Entries = alliance.Stream
                    }.Send();

                    var entry = new AllianceEventStreamEntry
                    {
                        EventType = AllianceEventStreamEntry.Type.Join
                    };

                    entry.SetTarget(Device.Player);
                    entry.SetSender(Device.Player);
                    alliance.AddEntry(entry);

                    alliance.Save();
                    Device.Player.Save();

                    alliance.UpdateOnlineCount();
                }
            }
        }
    }
}