using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
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
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            DeckIndex = Reader.ReadVInt();
        }

        public override void Process()
        {
            var home = Device.Player.Home;
            home.Deck.SwitchDeck(DeckIndex);
        }
    }
}