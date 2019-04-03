using System;
using ClashRoyale.Extensions;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
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
            var alliance = await Resources.Alliances.GetAlliance(home.AllianceInfo.Id);

            var member = alliance?.GetMember(MemberId);
            var sender = alliance?.GetMember(home.Id);

            if(member != null && sender != null)
            {
                if (GameUtils.IsHigherRoleThan(sender.Role, member.Role))
                {                 
                    var player = await member.GetPlayer();

                    if (player != null)
                    {
                        var oldRole = member.Role;
                        member.Role = NewRole;
                        player.Home.AllianceInfo.Role = NewRole;

                        var entry = new AllianceEventStreamEntry
                        {
                            CreationDateTime = DateTime.UtcNow,
                            Id = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                            EventType = GameUtils.IsHigherRoleThan(NewRole, oldRole) ? AllianceEventStreamEntry.Type.Promote : AllianceEventStreamEntry.Type.Demote
                        };

                        entry.SetTarget(player);
                        entry.SetSender(Device.Player);
                        alliance.AddEntry(entry);

                        if (NewRole == (int)Alliance.Role.Leader)
                        {
                            var oldLeader = await sender.GetPlayer();

                            sender.Role = (int)Alliance.Role.CoLeader;
                            oldLeader.Home.AllianceInfo.Role = (int) Alliance.Role.CoLeader;

                            oldLeader.Save();

                            var demoteEntry = new AllianceEventStreamEntry
                            {
                                CreationDateTime = DateTime.UtcNow,
                                Id = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
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
}