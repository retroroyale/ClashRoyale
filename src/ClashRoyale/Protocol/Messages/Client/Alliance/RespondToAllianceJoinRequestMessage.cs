using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class RespondToAllianceJoinRequestMessage : PiranhaMessage
    {
        public RespondToAllianceJoinRequestMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14321;
        }

        public long StreamId { get; set; }
        public bool Accepted { get; set; }

        public override void Decode()
        {
            StreamId = Reader.ReadLong();
            Accepted = Reader.ReadBoolean();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

            var entry = alliance?.Stream.Find(e => e.Id == StreamId);
            if (entry == null) return;

            alliance.RemoveEntry(entry);

            var newEntry = (JoinRequestAllianceStreamEntry) entry;
            newEntry.State = Accepted ? 2 : 0;

            newEntry.SetTarget(Device.Player);

            alliance.AddEntry(newEntry);

            if (Accepted)
            {
                var player = await Resources.Players.GetPlayerAsync(newEntry.SenderId);
                alliance.Add(new AllianceMember(player, Logic.Clan.Alliance.Role.Member));

                player.Home.AllianceInfo = alliance.GetAllianceInfo(player.Home.Id);

                if (player.Device != null)
                {
                    await new AvailableServerCommand(player.Device)
                    {
                        Command = new LogicJoinAllianceCommand(player.Device)
                        {
                            AllianceId = alliance.Id,
                            AllianceName = alliance.Name,
                            AllianceBadge = alliance.Badge
                        }
                    }.SendAsync();

                    await new AllianceStreamMessage(player.Device)
                    {
                        Entries = alliance.Stream
                    }.SendAsync();
                }

                player.Save();
            }

            alliance.Save();
            alliance.UpdateOnlineCount();
        }
    }
}