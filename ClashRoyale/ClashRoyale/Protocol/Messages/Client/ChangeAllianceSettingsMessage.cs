using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class ChangeAllianceSettingsMessage : PiranhaMessage
    {
        public ChangeAllianceSettingsMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14306;
        }

        public string Description { get; set; }
        public int Badge { get; set; }
        public int Type { get; set; }
        public int RequiredScore { get; set; }
        public int Region { get; set; }

        public override void Decode()
        {
            Description = Buffer.ReadScString();
            Buffer.ReadVInt();
            Badge = Buffer.ReadVInt();
            Type = Buffer.ReadVInt();
            RequiredScore = Buffer.ReadVInt();
            Buffer.ReadVInt();
            Region = Buffer.ReadVInt();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var clan = await Resources.Alliances.GetAlliance(home.AllianceInfo.Id);

            if (clan != null)
            {
                var oldBadge = clan.Badge;

                clan.Type = Type;
                clan.Badge = Badge;
                clan.Region = Region;
                clan.Description = Description;
                clan.RequiredScore = RequiredScore;

                clan.Save();

                foreach (var member in clan.Members)
                {
                    var player = await member.GetPlayer();

                    if (player != null)
                    {
                        if (Badge != oldBadge)
                        {
                            // TODO:
                            /*if (member.IsOnline)
                            {
                                await new AvailableServerCommand(Device)
                                {
                                    Command = new LogicAllianceSettingsChangedCommand(Device)
                                    {
                                        AllianceId = clan.Id,
                                        AllianceBadge = Badge
                                    }
                                }.Send();
                            }*/

                            player.Home.AllianceInfo.Badge = Badge;
                            player.Save();
                        }               
                    }
                }
            }
        }
    }
}