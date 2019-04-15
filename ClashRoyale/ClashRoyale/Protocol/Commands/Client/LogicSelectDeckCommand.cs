using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicSelectDeckCommand : LogicCommand
    {
        public LogicSelectDeckCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int DeckIndex { get; set; }

        public override void Decode()
        {
            Buffer.ReadVInt();
            Buffer.ReadVInt();

            DeckIndex = Buffer.ReadVInt();
        }

        public override async void Process()
        {
            await new OutOfSyncMessage(Device).SendAsync();
        }
    }
}