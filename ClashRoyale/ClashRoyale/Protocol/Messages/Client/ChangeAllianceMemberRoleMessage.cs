using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class ChangeAllianceMemberRoleMessage : PiranhaMessage
    {
        public ChangeAllianceMemberRoleMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14306;
        }

        public long MemberId { get; set; }

        public override void Decode()
        {
            MemberId = Reader.ReadVInt();
        }

        public override async void Process()
        {
            // TODO
        }
    }
}