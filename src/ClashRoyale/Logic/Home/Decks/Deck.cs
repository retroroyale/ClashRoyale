using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Extensions;
using ClashRoyale.Logic.Home.Decks.Items;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home.Decks
{
    public class Deck : List<Card>
    {
        [JsonIgnore] public Home Home { get; set; }

        public void Initialize()
        {
            for (var i = 0; i < 8; i++)
                Add(new Card(26, i, false));
        }

        public new void Add(Card card)
        {
            var index = FindIndex(c => c.ClassId == card.ClassId && c.InstanceId == card.InstanceId);

            if (index <= -1)
                base.Add(card);
            else
                this[index].Count += card.Count;
        }

        public void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(1); // DeckCount

            packet.WriteVInt(8);

            foreach (var card in GetRange(0, 8))
                packet.WriteVInt(card.GlobalId);

            packet.WriteByte(255);

            foreach (var card in GetRange(0, 8))
                card.Encode(packet);

            packet.WriteVInt(Count - 8);

            foreach (var card in this.Skip(8))
                card.Encode(packet);

            packet.WriteVInt(0); // CurrentSlot
        }

        public void EncodeAttack(IByteBuffer packet)
        {
            foreach (var card in GetRange(0, 8))
                card.EncodeAttack(packet);
        }

        public Card GetCard(int classId, int instanceId)
        {
            var index = FindIndex(c => c.ClassId == classId && c.InstanceId == instanceId);
            return index > -1 ? this[index] : null;
        }

        public void SwapCard(int cardOffset, int deckOffset)
        {
            var old = this[deckOffset];
            this[deckOffset] = this[cardOffset + 8];
            this[cardOffset + 8] = old;
        }

        public void UpgradeCard(int classId, int instanceId)
        {
            var card = GetCard(classId, instanceId);
            var data = card.GetRarityData;
            var materialCount = data.UpgradeMaterialCount[card.Level];

            if (materialCount > card.Count) return;
            if (!Home.UseGold(data.UpgradeCost[card.Level])) return;

            card.Count -= materialCount;

            Home.AddExpPoints(data.UpgradeExp[card.Level]);

            card.Level++;
        }

        public void SawCard(int classId, int instanceId)
        {
            var card = GetCard(classId, instanceId);
            card.IsNew = false;
        }
    }
}