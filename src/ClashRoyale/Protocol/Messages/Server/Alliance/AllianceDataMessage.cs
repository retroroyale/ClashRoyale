using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;
using System.Linq;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AllianceDataMessage : PiranhaMessage
    {
        public AllianceDataMessage(Device device) : base(device)
        {
            Id = 24301;
        }

        public Alliance Alliance { get; set; }

        public override void Encode()
        {
            Alliance.AllianceFullEntry(Writer);

            Writer.WriteVInt(Alliance.Members.Count);

            foreach (var member in Alliance.Members.OrderByDescending(p => p.Score)) member.AllianceMemberEntry(Writer);

            // Clan Chest
            Writer.WriteBoolean(true);
            Writer.WriteVInt(3); // State 0 = Preparation, 1 = Live, 2 = Over, 3 = Not active
            Writer.WriteVInt(3600); // Seconds
            Writer.WriteVInt(300 * 2); // Crowns/Wins

            Writer.WriteInt(TimeUtils.CurrentUnixTimestamp); // Begin (+Preparation)
            Writer.WriteInt(1594578202); // End

            Writer.WriteVInt(3446115); // Low Id (?)
            Writer.WriteVInt(1); // High Id (?)

            Writer.WriteVInt(0);
        }
    }
}