using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class RoyalTvContentMessage : PiranhaMessage
    {
        public RoyalTvContentMessage(Device device) : base(device)
        {
            Id = 24405;
        }

        public int ClassId { set; get; }
        public int InstanceId { set; get; }

        public override void Encode()
        {
            Writer.WriteVInt(0);

            Writer.WriteVInt(ClassId);
            Writer.WriteVInt(InstanceId);
        }
    }
}