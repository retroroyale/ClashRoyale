using ClashRoyale.Database;
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
                clan.Remove(player.Home.Id);
                player.Home.AllianceInfo.Reset();

                await new AvailableServerCommand(Device)
                {
                    Command = new LeaveAllianceCommand(Device)
                    {
                        AllianceId = clan.Id
                    }
                }.Send();

                if (clan.Members.Count == 0)
                {
                    await AllianceDb.Delete(clan.Id);
                    await Redis.UncacheAlliance(clan.Id);
                }
                else
                {
                    clan.UpdateOnlineCount();
                }

                // TODO CLAN MESSAGE
            }
        }
    }
}