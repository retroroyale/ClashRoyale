using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class LogicDiamondsAddedCommand : LogicCommand
    {
        public LogicDiamondsAddedCommand(Device device) : base(device)
        {
            Type = 202;
        }

        public int Diamonds { get; set; }

        public override void Encode()
        {
            Data.WriteVInt(0);
            Data.WriteVInt(Diamonds);
            Data.WriteScString("GPA.0000-0000-0000-00000");
            Data.WriteVInt(1);

            Data.WriteNullVInt(2);
            Data.WriteVInt(0);
            Data.WriteVInt(0);
        }
    }
}