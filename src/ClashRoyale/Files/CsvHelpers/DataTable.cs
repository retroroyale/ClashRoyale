using System.Collections.Generic;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvHelpers
{
    public class DataTable
    {
        public List<Data> Data;
        public Csv.Files Index;

        public DataTable()
        {
            Data = new List<Data>();
        }

        public DataTable(Table table, Csv.Files index)
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
            return Data?.Count ?? 0;
        }

        public Data Create(Row row)
        {
            switch (Index)
            {
                case Csv.Files.Abilities:
                {
                    return new Abilities(row, this);
                }

                case Csv.Files.Achievements:
                {
                    return new Achievements(row, this);
                }

                case Csv.Files.AllianceBadges:
                {
                    return new AllianceBadges(row, this);
                }

                case Csv.Files.AllianceRoles:
                {
                    return new AllianceRoles(row, this);
                }

                case Csv.Files.AreaEffectObjects:
                {
                    return new AreaEffectObjects(row, this);
                }

                case Csv.Files.Arenas:
                {
                    return new Arenas(row, this);
                }

                case Csv.Files.Buildings:
                {
                    return new Buildings(row, this);
                }

                case Csv.Files.CharacterBuffs:
                {
                    return new CharacterBuffs(row, this);
                }

                case Csv.Files.Characters:
                {
                    return new Characters(row, this);
                }

                case Csv.Files.ChestOrder:
                {
                    return new ChestOrder(row, this);
                }

                case Csv.Files.ConfigurationDefinitions:
                {
                    return new ConfigurationDefinitions(row, this);
                }

                case Csv.Files.ContentTests:
                {
                    return new ContentTests(row, this);
                }

                case Csv.Files.Decos:
                {
                    return new Decos(row, this);
                }

                case Csv.Files.DraftDeck:
                {
                    return new DraftDeck(row, this);
                }

                case Csv.Files.EventCategories:
                {
                    return new EventCategories(row, this);
                }

                case Csv.Files.EventCategoryDefinitions:
                {
                    return new EventCategoryDefinitions(row, this);
                }

                case Csv.Files.EventCategoryEnums:
                {
                    return new EventCategoryEnums(row, this);
                }

                case Csv.Files.EventCategoryObjectDefinitions:
                {
                    return new EventCategoryObjectDefinitions(row, this);
                }

                case Csv.Files.EventTargetingDefinitions:
                {
                    return new EventTargetingDefinitions(row, this);
                }

                case Csv.Files.ExpLevels:
                {
                    return new ExpLevels(row, this);
                }

                case Csv.Files.GambleChests:
                {
                    return new GambleChests(row, this);
                }

                case Csv.Files.GameModes:
                {
                    return new GameModes(row, this);
                }

                case Csv.Files.Globals:
                {
                    return new Globals(row, this);
                }

                case Csv.Files.Heroes:
                {
                    return new Heroes(row, this);
                }

                case Csv.Files.Locales:
                {
                    return new Locales(row, this);
                }

                case Csv.Files.Locations:
                {
                    return new Locations(row, this);
                }

                case Csv.Files.Npcs:
                {
                    return new Npcs(row, this);
                }

                case Csv.Files.PredefinedDecks:
                {
                    return new PredefinedDecks(row, this);
                }

                case Csv.Files.Projectiles:
                {
                    return new Projectiles(row, this);
                }

                case Csv.Files.PveBoss:
                {
                    return new PveBoss(row, this);
                }

                case Csv.Files.PveGamemodes:
                {
                    return new PveGamemodes(row, this);
                }

                case Csv.Files.PveWaves:
                {
                    return new PveWaves(row, this);
                }

                case Csv.Files.QuestOrder:
                {
                    return new QuestOrder(row, this);
                }

                case Csv.Files.Rarities:
                {
                    return new Rarities(row, this);
                }

                case Csv.Files.Regions:
                {
                    return new Regions(row, this);
                }

                case Csv.Files.ResourcePacks:
                {
                    return new ResourcePacks(row, this);
                }

                case Csv.Files.Resources:
                {
                    return new CsvLogic.Resources(row, this);
                }

                case Csv.Files.Shop:
                {
                    return new Shop(row, this);
                }

                case Csv.Files.Skins:
                {
                    return new Skins(row, this);
                }

                case Csv.Files.SpellSets:
                {
                    return new SpellSets(row, this);
                }

                case Csv.Files.SpellsBuildings:
                {
                    return new SpellsBuildings(row, this);
                }

                case Csv.Files.SpellsCharacters:
                {
                    return new SpellsCharacters(row, this);
                }

                case Csv.Files.SpellsHeroes:
                {
                    return new SpellsHeroes(row, this);
                }

                case Csv.Files.SpellsOther:
                {
                    return new SpellsOther(row, this);
                }

                case Csv.Files.SurvivalModes:
                {
                    return new SurvivalModes(row, this);
                }

                case Csv.Files.Taunts:
                {
                    return new Taunts(row, this);
                }

                case Csv.Files.TournamentTiers:
                {
                    return new TournamentTiers(row, this);
                }

                case Csv.Files.TreasureChests:
                {
                    return new TreasureChests(row, this);
                }

                case Csv.Files.TutorialChestOrder:
                {
                    return new TutorialChestOrder(row, this);
                }

                case Csv.Files.TutorialsHome:
                {
                    return new TutorialsHome(row, this);
                }

                case Csv.Files.TutorialsNpc:
                {
                    return new TutorialsNpc(row, this);
                }

                case Csv.Files.TveGamemodes:
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