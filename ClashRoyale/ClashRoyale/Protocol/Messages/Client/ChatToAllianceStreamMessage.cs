using System;
using System.Linq;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Messages.Server;

namespace ClashRoyale.Protocol.Messages.Client
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
            Message = Buffer.ReadScString();
        }

        public override async void Process()
        {
            var info = Device.Player.Home.AllianceInfo;

            if (info.HasAlliance)
            {
                var clan = await Resources.Alliances.GetAlliance(info.Id);

                if (clan != null)
                {
                    var entry = new ChatStreamEntry
                    {
                        CreationDateTime = DateTime.UtcNow,
                        Id = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        Message = Message
                    };

                    entry.SetSender(Device.Player);

                    clan.AddEntry(entry);

                    foreach (var member in clan.Members.Where(m => m.IsOnline))
                    {
                        var player = await Resources.Players.GetPlayer(member.Id, true);

                        if (player != null)
                        {
                            await new AllianceStreamEntryMessage(player.Device)
                            {
                                Entry = entry
                            }.Send();
                        }
                    }
                }
            }
        }
    }
}