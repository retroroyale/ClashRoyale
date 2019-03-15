using System.Collections.Generic;
using ClashRoyale.Extensions;
using ClashRoyale.Logic.Home.Decks.Items;
using DotNetty.Buffers;

namespace ClashRoyale.Logic.Home.Chests.Items
{
    public class Chest : List<Card>
    {
        public enum ChestType
        {
            Free = 2,
            Crown = 3,
            Shop = 4
        }

        public int ChestId { get; set; }
        public ChestType Type { get; set; }
        public int Gold { get; set; }
        public int Gems { get; set; }
        public bool IsDraft { get; set; }

        public void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(1);
            packet.WriteVInt(IsDraft ? 1 : 0);

            packet.WriteVInt(Count);

            foreach (var card in this) card.Encode(packet);

            if (!IsDraft)
            {
                packet.WriteVInt(0);
                packet.WriteVInt(Gold); // Gold
                packet.WriteVInt(Gems); // Gems

                packet.WriteVInt(ChestId);
                packet.WriteVInt((int) Type);
                packet.WriteVInt((int) Type);
            }
            else
            {
                packet.WriteNullVInt();
                packet.WriteVInt(2051);

                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(6188);
                packet.WriteVInt(10);
                packet.WriteVInt(1);
            }

            packet.WriteNullVInt(2);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
        }

        public new void Add(Card card)
        {
            var index = FindIndex(c => c.CardId == card.CardId);

            if (index > -1)
                this[index].Count += card.Count;
            else
                base.Add(card);
        }
    }
}