using System;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicKickAllianceMemberCommand : LogicCommand
    {
        public LogicKickAllianceMemberCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public long MemberId { get; set; }
        public string Message { get; set; }

        public override void Decode()
        {
            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Message = Buffer.ReadScString();

            MemberId = Buffer.ReadLong();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var clan = await Resources.Alliances.GetAlliance(home.AllianceInfo.Id);

            if (clan != null)
            {
                var member = clan.GetMember(MemberId);
                if (member != null)
                {
                    var player = await member.GetPlayer();

                    if (player != null)
                    {
                        var entry = new AllianceEventStreamEntry
                        {
                            CreationDateTime = DateTime.UtcNow,
                            Id = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                            EventType = AllianceEventStreamEntry.Type.Kick
                        };

                        entry.SetTarget(player);
                        entry.SetSender(Device.Player);

                        clan.AddEntry(entry);
                        clan.Remove(MemberId);

                        player.Home.AllianceInfo.Reset();

                        player.Save();
                        clan.Save();

                        if (player.Device != null)
                            await new AvailableServerCommand(player.Device)
                            {
                                Command = new LogicLeaveAllianceCommand(player.Device)
                                {
                                    AllianceId = clan.Id,
                                    IsKick = true
                                }
                            }.Send();
                    }
                }
            }
        }
    }
}