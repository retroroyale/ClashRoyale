using System;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic.Home.Chests.Items;
using ClashRoyale.Logic.Home.Decks.Items;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home.Chests
{
    public class Chests
    {
        [JsonIgnore] public Home Home { get; set; }

        public Chest BuyChest(int instanceId, Chest.ChestType type)
        {
            var chests = Csv.Tables.Get(Csv.Types.TreasureChests);
            var mainchest = chests.GetDataWithInstanceId<TreasureChests>(instanceId);
            var baseChest = chests.GetData<TreasureChests>(mainchest.BaseChest);
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
                            var card = Card.Random(Card.Rarity.Common);
                            card.Count = random.Next(40, 60);

                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
                else
                {
                    for (var i = 0; i < random.Next(2, 4); i++)
                        if (random.Next(1, 2) == 1)
                        {
                            var card = Card.Random(Card.Rarity.Common);
                            card.Count = random.Next(10, 20);

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
                            var card = Card.Random(Card.Rarity.Rare);
                            card.Count = random.Next(10, 30);

                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
                else
                {
                    for (var i = 0; i < random.Next(1, 2); i++)
                        if (random.Next(1, 4) == 1)
                        {
                            var card = Card.Random(Card.Rarity.Rare);
                            card.Count = random.Next(5, 15);

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
                            var card = Card.Random(Card.Rarity.Epic);
                            card.Count = random.Next(1, 8);

                            chest.Add(card);
                            Home.Deck.Add(card);
                        }
                }
                else
                {
                    if (random.Next(1, 20) == 1)
                    {
                        var card = Card.Random(Card.Rarity.Epic);
                        card.Count = random.Next(1, 5);

                        chest.Add(card);
                        Home.Deck.Add(card);
                    }
                }
            }

            // Legendary
            {
                if (type == Chest.ChestType.Shop)
                {
                    if (random.Next(1, 10) == 1)
                    {
                        var card = Card.Random(Card.Rarity.Legendary);
                        card.Count = 1;

                        chest.Add(card);
                        Home.Deck.Add(card);
                    }
                }
                else
                {
                    if (random.Next(1, 50) == 1)
                    {
                        var card = Card.Random(Card.Rarity.Legendary);
                        card.Count = 1;

                        chest.Add(card);
                        Home.Deck.Add(card);
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

            return chest;
        }
    }
}