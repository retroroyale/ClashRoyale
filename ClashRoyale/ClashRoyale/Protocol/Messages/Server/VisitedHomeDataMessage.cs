using ClashRoyale.Extensions;
using ClashRoyale.Logic;

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

            Packet.WriteVInt(8);
            Packet.WriteShort(255);
            Packet.WriteVInt(1);

            foreach (var card in Player.Home.Deck.GetRange(0, 8))
                card.Encode(Packet);

            Packet.WriteLong(Player.Home.PlayerId);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(1);

            Player.LogicClientAvatar(Packet);
        }
    }
}