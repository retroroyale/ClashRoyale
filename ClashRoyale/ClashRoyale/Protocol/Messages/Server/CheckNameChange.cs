using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class CheckNameChange : PiranhaMessage
    {
        public CheckNameChange(Device device) : base(device)
        {
            Id = 20300;
        }

        public string Name { get; set; }

        public override void Encode()
        {
            Writer.WriteByte(0);
            Writer.WriteInt(0);
            Writer.WriteScString(Name);
        }
    }
}