using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan;

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

            foreach (var member in Alliance.Members) member.AllianceMemberEntry(Writer);

            // Clan Chest
            Writer.WriteBoolean(false);

            /*Writer.WriteBoolean(true);
            Writer.WriteVInt(1); // State 0 = Preparation, 1 = Live, 2 = Over
            Writer.WriteVInt(3600); // Seconds
            Writer.WriteVInt(300 * 2); // Crowns/Wins

            Writer.WriteInt(1493968907); // Begin (+Preparation)
            Writer.WriteInt(1494228107); // End

            Writer.WriteVInt(3446115); // Low Id (?)
            Writer.WriteVInt(1); // High Id (?)

            Writer.WriteVInt(1);*/
        }
    }
}