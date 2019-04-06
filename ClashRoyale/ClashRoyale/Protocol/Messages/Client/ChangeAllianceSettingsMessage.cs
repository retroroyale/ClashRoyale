using System;
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
            Description = Reader.ReadScString();
            Reader.ReadVInt();
            Badge = Reader.ReadVInt();
            Type = Reader.ReadVInt();
            RequiredScore = Reader.ReadVInt();
            Reader.ReadVInt();
            Region = Reader.ReadVInt();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAlliance(home.AllianceInfo.Id);

            if (alliance != null)
            {
                var oldBadge = alliance.Badge;

                alliance.Type = Type;
                alliance.Badge = Badge;
                alliance.Region = Region;
                alliance.Description = Description;
                alliance.RequiredScore = RequiredScore;

                alliance.Save();

                if (Badge != oldBadge)
                {
                    foreach (var member in alliance.Members)
                    {
                        var player = await member.GetPlayer();

                        if (player != null)
                        {
                            // TODO:
                            /*if (member.IsOnline)
                            {
                                await new AvailableServerCommand(player.Device)
                                {
                                    Command = new LogicAllianceSettingsChangedCommand(player.Device)
                                    {
                                        AllianceId = alliance.Id,
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