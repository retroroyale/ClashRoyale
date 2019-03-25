using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using ClashRoyale.Database;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class ChatToAllianceStreamMessage : PiranhaMessage
    {
        public ChatToAllianceStreamMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14315;
        }

        public string Message { get; set; }

        public override void Decode()
        {
            Message = Buffer.ReadScString();
        }

        public override async void Process()
        {
            await new ServerErrorMessage(Device)
            {
                Message = Message
            }.Send();
        }
    }
}