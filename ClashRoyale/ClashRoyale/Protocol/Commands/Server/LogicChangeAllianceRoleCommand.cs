using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class LogicChangeAllianceRoleCommand : LogicCommand
    {
        public LogicChangeAllianceRoleCommand(Device device) : base(device)
        {
            Type = 207;
        }

        public long AllianceId { get; set; }
        public int NewRole { get; set; }

        public override void Encode()
        {
            Data.WriteLong(AllianceId);
            Data.WriteVInt(NewRole);
        }
    }
}