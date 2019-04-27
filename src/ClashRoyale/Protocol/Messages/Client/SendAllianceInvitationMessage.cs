using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class SendAllianceInvitationMessage : PiranhaMessage
    {
        public SendAllianceInvitationMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14322;
        }

        public override async void Process()
        {
            // TODO:
            await new ServerErrorMessage(Device)
            {
                Message = "Coming soon"
            }.SendAsync();
        }
    }
}