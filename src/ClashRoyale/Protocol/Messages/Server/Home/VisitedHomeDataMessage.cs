using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class VisitedHomeDataMessage : PiranhaMessage
    {
        public VisitedHomeDataMessage(Device device) : base(device)
        {
            Id = 24113;
        }

        public Player Player { get; set; }

        public override void Encode()
        {
            if (Player == null)
                return;

            Writer.WriteVInt(8);
            Writer.WriteShort(255);
            Writer.WriteVInt(1);

            foreach (var card in Player.Home.Deck.GetRange(0, 8))
                card.Encode(Writer);

            Writer.WriteLong(Player.Home.Id);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);

            Player.LogicClientAvatar(Writer);
        }
    }
}