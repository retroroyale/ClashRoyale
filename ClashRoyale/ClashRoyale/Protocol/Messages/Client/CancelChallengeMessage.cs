using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class CancelChallengeMessage : PiranhaMessage
    {
        public CancelChallengeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14123;
        }

        public override async void Process()
        {
            await new CancelChallengeDoneMessage(Device).Send();
        }
    }
}