using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
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
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            Message = Reader.ReadScString();

            MemberId = Reader.ReadLong();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

            if (alliance != null)
            {
                var member = alliance.GetMember(MemberId);
                if (member != null)
                {
                    var player = await member.GetPlayerAsync();

                    if (player != null)
                    {
                        var entry = new AllianceEventStreamEntry
                        {
                            EventType = AllianceEventStreamEntry.Type.Kick
                        };

                        entry.SetTarget(player);
                        entry.SetSender(Device.Player);

                        alliance.AddEntry(entry);
                        alliance.Remove(MemberId);

                        player.Home.AllianceInfo.Reset();

                        player.Save();
                        alliance.Save();

                        if (player.Device != null)
                            await new AvailableServerCommand(player.Device)
                            {
                                Command = new LogicLeaveAllianceCommand(player.Device)
                                {
                                    AllianceId = alliance.Id,
                                    IsKick = true
                                }
                            }.SendAsync();
                    }
                }
            }
        }
    }
}