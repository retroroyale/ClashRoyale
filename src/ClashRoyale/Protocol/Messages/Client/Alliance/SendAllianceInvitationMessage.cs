using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class SendAllianceInvitationMessage : PiranhaMessage
    {
        public SendAllianceInvitationMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14322;
        }

        public override async void Process()
        {
            await new AllianceInvitationSendFailedMessage(Device)
            {
                Reason = 6
            }.SendAsync();

            // TODO
        }
    }
}