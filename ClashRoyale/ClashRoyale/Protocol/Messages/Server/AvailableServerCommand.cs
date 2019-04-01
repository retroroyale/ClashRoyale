using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvailableServerCommand : PiranhaMessage
    {
        public AvailableServerCommand(Device device) : base(device)
        {
            Id = 24111;
        }

        public LogicCommand Command { get; set; }

        public override void Encode()
        {
            Command.Encode();

            Writer.WriteVInt(Command.Type);
            Writer.WriteBytes(Command.Data);
        }
    }
}