using System;
using System.Collections.Generic;
using System.Linq;
using ClashRoyale.Extensions;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home.Decks.Items
{
    public class Card
    {
        public enum Rarity
        {
            Common = 0,
            Rare = 1,
            Epic = 2,
            Legendary = 3,
            Hero = 4
        }

        /// <summary>
        /// Create a new card with given values
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="instanceId"></param>
        /// <param name="isNew"></param>
        /// <param name="count"></param>
        public Card(int classId, int instanceId, bool isNew, int count = 0)
        {
            ClassId = classId;
            InstanceId = instanceId;
            IsNew = isNew;
            Count = count;
        }

        /// <summary>
        /// Copy values from a different class to this new instance
        /// </summary>
        /// <param name="card"></param>
        public Card(Card card)
        {
            Count = card.Count;
            InstanceId = card.InstanceId;
            ClassId = card.ClassId;
            Level = card.Level;
            IsNew = card.IsNew;
        }

        public Card()
        {
            // Json.
        }

        public int Count { get; set; }
        public int InstanceId { get; set; }
        public int ClassId { get; set; }
        public int Level { get; set; }
        public bool IsNew { get; set; }

        [JsonIgnore] public int GlobalId => ClassId * 1000000 + InstanceId;

        [JsonIgnore] public int CardId => Id(ClassId, InstanceId);

        [JsonIgnore] public Rarity CardRarity => GetRarity(GetRarityData.Name);

        [JsonIgnore]
        public Rarities GetRarityData
        {
            get
            {
                switch (ClassId)
                {
                    case 26:
                    {
                        var data = Csv.Tables.Get(Csv.Types.SpellsCharacters)
                            .GetDataWithInstanceId<SpellsCharacters>(InstanceId);
                        return Csv.Tables.Get(Csv.Types.Rarities).GetData<Rarities>(data.Rarity);
                    }

                    case 27:
                    {
                        var data = Csv.Tables.Get(Csv.Types.SpellsBuildings)
                            .GetDataWithInstanceId<SpellsBuildings>(InstanceId);
                        return Csv.Tables.Get(Csv.Types.Rarities).GetData<Rarities>(data.Rarity);
                    }

                    case 28:
                    {
                        var data = Csv.Tables.Get(Csv.Types.SpellsOther).GetDataWithInstanceId<SpellsOther>(InstanceId);
                        return Csv.Tables.Get(Csv.Types.Rarities).GetData<Rarities>(data.Rarity);
                    }
                }

                return null;
            }
        }

        public void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(CardId);
            packet.WriteVInt(Level);
            packet.WriteVInt(0);
            packet.WriteVInt(Count);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(IsNew ? 2 : 0);
        }

        public void EncodeAttack(IByteBuffer packet)
        {
            packet.WriteVInt(CardId);
            packet.WriteVInt(Level);
        }

        public static int Id(int classId, int instanceId)
        {
            switch (classId)
            {
                case 27:
                {
                    instanceId += 58;
                    break;
                }

                case 28:
                {
                    instanceId += 72;
                    break;
                }
            }

            return instanceId + 1;
        }

        public static Card[] GetAllCards()
        {
            var cards = new List<Card>();

            foreach (var data in Csv.Tables.Get(Csv.Types.SpellsCharacters).GetDatas())
                if (data is SpellsCharacters character)
                    if (!character.NotInUse)
                        cards.Add(new Card(26, data.GetInstanceId(), false));

            foreach (var data in Csv.Tables.Get(Csv.Types.SpellsBuildings).GetDatas())
                if (data is SpellsBuildings building)
                    if (!building.NotInUse)
                        cards.Add(new Card(27, data.GetInstanceId(), false));

            foreach (var data in Csv.Tables.Get(Csv.Types.SpellsOther).GetDatas())
                if (data is SpellsOther spell)
                    if (!spell.NotInUse)
                        cards.Add(new Card(28, data.GetInstanceId(), false));

            return cards.ToArray();
        }

        public static Card Random()
        {
            Card card = null;

            var random = new Random();

            switch (random.Next(26, 29))
            {
                case 26:
                {
                    var datas = Csv.Tables.Get(Csv.Types.SpellsCharacters).GetDatas()
                        .Where(s => !((SpellsCharacters) s).NotInUse);

                    if (datas.ElementAt(random.Next(0, datas.Count())) is SpellsCharacters c)
                        card = new Card(26, c.GetInstanceId(), false);

                    break;
                }

                case 27:
                {
                    var datas = Csv.Tables.Get(Csv.Types.SpellsBuildings).GetDatas()
                        .Where(s => !((SpellsBuildings) s).NotInUse);

                    if (datas.ElementAt(random.Next(0, datas.Count())) is SpellsBuildings c)
                        card = new Card(27, c.GetInstanceId(), false);

                    break;
                }

                case 28:
                {
                    var datas = Csv.Tables.Get(Csv.Types.SpellsOther).GetDatas()
                        .Where(s => !((SpellsOther) s).NotInUse);

                    if (datas.ElementAt(random.Next(0, datas.Count())) is SpellsOther c)
                        card = new Card(28, c.GetInstanceId(), false);

                    break;
                }
            }

            return card;
        }

        public static Card Random(Rarity rarity)
        {
            Card card = null;

            var random = new Random();
            var result = rarity == Rarity.Legendary ? random.Next(1, 3) : random.Next(1, 4);

            switch (result)
            {
                case 1:
                {
                    var datas = Csv.Tables.Get(Csv.Types.SpellsCharacters).GetDatas()
                        .Where(s => !((SpellsCharacters) s).NotInUse &&
                                    ((SpellsCharacters) s).Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsCharacters c)
                        card = new Card(26, c.GetInstanceId(), false);

                    break;
                }

                case 2:
                {
                    var datas = Csv.Tables.Get(Csv.Types.SpellsOther).GetDatas()
                        .Where(s => !((SpellsOther) s).NotInUse && ((SpellsOther) s).Rarity == rarity.ToString());

                    var enumerable = datas.ToList();
                    if (enumerable.ElementAt(random.Next(0, enumerable.Count)) is SpellsOther c)
                        card = new Card(28, c.GetInstanceId(), false);

                    break;
                }

                case 3:
                {
                    var datas = Csv.Tables.Get(Csv.Types.SpellsBuildings).GetDatas()
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

        public static Rarity GetRarity(string name)
        {
            switch (name)
            {
                case "Common":
                    return Rarity.Common;
                case "Rare":
                    return Rarity.Rare;
                case "Epic":
                    return Rarity.Epic;
                case "Legendary":
                    return Rarity.Legendary;
            }

            return Rarity.Common;
        }
    }
}