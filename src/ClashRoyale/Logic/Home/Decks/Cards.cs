using System;
using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic.Home.Decks.Items;

namespace ClashRoyale.Logic.Home.Decks
{
    public class Cards
    {
        private static SpellsOther[] _spellsOther;
        private static SpellsBuildings[] _spellsBuildings;
        private static SpellsCharacters[] _spellsCharacters;

        public static void Initialize()
        {
            _spellsOther = Csv.Tables.Get(Csv.Files.SpellsOther).GetDatas().Cast<SpellsOther>()
                .Where(s => !s.NotInUse).ToArray();
            
            _spellsBuildings = Csv.Tables.Get(Csv.Files.SpellsBuildings).GetDatas().Cast<SpellsBuildings>()
                .Where(s => !s.NotInUse).ToArray();

            _spellsCharacters = Csv.Tables.Get(Csv.Files.SpellsCharacters).GetDatas().Cast<SpellsCharacters>()
                .Where(s => !s.NotInUse).ToArray();
        }

        /// <summary>
        /// Returns all cards
        /// </summary>
        /// <returns></returns>
        public static Card[] GetAllCards()
        {
            var cards = _spellsCharacters.Select(data => new Card(26, data.GetInstanceId(), false)).ToList();

            cards.AddRange(_spellsBuildings.Select(data => new Card(27, data.GetInstanceId(), false)));
            cards.AddRange(_spellsOther.Select(data => new Card(28, data.GetInstanceId(), false)));

            return cards.ToArray();
        }

        public static Card RandomByArena(Card.Rarity rarity, List<string> chestArenas)
        {
            var random = new Random();
            var cards = new List<Card>();

            foreach (var chestArena in chestArenas)
            {
                if (_spellsCharacters.Any(x => x.UnlockArena == chestArena))
                    cards.AddRange(_spellsCharacters
                        .Where(x => x.UnlockArena == chestArena && x.Rarity == rarity.ToString())
                        .Select(data => new Card(26, data.GetInstanceId(), false)));
            }

            foreach (var chestArena in chestArenas)
            {
                if (_spellsOther.Any(x => x.UnlockArena == chestArena))
                    cards.AddRange(_spellsOther.Where(x => x.UnlockArena == chestArena && x.Rarity == rarity.ToString())
                        .Select(data => new Card(28, data.GetInstanceId(), false)));
            }

            if (rarity != Card.Rarity.Legendary)
            {
                foreach (var chestArena in chestArenas)
                {
                    if (_spellsBuildings.Any(x => x.UnlockArena == chestArena))
                        cards.AddRange(_spellsBuildings
                            .Where(x => x.UnlockArena == chestArena && x.Rarity == rarity.ToString())
                            .Select(data => new Card(27, data.GetInstanceId(), false)));
                }
            }

            //Console.WriteLine($"Found {cards.Count} cards for {chestArenas.Count} Arenas with rarity {rarity.ToString()}");

            return cards.Count > 0 ? cards.ElementAt(random.Next(0, cards.Count)) : null;
        }

        /// <summary>
        /// Random card by rarity
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public static Card Random(Card.Rarity rarity)
        {
            Card card = null;

            var random = new Random();
            var result = rarity == Card.Rarity.Legendary ? random.Next(1, 3) : random.Next(1, 4);

            switch (result)
            {
                case 1:
                {
                    var datas = _spellsCharacters.Where(s => s.Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsCharacters c)
                        card = new Card(26, c.GetInstanceId(), false);

                    break;
                }

                case 2:
                {
                    var datas = _spellsOther.Where(s => s.Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsOther c)
                        card = new Card(28, c.GetInstanceId(), false);

                    break;
                }

                case 3:
                {
                    var datas = _spellsBuildings.Where(s => s.Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsBuildings c)
                        card = new Card(27, c.GetInstanceId(), false);

                    break;
                }
            }

            return card;
        }

        /// <summary>
        /// Returns a random card
        /// </summary>
        /// <returns></returns>
        public static Card Random()
        {
            Card card = null;

            var random = new Random();

            switch (random.Next(26, 29))
            {
                case 26:
                {
                    if (_spellsCharacters.ElementAt(random.Next(0, _spellsCharacters.Length)) is SpellsCharacters c)
                        card = new Card(26, c.GetInstanceId(), false);

                    break;
                }

                case 27:
                {
                    if (_spellsBuildings.ElementAt(random.Next(0, _spellsBuildings.Length)) is SpellsBuildings c)
                        card = new Card(27, c.GetInstanceId(), false);

                    break;
                }

                case 28:
                {
                    if (_spellsOther.ElementAt(random.Next(0, _spellsOther.Length)) is SpellsOther c)
                        card = new Card(28, c.GetInstanceId(), false);

                    break;
                }
            }

            return card;
        }
    }
}