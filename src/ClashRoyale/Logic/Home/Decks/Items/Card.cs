using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Utilities.Netty;
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
        ///     Create a new card with given values
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
        ///     Copy values from a different class to this new instance
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
                        var data = Csv.Tables.Get(Csv.Files.SpellsCharacters)
                            .GetDataWithInstanceId<SpellsCharacters>(InstanceId);
                        return Csv.Tables.Get(Csv.Files.Rarities).GetData<Rarities>(data.Rarity);
                    }

                    case 27:
                    {
                        var data = Csv.Tables.Get(Csv.Files.SpellsBuildings)
                            .GetDataWithInstanceId<SpellsBuildings>(InstanceId);
                        return Csv.Tables.Get(Csv.Files.Rarities).GetData<Rarities>(data.Rarity);
                    }

                    case 28:
                    {
                        var data = Csv.Tables.Get(Csv.Files.SpellsOther).GetDataWithInstanceId<SpellsOther>(InstanceId);
                        return Csv.Tables.Get(Csv.Files.Rarities).GetData<Rarities>(data.Rarity);
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
            if (classId >= 27) instanceId += Csv.Tables.Get(Csv.Files.SpellsCharacters).Count();
            if (classId == 28) instanceId += Csv.Tables.Get(Csv.Files.SpellsBuildings).Count();

            return instanceId + 1;
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
                default:
                    return Rarity.Common;
            }
        }
    }
}