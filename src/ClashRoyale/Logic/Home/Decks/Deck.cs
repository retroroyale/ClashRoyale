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

            var currentDeck = Home.Decks[Home.SelectedDeck];

            for (var i = 0; i < 8; i++)
            {
                var card = this[i];
                var pos = Array.FindIndex(currentDeck, c => c == card.GlobalId);

                if (pos == -1)
                {
                    pos = FindIndex(c => c.GlobalId == card.GlobalId); 

                    // Card in deck
                    var old = currentDeck[i];
                    currentDeck[i] = card.GlobalId;

                    // Cards in deck from collection
                    var oldCard = FindIndex(c => c.GlobalId == old);
                    this[pos] = this[oldCard];
                    this[oldCard] = card;
                }
                else
                {
                    var c = currentDeck[i];
                    currentDeck[i] = currentDeck[pos];
                    currentDeck[pos] = c;
                }
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

        public void SwapCard(int cardOffset, int deckOffset, int deck = -1)
        {
            var currentDeck = Home.Decks[deck == -1 ? Home.SelectedDeck : deck];
            currentDeck[deckOffset] = this[cardOffset + 8].GlobalId;

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