using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class LogicAllianceSettingsChangedCommand : LogicCommand
    {
        public LogicAllianceSettingsChangedCommand(Device device) : base(device)
        {
            Type = 212;
        }

        public long AllianceId { get; set; }
        public int AllianceBadge { get; set; }

        public override void Encode()
        {
            Data.WriteLong(AllianceId);

            Data.WriteVInt(16);
            Data.WriteVInt(AllianceBadge);
        }
    }
}