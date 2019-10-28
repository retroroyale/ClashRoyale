using System;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic.Home.Chests.Items;
using ClashRoyale.Logic.Home.Decks;
using ClashRoyale.Logic.Home.Decks.Items;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home.Chests
{
    public class Chests
    {
        [JsonIgnore] public Home Home { get; set; }

        public Chest BuyChest(int instanceId, Chest.ChestType type)
        {
            var chests = Csv.Tables.Get(Csv.Files.TreasureChests);
            var mainchest = chests.GetDataWithInstanceId<TreasureChests>(instanceId);
            var baseChest = chests.GetData<TreasureChests>(mainchest.BaseChest);
            var chestArenas = Home.Arena.GetChestArenaNames();
            var random = new Random();

            var chest = new Chest
            {
                ChestId = instanceId,
                IsDraft = mainchest.DraftChest,
                Type = type
            };

            // Common
            {
                if (type == Chest.ChestType.Shop)
                {
                    for (var i = 0; i < random.Next(2, 5); i++)
                        if (random.Next(1, 2) == 1)
                        {
                            var card = Cards.RandomByArena(Card.Rarity.Common, chestArenas);
                            if (card == null) continue;

                            card.Count = random.Next(50, 100);
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
                else
                {
                    for (var i = 0; i < random.Next(2, 4); i++)
                        if (random.Next(1, 2) == 1)
                        {
                            var card = Cards.RandomByArena(Card.Rarity.Common, chestArenas);
                            if (card == null) continue;

                            card.Count = random.Next(15, 25);
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
            }

            // Rare
            {
                if (type == Chest.ChestType.Shop)
                {
                    for (var i = 0; i < random.Next(1, 4); i++)
                        if (random.Next(1, 2) == 1)
                        {
                            var card = Cards.RandomByArena(Card.Rarity.Rare, chestArenas);
                            if (card == null) continue;

                            card.Count = random.Next(15, 35);
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
                else
                {
                    for (var i = 0; i < random.Next(1, 2); i++)
                        if (random.Next(1, 4) == 1)
                        {
                            var card = Cards.RandomByArena(Card.Rarity.Rare, chestArenas);
                            if (card == null) continue;

                            card.Count = random.Next(8, 18);
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
            }

            // Epic
            {
                if (type == Chest.ChestType.Shop)
                {
                    for (var i = 0; i < random.Next(1, 2); i++)
                        if (random.Next(1, 3) == 1)
                        {
                            var card = Cards.RandomByArena(Card.Rarity.Epic, chestArenas);
                            if (card == null) continue;

                            card.Count = random.Next(2, 10);
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
                else
                {
                    if (random.Next(1, 20) == 1)
                    {
                        var card = Cards.RandomByArena(Card.Rarity.Epic, chestArenas);

                        if (card != null)
                        {
                            card.Count = random.Next(1, 5);
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                    }
                }
            }

            // Legendary
            {
                if (type == Chest.ChestType.Shop)
                {
                    if (random.Next(1, 8) == 1)
                    {
                        var card = Cards.RandomByArena(Card.Rarity.Legendary, chestArenas);

                        if (card != null)
                        {
                            card.Count = 1;
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                    }
                }
                else
                {
                    if (random.Next(1, 50) == 1)
                    {
                        var card = Cards.RandomByArena(Card.Rarity.Legendary, chestArenas);

                        if (card != null)
                        {
                            card.Count = 1;
                            card.IsNew = true;
                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                    }
                }
            }

            if (type == Chest.ChestType.Shop)
            {
                // TODO: Cost

                if (random.Next(1, 5) == 1) chest.Gems = random.Next(5, 15);
                if (random.Next(1, 4) == 1) chest.Gold = random.Next(100, 250);
            }
            else
            {
                if (random.Next(1, 10) == 1) chest.Gems = random.Next(1, 5);
                if (random.Next(1, 8) == 1) chest.Gold = random.Next(10, 75);
            }

            Home.Gold += chest.Gold;
            Home.Diamonds += chest.Gems;

            /*var price =
                ((baseChest.ShopPriceWithoutSpeedUp * Home.Arena.GetCurrentArenaData().ChestShopPriceMultiplier) / 100);

            Console.WriteLine(RoundPrice(price));*/

            return chest;
        }

        /// <summary>
        ///     by nameless
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        private int RoundPrice(int price)
        {
            if (price > 500)
                return 100 * ((price + 50) / 100);
            if (price > 100)
                return 10 * ((price + 5) / 10);
            if (price > 20)
                return 5 * ((price + 3) / 5);

            return price;
        }
    }
}