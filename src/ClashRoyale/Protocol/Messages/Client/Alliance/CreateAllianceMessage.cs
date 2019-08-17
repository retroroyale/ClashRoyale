using ClashRoyale.Database;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class CreateAllianceMessage : PiranhaMessage
    {
        public CreateAllianceMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14301;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Badge { get; set; }
        public int Type { get; set; }
        public int RequiredScore { get; set; }
        public int Region { get; set; }

        public override void Decode()
        {
            Name = Reader.ReadScString();
            Description = Reader.ReadScString();
            Reader.ReadVInt();
            Badge = Reader.ReadVInt();
            Type = Reader.ReadVInt();
            RequiredScore = Reader.ReadVInt();
            Region = Reader.ReadVInt();
            Region = Reader.ReadVInt();
        }

        public override async void Process()
        {
            var player = Device.Player;
            if (!player.Home.UseGold(1000)) return;

            var alliance = await AllianceDb.CreateAsync();

            if (alliance != null)
            {
                alliance.Name = Name;
                alliance.Description = Description;
                alliance.Badge = Badge;
                alliance.Type = Type;
                alliance.RequiredScore = RequiredScore;
                alliance.Region = Region;

                alliance.Members.Add(
                    new AllianceMember(player, Logic.Clan.Alliance.Role.Leader));

                player.Home.AllianceInfo = alliance.GetAllianceInfo(player.Home.Id);

                alliance.Save();
                player.Save();

                await new AvailableServerCommand(Device)
                {
                    Command = new LogicJoinAllianceCommand(Device)
                    {
                        AllianceId = alliance.Id,
                        AllianceName = Name,
                        AllianceBadge = Badge
                    }
                }.SendAsync();

                await new AvailableServerCommand(Device)
                {
                    Command = new LogicChangeAllianceRoleCommand(Device)
                    {
                        AllianceId = alliance.Id,
                        NewRole = 2
                    }
                }.SendAsync();

                alliance.UpdateOnlineCount();
            }
            else
            {
                Device.Disconnect();
            }
        }
    }
}