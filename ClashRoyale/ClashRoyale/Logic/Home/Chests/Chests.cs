using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home.Chests
{
    public class Chests
    {
        [JsonIgnore] public Home Home { get; set; }

        /*public Chest BuyChest(int instanceId, Chest.ChestType type)
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

            for (var i = 0; i < baseChest.RandomSpells / 2; i++)
            {
                var card = Card.Random(Card.Rarity.Common);
                card.Count = random.Next(40, 60);

                chest.Add(card);
            }

            for (var i = 0; i < baseChest.RandomSpells / 3; i++)
            {
                var card = Card.Random(Card.Rarity.Rare);
                card.Count = random.Next(5, 10);

                chest.Add(card);
            }

            // Epic
            { 
                var card = Card.Random(Card.Rarity.Epic);
                card.Count = random.Next(1, 5);

                chest.Add(card);
            }

            // Legendary
            {
                var card = Card.Random(Card.Rarity.Legendary);
                card.Count = 1;

                chest.Add(card);
            }

            return chest;
        }*/
    }
}