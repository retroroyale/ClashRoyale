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

        /// <summary>
        /// Add a card if we have it already in collection just add the ammount of material
        /// </summary>
        /// <param name="card"></param>
        public new void Add(Card card)
        {
            var index = FindIndex(c => c.ClassId == card.ClassId && c.InstanceId == card.InstanceId);

            if (index <= -1)
                base.Add(card);
            else
                this[index].Count += card.Count;
        }

        /// <summary>
        /// Encodes the whole collection
        /// </summary>
        /// <param name="packet"></param>
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

        /// <summary>
        /// Switch between 5 decks
        /// </summary>
        /// <param name="deckIndex"></param>
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

        /// <summary>
        /// Encodes this deck for a battle
        /// </summary>
        /// <param name="packet"></param>
        public void EncodeAttack(IByteBuffer packet)
        {
            foreach (var card in GetRange(0, 8))
                card.EncodeAttack(packet);
        }

        /// <summary>
        /// Get a card by it's class and instance id
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public Card GetCard(int classId, int instanceId)
        {
            var index = FindIndex(c => c.ClassId == classId && c.InstanceId == instanceId);
            return index > -1 ? this[index] : null;
        }

        /// <summary>
        /// Get a card by it's globalId
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public Card GetCard(int globalId)
        {
            var index = FindIndex(c => c.GlobalId == globalId);
            return index > -1 ? this[index] : null;
        }

        /// <summary>
        /// Returns the card offset in the collection
        /// </summary>
        /// <param name="globalId"></param>
        /// <returns></returns>
        public int GetCardOffset(int globalId)
        {
            var index = FindIndex(c => c.GlobalId == globalId);
            return index;
        }

        /// <summary>
        /// Swap cards in deck
        /// </summary>
        /// <param name="cardOffset"></param>
        /// <param name="deckOffset"></param>
        public void SwapCard(int cardOffset, int deckOffset)
        {
            var currentDeck = Home.Decks[Home.SelectedDeck];
            currentDeck[deckOffset] = this[cardOffset + 8].GlobalId;

            var old = this[deckOffset];
            this[deckOffset] = this[cardOffset + 8];
            this[cardOffset + 8] = old;
        }

        /// <summary>
        /// Upgrade all cards if an upgrade is available and enough gold
        /// </summary>
        public void UpgradeAll()
        {
            foreach (var card in this) UpgradeCard(card);
        }

        /// <summary>
        /// Upgrade a card by it's class and instance id
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="instanceId"></param>
        /// <param name="force"></param>
        public void UpgradeCard(int classId, int instanceId, bool force = false)
        {
            var card = GetCard(classId, instanceId);

            if (card != null)
                UpgradeCard(card, force);
        }

        /// <summary>
        /// Upgrade a card and check if enough cards and gold are available to use or force an upgrade
        /// </summary>
        /// <param name="card"></param>
        /// <param name="force"></param>
        public void UpgradeCard(Card card, bool force = false)
        {
            var data = card.GetRarityData;
            if (data == null) return;

            if (card.Level >= data.UpgradeMaterialCount.Length - 1) return;

            if (!force)
            {
                var materialCount = data.UpgradeMaterialCount[card.Level];

                if (materialCount > card.Count) return;
                if (!Home.UseGold(data.UpgradeCost[card.Level])) return;

                card.Count -= materialCount;
            }

            Home.AddExpPoints(data.UpgradeExp[card.Level]);
            card.Level++;
        }

        /// <summary>
        /// When a card is new and a player taps on it the first time in it's collection
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="instanceId"></param>
        public void SawCard(int classId, int instanceId)
        {
            var card = GetCard(classId, instanceId);
            card.IsNew = false;
        }
    }
}