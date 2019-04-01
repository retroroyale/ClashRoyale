using System;
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
            var clan = await Resources.Alliances.GetAlliance(AllianceId);
            var home = Device.Player.Home;

            if (clan != null)
            {
                if (clan.Members.Count <= 0 || clan.Members.Count >= 50)
                {
                    await new AllianceJoinFailedMessage(Device).Send();
                }
                else
                {
                    clan.Add(new AllianceMember(Device.Player, Alliance.Role.Member));

                    home.AllianceInfo = clan.GetAllianceInfo(home.Id);

                    await new AvailableServerCommand(Device)
                    {
                        Command = new LogicJoinAllianceCommand(Device)
                        {
                            AllianceId = clan.Id,
                            AllianceName = clan.Name,
                            AllianceBadge = clan.Badge
                        }
                    }.Send();

                    await new AllianceStreamMessage(Device)
                    {
                        Entries = clan.Stream
                    }.Send();

                    var entry = new AllianceEventStreamEntry
                    {
                        CreationDateTime = DateTime.UtcNow,
                        Id = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        EventType = AllianceEventStreamEntry.Type.Join
                    };

                    entry.SetTarget(Device.Player);
                    entry.SetSender(Device.Player);
                    clan.AddEntry(entry);

                    clan.Save();
                    Device.Player.Save();

                    clan.UpdateOnlineCount();
                }
            }
        }
    }
}