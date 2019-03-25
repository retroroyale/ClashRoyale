using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
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
            AllianceId = Buffer.ReadLong();
        }

        public override async void Process()
        {
            var clan = await Resources.Alliances.GetAlliance(AllianceId);
            var player = Device.Player;

            if (clan != null)
            {
                if (clan.Members.Count <= 0 || clan.Members.Count >= 50)
                {
                    await new AllianceJoinFailedMessage(Device).Send();
                }
                else
                {
                    clan.Members.Add(new AllianceMember(player, Alliance.Role.Member));

                    player.Home.AllianceInfo = clan.GetAllianceInfo(player.Home.Id);

                    await new AvailableServerCommand(Device)
                    {
                        Command = new JoinAllianceCommand(Device)
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

                    clan.UpdateOnlineCount();

                    // TODO CLAN MESSAGE
                }
            }
        }
    }
}