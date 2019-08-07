using System;
using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Logic.Home.Decks.Items;
using ClashRoyale.Utilities.Netty;
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
            {
                var card = new Card(26, i, false);
                Add(card);
                foreach (var deck in Home.Decks) deck[i] = card.GlobalId;
            }
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
            packet.WriteVInt(Home.Decks.Length); // DeckCount

            foreach (var deck in Home.Decks)
            {
                packet.WriteVInt(deck.Length);

                foreach (var globalId in deck)
                    packet.WriteVInt(globalId);
            }

            packet.WriteByte(255);

            foreach (var card in GetRange(0, 8))
                card.Encode(packet);

            packet.WriteVInt(Count - 8);

            foreach (var card in this.Skip(8))
                card.Encode(packet);

            packet.WriteVInt(Home.SelectedDeck); // CurrentSlot
        }

        public void SwitchDeck(int deckIndex)
        {
            if (deckIndex > 4) return;

            for (var i = 0; i < Home.Decks[deckIndex].Length; i++)
            {
                var card = Home.Decks[deckIndex][i];
                var newDeckCard = GetCard(card);
                var oldDeckCard = this[i];

                var newOldCardIndex = IndexOf(newDeckCard);

                this[newOldCardIndex] = oldDeckCard;
                this[i] = newDeckCard;
            }

            Home.SelectedDeck = deckIndex;
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

        public Card GetCard(int globalId)
        {
            var index = FindIndex(c => c.GlobalId == globalId);
            return index > -1 ? this[index] : null;
        }

        public int GetCardOffset(int globalId)
        {
            var index = FindIndex(c => c.GlobalId == globalId);
            return index;
        }

        public void SwapCard(int cardOffset, int deckOffset)
        {
            var currentDeck = Home.Decks[Home.SelectedDeck];
            currentDeck[deckOffset] = this[cardOffset + 8].GlobalId;

            var old = this[deckOffset];
            this[deckOffset] = this[cardOffset + 8];
            this[cardOffset + 8] = old;
        }

        public void UpgradeAll()
        {
            foreach (var card in this) UpgradeCard(card);
        }

        public void UpgradeCard(int classId, int instanceId)
        {
            var card = GetCard(classId, instanceId);
            UpgradeCard(card);
        }

        public void UpgradeCard(Card card)
        {
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