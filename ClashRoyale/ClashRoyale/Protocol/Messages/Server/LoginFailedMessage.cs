using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class LoginFailedMessage : PiranhaMessage
    {
        public LoginFailedMessage(Device device) : base(device)
        {
            Id = 20103;
            Version = 4;
        }

        public byte ErrorCode { get; set; }
        public int SecondsUntilMaintenanceEnds { get; set; }
        public string Reason { get; set; }
        public string ResourceFingerprintData { get; set; }
        public string ContentUrl { get; set; }
        public string UpdateUrl { get; set; }

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
            Writer.WriteByte(ErrorCode); // ErrorCode
            Writer.WriteScString(ResourceFingerprintData); // Fingerprint
            Writer.WriteScString(null);
            Writer.WriteScString(ContentUrl); // Content URL
            Writer.WriteScString(UpdateUrl); // Update URL
            Writer.WriteScString(Reason);
            Writer.WriteVInt(SecondsUntilMaintenanceEnds);
        }
    }
}