using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class LogicLeaveAllianceCommand : LogicCommand
    {
        public LogicLeaveAllianceCommand(Device device) : base(device)
        {
            Type = 205;
        }

        public long AllianceId { get; set; }
        public bool IsKick { get; set; }

        public override void Encode()
        {
            Data.WriteLong(AllianceId);

            Data.WriteBoolean(IsKick);
            Data.WriteBoolean(!IsKick);

            Data.WriteNullVInt(2);

            Data.WriteByte(0);
            Data.WriteByte(0);
        }
    }
}