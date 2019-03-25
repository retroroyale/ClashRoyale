using ClashRoyale.Logic;
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
            var player = Device.Player;
            var clan = await Resources.Alliances.GetAlliance(player.Home.AllianceInfo.Id);         

            if (clan != null)
            {
                if (clan.Members.Count <= 0 || clan.Members.Count >= 50)
                {
                    await new AllianceJoinFailedMessage(Device).Send();
                }
                else
                {
                    clan.Remove(player.Home.Id);
                    player.Home.AllianceInfo.Reset();

                    await new AvailableServerCommand(Device)
                    {
                        Command = new LeaveAllianceCommand(Device)
                        {
                            AllianceId = clan.Id
                        }
                    }.Send();

                    clan.UpdateOnlineCount();

                    // TODO CLAN MESSAGE
                }
            }
        }
    }
}