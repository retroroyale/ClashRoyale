using System.Collections.Generic;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvHelpers
{
    public class DataTable
    {
        protected List<Data> Data;
        protected Csv.Types Index;

        public DataTable()
        {
            Data = new List<Data>();
        }

        public DataTable(Table table, Csv.Types index)
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
            switch ((Csv.Types)Index)
            {
                case Csv.Types.Abilities:
                {
                    return new Abilities(row, this);
                }

                case Csv.Types.Achievements:
                {
                    return new Achievements(row, this);
                }

                case Csv.Types.AllianceBadges:
                {
                    return new AllianceBadges(row, this);
                }

                case Csv.Types.AllianceRoles:
                {
                    return new AllianceRoles(row, this);
                }

                case Csv.Types.AreaEffectObjects:
                {
                    return new AreaEffectObjects(row, this);
                }

                case Csv.Types.Arenas:
                {
                    return new Arenas(row, this);
                }

                case Csv.Types.Buildings:
                {
                    return new Buildings(row, this);
                }

                case Csv.Types.CharacterBuffs:
                {
                    return new CharacterBuffs(row, this);
                }

                case Csv.Types.Characters:
                {
                    return new Characters(row, this);
                }

                case Csv.Types.ChestOrder:
                {
                    return new ChestOrder(row, this);
                }

                case Csv.Types.ConfigurationDefinitions:
                {
                    return new ConfigurationDefinitions(row, this);
                }

                case Csv.Types.ContentTests:
                {
                    return new ContentTests(row, this);
                }

                case Csv.Types.Decos:
                {
                    return new Decos(row, this);
                }

                case Csv.Types.DraftDeck:
                {
                    return new DraftDeck(row, this);
                }

                case Csv.Types.EventCategories:
                {
                    return new EventCategories(row, this);
                }

                case Csv.Types.EventCategoryDefinitions:
                {
                    return new EventCategoryDefinitions(row, this);
                }

                case Csv.Types.EventCategoryEnums:
                {
                    return new EventCategoryEnums(row, this);
                }

                case Csv.Types.EventCategoryObjectDefinitions:
                {
                    return new EventCategoryObjectDefinitions(row, this);
                }

                case Csv.Types.EventTargetingDefinitions:
                {
                    return new EventTargetingDefinitions(row, this);
                }

                case Csv.Types.ExpLevels:
                {
                    return new ExpLevels(row, this);
                }

                case Csv.Types.GambleChests:
                {
                    return new GambleChests(row, this);
                }

                case Csv.Types.GameModes:
                {
                    return new GameModes(row, this);
                }

                case Csv.Types.Globals:
                {
                    return new Globals(row, this);
                }

                case Csv.Types.Heroes:
                {
                    return new Heroes(row, this);
                }

                case Csv.Types.Locales:
                {
                    return new Locales(row, this);
                }

                case Csv.Types.Locations:
                {
                    return new Locations(row, this);
                }

                case Csv.Types.Npcs:
                {
                    return new Npcs(row, this);
                }

                case Csv.Types.PredefinedDecks:
                {
                    return new PredefinedDecks(row, this);
                }

                case Csv.Types.Projectiles:
                {
                    return new Projectiles(row, this);
                }

                case Csv.Types.PveBoss:
                {
                    return new PveBoss(row, this);
                }

                case Csv.Types.PveGamemodes:
                {
                    return new PveGamemodes(row, this);
                }

                case Csv.Types.PveWaves:
                {
                    return new PveWaves(row, this);
                }

                case Csv.Types.QuestOrder:
                {
                    return new QuestOrder(row, this);
                }

                case Csv.Types.Rarities:
                {
                    return new Rarities(row, this);
                }

                case Csv.Types.Regions:
                {
                    return new Regions(row, this);
                }

                case Csv.Types.ResourcePacks:
                {
                    return new ResourcePacks(row, this);
                }

                case Csv.Types.Resources:
                {
                    return new CsvLogic.Resources(row, this);
                }

                case Csv.Types.Shop:
                {
                    return new Shop(row, this);
                }

                case Csv.Types.Skins:
                {
                    return new Skins(row, this);
                }

                case Csv.Types.SpellSets:
                {
                    return new SpellSets(row, this);
                }

                case Csv.Types.SpellsBuildings:
                {
                    return new SpellsBuildings(row, this);
                }

                case Csv.Types.SpellsCharacters:
                {
                    return new SpellsCharacters(row, this);
                }

                case Csv.Types.SpellsHeroes:
                {
                    return new SpellsHeroes(row, this);
                }

                case Csv.Types.SpellsOther:
                {
                    return new SpellsOther(row, this);
                }

                case Csv.Types.SurvivalModes:
                {
                    return new SurvivalModes(row, this);
                }

                case Csv.Types.Taunts:
                {
                    return new Taunts(row, this);
                }

                case Csv.Types.TournamentTiers:
                {
                    return new TournamentTiers(row, this);
                }

                case Csv.Types.TreasureChests:
                {
                    return new TreasureChests(row, this);
                }

                case Csv.Types.TutorialChestOrder:
                {
                    return new TutorialChestOrder(row, this);
                }

                case Csv.Types.TutorialsHome:
                {
                    return new TutorialsHome(row, this);
                }

                case Csv.Types.TutorialsNpc:
                {
                    return new TutorialsNpc(row, this);
                }

                case Csv.Types.TveGamemodes:
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
            return (int)Index;
        }
    }
}