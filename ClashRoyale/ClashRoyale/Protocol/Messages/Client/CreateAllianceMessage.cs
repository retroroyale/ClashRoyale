using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using ClashRoyale.Database;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;

namespace ClashRoyale.Protocol.Messages.Client
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
            Name = Buffer.ReadScString();
            Description = Buffer.ReadScString();
            Buffer.ReadVInt();
            Badge = Buffer.ReadVInt();
            Type = Buffer.ReadVInt();
            RequiredScore = Buffer.ReadVInt();
            Region = Buffer.ReadVInt();
            Region = Buffer.ReadVInt();
        }

        public override async void Process()
        {
            var alliance = await AllianceDb.Create();

            if (alliance != null)
            {
                alliance.Name = Name;
                alliance.Description = Description;
                alliance.Badge = Badge;
                alliance.Type = Type;
                alliance.RequiredScore = RequiredScore;

                var player = Device.Player;

                alliance.Members.Add(
                    new AllianceMember(player, Alliance.Role.Leader));

                player.Home.AllianceInfo = alliance.GetAllianceInfo(player.Home.Id);

                player.Save();         
                alliance.Save();

                await new AvailableServerCommand(Device)
                {
                    Command = new JoinAllianceCommand(Device)
                    {
                        AllianceId = alliance.Id,
                        AllianceName = Name,
                        AllianceBadge = Badge
                    }
                }.Send();
            }
            else
            {
                await Device.Disconnect();
            }
        }
    }
}