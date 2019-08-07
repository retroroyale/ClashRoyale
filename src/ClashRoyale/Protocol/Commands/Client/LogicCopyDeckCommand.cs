using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicCopyDeckCommand : LogicCommand
    {
        public LogicCopyDeckCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int Count { get; set; }
        public int Slot { get; set; }

        public override void Decode()
        {
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            Slot = Reader.ReadVInt();
            Count = Reader.ReadVInt();
        }

        public override void Process()
        {
            var deck = Device.Player.Home.Deck;

            deck.SwitchDeck(Slot);

            for (var i = 0; i < Count; i++)
            {
                var globalId = Reader.ReadVInt();
                var offset = deck.GetCardOffset(globalId) - 8;

                if (offset != i && offset > -1)
                    deck.SwapCard(offset, i);
            }
        }
    }
}