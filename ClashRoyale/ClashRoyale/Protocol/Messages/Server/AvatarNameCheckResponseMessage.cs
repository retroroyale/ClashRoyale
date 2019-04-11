using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class AvatarNameCheckResponseMessage : PiranhaMessage
    {
        public AvatarNameCheckResponseMessage(Device device) : base(device)
        {
            Id = 20300;
        }

        public string Name { get; set; }

        public override void Encode()
        {
            Writer.WriteBoolean(false); // IsValid
            Writer.WriteInt(0); // ErrorCode
            Writer.WriteScString(Name);
        }
    }
}