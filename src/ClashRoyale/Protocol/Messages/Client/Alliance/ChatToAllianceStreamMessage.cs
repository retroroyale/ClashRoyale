using System;
using System.Linq;
using ClashRoyale.Database;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class ChatToAllianceStreamMessage : PiranhaMessage
    {
        public ChatToAllianceStreamMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14315;
        }

        public string Message { get; set; }

        public override void Decode()
        {
            Message = Reader.ReadScString();
        }

        public override async void Process()
        {
            var info = Device.Player.Home.AllianceInfo;
            if (!info.HasAlliance) return;

            var alliance = await Resources.Alliances.GetAllianceAsync(info.Id);
            if (alliance == null) return;

            if (Message.StartsWith('/'))
            {
                var cmd = Message.Split(' ');
                var cmdType = cmd[0];
                var cmdValue = 0;

                if (cmd.Length > 1)
                    if (Message.Split(' ')[1].Any(char.IsDigit))
                        int.TryParse(Message.Split(' ')[1], out cmdValue);

                switch (cmdType)
                {
                    case "/upgrade":
                    {
                        Device.Player.Home.Deck.UpgradeAll();
                        Device.Disconnect();
                        break;
                    }

                    case "/exp":
                    {
                        Device.Player.Home.AddExpPoints(cmdValue);
                        Device.Disconnect();
                        break;
                    }

                    case "/gold":
                    {
                        Device.Player.Home.Gold += cmdValue;
                        Device.Disconnect();
                        break;
                    }

                    case "/status":
                    {
                        await new ServerErrorMessage(Device)
                        {
                            Message = $"Online Players: {Resources.Players.Count}\nTotal Players: {await PlayerDb.CountAsync()}"
                        }.SendAsync();

                        break;
                    }

                        case "/replay":
                        {
                            await new HomeBattleReplayDataMessage(Device).SendAsync();
                            break;
                        }

                        /*case "/free":
                        {
                            Device.Player.Home.FreeChestTime = Device.Player.Home.FreeChestTime.Subtract(TimeSpan.FromMinutes(245));
                            Device.Disconnect();
                            break;
                        }*/

                        /*case "/trophies":
                        {
                            if (cmdValue >= 0)
                                Device.Player.Home.Arena.AddTrophies(cmdValue);
                            else if (cmdValue < 0)
                                Device.Player.Home.Arena.RemoveTrophies(cmdValue);

                            Device.Disconnect();
                            break;
                        }*/

                        /*case "/test":
                        {
                            var entry = new DonateStreamEntry
                            {
                                Message = Message,
                                TotalCapacity = 10
                            };

                            entry.SetSender(Device.Player);

                            alliance.AddEntry(entry);

                            break;
                        }*/

                        /*case "/test":
                        {
                            var entry = new AllianceMailAvatarStreamEntry
                            {
                                Message = "Works",
                                Title = "Hehe",
                                AllianceId = 1,
                                AllianceName = "LOL",
                                AllianceBadge = 5,
                                IsNew = true
                            };

                            entry.SetSender(Device.Player);

                            Device.Player.AddEntry(entry);

                            break;
                        }*/
                }
            }
            else
            {
                var entry = new ChatStreamEntry
                {
                    Message = Message
                };

                entry.SetSender(Device.Player);

                alliance.AddEntry(entry);
            }
        }
    }
}