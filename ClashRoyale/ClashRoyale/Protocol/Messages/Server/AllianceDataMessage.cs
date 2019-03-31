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
            Alliance.AllianceFullEntry(Packet);

            Packet.WriteVInt(Alliance.Members.Count);

            foreach (var member in Alliance.Members) member.AllianceMemberEntry(Packet);

            // Clan Chest
            Packet.WriteBoolean(false);
        }
    }
}