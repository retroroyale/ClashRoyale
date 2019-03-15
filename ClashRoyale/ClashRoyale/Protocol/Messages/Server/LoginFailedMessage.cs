using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class LoginFailedMessage : PiranhaMessage
    {
        public LoginFailedMessage(Device device) : base(device)
        {
            Id = 20103;
        }

        public byte ErrorCode { get; set; }
        public int SecondsUntilMaintenanceEnds { get; set; }
        public string Reason { get; set; }
        public string ResourceFingerprintData { get; set; }

        // After login
        // 7  = Content Update
        // 8  = Update available
        // 10 = Maintenance
        // 11 = Banned
        // 12 = Played too long

        // Before login
        // 8  = Maintenance
        // 9  = Banned
        // 10 = Update available

        public override void Encode()
        {
            Packet.WriteByte(ErrorCode); // ErrorCode
            Packet.WriteScString(ResourceFingerprintData); // Fingerprint
            Packet.WriteScString(null);
            Packet.WriteScString(null); // Content URL
            Packet.WriteScString(null); // Update URL
            Packet.WriteScString(Reason);
            Packet.WriteVInt(SecondsUntilMaintenanceEnds);
        }
    }
}