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

        public long UserId { get; set; }

        public override void Encode()
        {
            Packet.WriteVInt(8);

            Packet.WriteHex("00FF");

            Packet.WriteVInt(1);

            foreach (var card in Device.Player.Home.Deck.GetRange(0, 8))
                card.Encode(Packet);

            Packet.WriteLong(UserId);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(1);

            Device.Player.LogicClientAvatar(Packet);
        }
    }
}