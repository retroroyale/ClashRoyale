using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class LeaveAllianceCommand : LogicCommand
    {
        public LeaveAllianceCommand(Device device) : base(device)
        {
            Type = 205;
        }

        public long AllianceId { get; set; }

        public override void Encode()
        {
            Data.WriteLong(AllianceId);

            Data.WriteByte(0);
            Data.WriteByte(1);

            Data.WriteNullVInt(2);

            Data.WriteByte(0);
            Data.WriteByte(0);
        }
    }
}