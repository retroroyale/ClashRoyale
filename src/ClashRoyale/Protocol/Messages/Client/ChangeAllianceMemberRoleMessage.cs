using ClashRoyale.Extensions;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class ChangeAllianceMemberRoleMessage : PiranhaMessage
    {
        public ChangeAllianceMemberRoleMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14306;
        }

        public long MemberId { get; set; }
        public int NewRole { get; set; }

        public override void Decode()
        {
            MemberId = Reader.ReadLong();
            NewRole = Reader.ReadVInt();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

            var member = alliance?.GetMember(MemberId);
            var sender = alliance?.GetMember(home.Id);

            if (member != null && sender != null)
                if (GameUtils.IsHigherRoleThan(sender.Role, member.Role))
                {
                    var player = await member.GetPlayerAsync();

                    if (player != null)
                    {
                        var oldRole = member.Role;
                        member.Role = NewRole;
                        player.Home.AllianceInfo.Role = NewRole;

                        var entry = new AllianceEventStreamEntry
                        {
                            EventType = GameUtils.IsHigherRoleThan(NewRole, oldRole)
                                ? AllianceEventStreamEntry.Type.Promote
                                : AllianceEventStreamEntry.Type.Demote
                        };

                        entry.SetTarget(Device.Player);
                        entry.SetSender(player);
                        alliance.AddEntry(entry);

                        if (member.IsOnline)
                            await new AvailableServerCommand(player.Device)
                            {
                                Command = new LogicChangeAllianceRoleCommand(player.Device)
                                {
                                    AllianceId = alliance.Id,
                                    NewRole = NewRole
                                }
                            }.SendAsync();

                        if (NewRole == (int) Alliance.Role.Leader)
                        {
                            var oldLeader = await sender.GetPlayerAsync();

                            sender.Role = (int) Alliance.Role.CoLeader;
                            oldLeader.Home.AllianceInfo.Role = (int) Alliance.Role.CoLeader;

                            oldLeader.Save();

                            var demoteEntry = new AllianceEventStreamEntry
                            {
                                EventType = AllianceEventStreamEntry.Type.Demote
                            };

                            demoteEntry.SetTarget(Device.Player);
                            demoteEntry.SetSender(Device.Player);
                            alliance.AddEntry(demoteEntry);
                        }

                        alliance.Save();
                        player.Save();
                    }
                }
        }
    }
}