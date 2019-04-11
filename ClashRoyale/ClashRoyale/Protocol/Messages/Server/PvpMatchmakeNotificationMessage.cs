using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class PvpMatchmakeNotificationMessage : PiranhaMessage
    {
        public PvpMatchmakeNotificationMessage(Device device) : base(device)
        {
            Id = 22957;
        }

        public int LevelIndex { get; set; }

        public override void Encode()
        {
            Writer.WriteVInt(LevelIndex); 
        }
    }
}