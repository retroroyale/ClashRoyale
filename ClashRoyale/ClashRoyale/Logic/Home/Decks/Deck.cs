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

            foreach (var card in Card.GetAllCards().Skip(8))
                Add(card);       
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

            packet.WriteVInt(-1);

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

        public void SwapCard(int collectionIndex, int deckIndex)
        {
            var old = this[deckIndex];
            this[deckIndex] = this[collectionIndex + 8];
            this[collectionIndex + 8] = old;
        }

        public void UpgradeCard(int classId, int instanceId)
        {
            var card = GetCard(classId, instanceId);
            var data = card.GetRarityData;

            if (Home.Gold < data.UpgradeCost[card.Level]) return;

            var materialCount = data.UpgradeMaterialCount[card.Level];

            if (materialCount > card.Count) return;

            card.Count -= materialCount;
            Home.Gold -= data.UpgradeCost[card.Level];

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