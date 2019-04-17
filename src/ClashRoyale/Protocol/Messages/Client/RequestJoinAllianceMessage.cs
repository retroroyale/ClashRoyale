using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class RequestJoinAllianceMessage : PiranhaMessage
    {
        public RequestJoinAllianceMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14317;
        }

        public long AllianceId { get; set; }
        public string Message { get; set; }

        public override void Decode()
        {
            AllianceId = Reader.ReadLong();
            Message = Reader.ReadScString();
            Reader.ReadInt();
        }

        public override async void Process()
        {
            var alliance = await Resources.Alliances.GetAllianceAsync(AllianceId);

            if (alliance != null)
            {
                if (alliance.Members.Count <= 0 || alliance.Members.Count >= 50)
                {
                    await new AllianceJoinRequestFailedMessage(Device).SendAsync();
                }
                else if (alliance.Stream.FindIndex(e => e.StreamEntryType == 3 && e.SenderId == Device.Player.Home.Id && ((JoinRequestAllianceStreamEntry)e).State == 1) > -1)
                {
                    await new AllianceJoinRequestFailedMessage(Device)
                    {
                        Reason = 2
                    }.SendAsync();
                }
                else
                {
                    await new AllianceJoinRequestOkMessage(Device).SendAsync();

                    var entry = new JoinRequestAllianceStreamEntry
                    {
                        Message = Message,
                        State = 1
                    };

                    entry.SetTarget(Device.Player);
                    entry.SetSender(Device.Player);
                    alliance.AddEntry(entry);

                    alliance.Save();
                }
            }
        }
    }
}