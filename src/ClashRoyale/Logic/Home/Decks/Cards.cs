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
        public static Card[] GetAllCards()
        {
            var cards = new List<Card>();

            foreach (var data in Csv.Tables.Get(Csv.Files.SpellsCharacters).GetDatas())
                if (data is SpellsCharacters character)
                    if (!character.NotInUse)
                        cards.Add(new Card(26, data.GetInstanceId(), false));

            foreach (var data in Csv.Tables.Get(Csv.Files.SpellsBuildings).GetDatas())
                if (data is SpellsBuildings building)
                    if (!building.NotInUse)
                        cards.Add(new Card(27, data.GetInstanceId(), false));

            foreach (var data in Csv.Tables.Get(Csv.Files.SpellsOther).GetDatas())
                if (data is SpellsOther spell)
                    if (!spell.NotInUse)
                        cards.Add(new Card(28, data.GetInstanceId(), false));

            return cards.ToArray();
        }

        public static Card Random(Card.Rarity rarity)
        {
            Card card = null;

            var random = new Random();
            var result = rarity == Card.Rarity.Legendary ? random.Next(1, 3) : random.Next(1, 4);

            switch (result)
            {
                case 1:
                {
                    var datas = Csv.Tables.Get(Csv.Files.SpellsCharacters).GetDatas()
                        .Where(s => !((SpellsCharacters) s).NotInUse &&
                                    ((SpellsCharacters) s).Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsCharacters c)
                        card = new Card(26, c.GetInstanceId(), false);

                    break;
                }

                case 2:
                {
                    var datas = Csv.Tables.Get(Csv.Files.SpellsOther).GetDatas()
                        .Where(s => !((SpellsOther) s).NotInUse && ((SpellsOther) s).Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsOther c)
                        card = new Card(28, c.GetInstanceId(), false);

                    break;
                }

                case 3:
                {
                    var datas = Csv.Tables.Get(Csv.Files.SpellsBuildings).GetDatas()
                        .Where(s => !((SpellsBuildings) s).NotInUse &&
                                    ((SpellsBuildings) s).Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsBuildings c)
                        card = new Card(27, c.GetInstanceId(), false);

                    break;
                }
            }

            return card;
        }

        public static Card Random()
        {
            Card card = null;

            var random = new Random();

            switch (random.Next(26, 29))
            {
                case 26:
                {
                    var datas = Csv.Tables.Get(Csv.Files.SpellsCharacters).GetDatas()
                        .Where(s => !((SpellsCharacters) s).NotInUse);

                    if (datas.ElementAt(random.Next(0, datas.Count())) is SpellsCharacters c)
                        card = new Card(26, c.GetInstanceId(), false);

                    break;
                }

                case 27:
                {
                    var datas = Csv.Tables.Get(Csv.Files.SpellsBuildings).GetDatas()
                        .Where(s => !((SpellsBuildings) s).NotInUse);

                    if (datas.ElementAt(random.Next(0, datas.Count())) is SpellsBuildings c)
                        card = new Card(27, c.GetInstanceId(), false);

                    break;
                }

                case 28:
                {
                    var datas = Csv.Tables.Get(Csv.Files.SpellsOther).GetDatas()
                        .Where(s => !((SpellsOther) s).NotInUse);

                    if (datas.ElementAt(random.Next(0, datas.Count())) is SpellsOther c)
                        card = new Card(28, c.GetInstanceId(), false);

                    break;
                }
            }

            return card;
        }
    }
}