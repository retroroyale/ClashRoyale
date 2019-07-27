using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class LogicChangeNameCommand : LogicCommand
    {
        public LogicChangeNameCommand(Device device) : base(device)
        {
            Type = 201;
        }

        public int NameSet { get; set; }

        public override void Encode()
        {
            Data.WriteScString(Device.Player.Home.Name);
            Data.WriteInt(NameSet);
        }
    }
}