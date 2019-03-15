using System.Collections.Generic;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvHelpers
{
    public class DataTable
    {
        protected List<Data> Data;
        protected int Index;

        public DataTable()
        {
            Data = new List<Data>();
        }

        public DataTable(Table table, int index)
        {
            Index = index;
            Data = new List<Data>();

            for (var i = 0; i < table.GetRowCount(); i++)
            {
                var row = table.GetRowAt(i);
                var data = Create(row);

                Data.Add(data);
            }
        }

        public int Count()
        {
            if (Data != null)
                return Data.Count;
            return 0;
        }

        public Data Create(Row row)
        {
            switch (Index)
            {
                case 1:
                {
                    return new Abilities(row, this);
                }

                case 2:
                {
                    return new Achievements(row, this);
                }

                case 3:
                {
                    return new AllianceBadges(row, this);
                }

                case 4:
                {
                    return new AllianceRoles(row, this);
                }

                case 5:
                {
                    return new AreaEffectObjects(row, this);
                }

                case 6:
                {
                    return new Arenas(row, this);
                }

                case 7:
                {
                    return new Buildings(row, this);
                }

                case 8:
                {
                    return new CharacterBuffs(row, this);
                }

                case 9:
                {
                    return new Characters(row, this);
                }

                case 10:
                {
                    return new ChestOrder(row, this);
                }

                case 11:
                {
                    return new ConfigurationDefinitions(row, this);
                }

                case 12:
                {
                    return new ContentTests(row, this);
                }

                case 13:
                {
                    return new Decos(row, this);
                }

                case 14:
                {
                    return new DraftDeck(row, this);
                }

                case 15:
                {
                    return new EventCategories(row, this);
                }

                case 16:
                {
                    return new EventCategoryDefinitions(row, this);
                }

                case 17:
                {
                    return new EventCategoryEnums(row, this);
                }

                case 18:
                {
                    return new EventCategoryObjectDefinitions(row, this);
                }

                case 19:
                {
                    return new EventTargetingDefinitions(row, this);
                }

                case 20:
                {
                    return new ExpLevels(row, this);
                }

                case 21:
                {
                    return new GambleChests(row, this);
                }

                case 22:
                {
                    return new GameModes(row, this);
                }

                case 23:
                {
                    return new Globals(row, this);
                }

                case 24:
                {
                    return new Heroes(row, this);
                }

                case 25:
                {
                    return new Locales(row, this);
                }

                case 26:
                {
                    return new Locations(row, this);
                }

                case 27:
                {
                    return new Npcs(row, this);
                }

                case 28:
                {
                    return new PredefinedDecks(row, this);
                }

                case 29:
                {
                    return new Projectiles(row, this);
                }

                case 30:
                {
                    return new PveBoss(row, this);
                }

                case 31:
                {
                    return new PveGamemodes(row, this);
                }

                case 32:
                {
                    return new PveWaves(row, this);
                }

                case 33:
                {
                    return new QuestOrder(row, this);
                }

                case 34:
                {
                    return new Rarities(row, this);
                }

                case 35:
                {
                    return new Regions(row, this);
                }

                case 36:
                {
                    return new ResourcePacks(row, this);
                }

                case 37:
                {
                    return new CsvLogic.Resources(row, this);
                }

                case 38:
                {
                    return new Shop(row, this);
                }

                case 39:
                {
                    return new Skins(row, this);
                }

                case 40:
                {
                    return new SpellSets(row, this);
                }

                case 41:
                {
                    return new SpellsBuildings(row, this);
                }

                case 42:
                {
                    return new SpellsCharacters(row, this);
                }

                case 43:
                {
                    return new SpellsHeroes(row, this);
                }

                case 44:
                {
                    return new SpellsOther(row, this);
                }

                case 45:
                {
                    return new SurvivalModes(row, this);
                }

                case 46:
                {
                    return new Taunts(row, this);
                }

                case 47:
                {
                    return new TournamentTiers(row, this);
                }

                case 48:
                {
                    return new TreasureChests(row, this);
                }

                case 49:
                {
                    return new TutorialChestOrder(row, this);
                }

                case 50:
                {
                    return new TutorialsHome(row, this);
                }

                case 51:
                {
                    return new TutorialsNpc(row, this);
                }

                case 52:
                {
                    return new TveGamemodes(row, this);
                }

                default:
                {
                    return new Data(row, this);
                }
            }
        }

        public List<Data> GetDatas()
        {
            return Data;
        }

        public Data GetDataWithId(int id)
        {
            return Data[GlobalId.GetInstanceId(id)];
        }

        public T GetDataWithInstanceId<T>(int id) where T : Data
        {
            return Data[id] as T;
        }

        public T GetData<T>(string name) where T : Data
        {
            return Data.Find(data => data.GetName() == name) as T;
        }

        public int GetIndex()
        {
            return Index;
        }
    }
}