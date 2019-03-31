using System;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using DotNetty.Buffers;

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
                        Id = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        Message = Message
                    };

                    entry.SetSender(Device.Player);

                    clan.AddEntry(entry);
                }
            }
        }
    }
}