using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Logic.Home.StreamEntry.Entries
{
    public class DonationReceivedStreamEntry : AvatarStreamEntry
    {
        public DonationReceivedStreamEntry()
        {
            StreamEntryType = 7;
        }

        public override void Encode(IByteBuffer packet)
        {
            base.Encode(packet);

            packet.WriteVInt(1); // Count

            packet.WriteVInt(26);
            packet.WriteVInt(1);
        }
    }
}