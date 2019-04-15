using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class ReportUserStatusMessage : PiranhaMessage
    {
        public ReportUserStatusMessage(Device device) : base(device)
        {
            Id = 20117;
        }

        public int ErrorCode { get; set; }

        // Errorcodes:
        // 1 = sent
        // 2 = too much sent
        // 3 = already reported
        // 6 = too much clan reports sent(?)
        // 7 = already reported(?)

        public override void Encode()
        {
            Writer.WriteInt(ErrorCode);
        }
    }
}