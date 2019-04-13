using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class UnlockAccountFailedMessage : PiranhaMessage
    {
        public UnlockAccountFailedMessage(Device device) : base(device)
        {
            Id = 20133;
        }

        public int ErrorCode { get; set; }

        // Errorcodes:
        // 4 = invalid
        // 5 = unavailable
        // 10 = maintenance

        public override void Encode()
        {
            Writer.WriteInt(ErrorCode); 
        }
    }
}