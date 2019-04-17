using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceStreamEntryMessage : PiranhaMessage
    {
        public AllianceStreamEntryMessage(Device device) : base(device)
        {
            Id = 24312;
        }

        public AllianceStreamEntry Entry { get; set; }

        public override void Encode()
        {
            if (Entry.StreamEntryType == 10)
            {
                if (Entry.SenderId == Device.Player.Home.Id)
                {
                    ((ChallengeStreamEntry)Entry).Closed = true;
                }
            }

            Entry.Encode(Writer);
        }
    }
}