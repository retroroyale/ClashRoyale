using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Clan.StreamEntry.Entries
{
    public class DonateStreamEntry : AllianceStreamEntry
    {
        public DonateStreamEntry()
        {
            StreamEntryType = 1;
        }

        [JsonProperty("msg")] public string Message { get; set; }
        [JsonProperty("totalCapacity")] public int TotalCapacity { get; set; }
        [JsonProperty("usedCapacity")] public int UsedCapacity { get; set; }

        public override void Encode(IByteBuffer packet)
        {
            base.Encode(packet);

            packet.WriteVInt(1);

            packet.WriteVInt(TotalCapacity);
            packet.WriteVInt(UsedCapacity);

            packet.WriteVInt(1);

            //if (UsedCapacity > 0)
            {
                // DonationContainer
                packet.WriteLong(SenderId);
                packet.WriteVInt(1); // Count
                packet.WriteVInt(26);
                packet.WriteVInt(1);
            }

            packet.WriteVInt(1); // Count
            packet.WriteVInt(26);
            packet.WriteVInt(1);

            packet.WriteVInt(1);
            packet.WriteScString(Message);
        }
    }
}