using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicSwapSpellsCommand : LogicCommand
    {
        public LogicSwapSpellsCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int CardId { get; set; }
        public int DeckIndex { get; set; }

        public override void Decode()
        {
            Buffer.ReadVInt();
            Buffer.ReadVInt();

            CardId = Buffer.ReadVInt();
            DeckIndex = Buffer.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.Deck.SwapCard(CardId, DeckIndex);
        }
    }
}