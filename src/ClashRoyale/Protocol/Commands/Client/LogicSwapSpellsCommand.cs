using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicSwapSpellsCommand : LogicCommand
    {
        public LogicSwapSpellsCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int CardOffset { get; set; }
        public int DeckOffset { get; set; }

        public override void Decode()
        {
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            CardOffset = Reader.ReadVInt();
            DeckOffset = Reader.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.Deck.SwapCard(CardOffset, DeckOffset);
        }
    }
}