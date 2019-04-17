using System;
using System.Collections.Generic;
using ClashRoyale.Files.CsvHelpers;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files
{
    public partial class Csv
    {
        public enum Files
        {
            Abilities = 1,
            Achievements = 2,
            AllianceBadges = 3,
            AllianceRoles = 4,
            AreaEffectObjects = 5,
            Arenas = 6,
            Buildings = 7,
            CharacterBuffs = 8,
            Characters = 9,
            ChestOrder = 10,
            ConfigurationDefinitions = 11,
            ContentTests = 12,
            Decos = 13,
            DraftDeck = 14,
            EventCategories = 15,
            EventCategoryDefinitions = 16,
            EventCategoryEnums = 17,
            EventCategoryObjectDefinitions = 18,
            EventTargetingDefinitions = 19,
            ExpLevels = 20,
            GambleChests = 21,
            GameModes = 22,
            Globals = 23,
            Heroes = 24,
            Locales = 25,
            Locations = 26,
            Npcs = 27,
            PredefinedDecks = 28,
            Projectiles = 29,
            PveBoss = 30,
            PveGamemodes = 31,
            PveWaves = 32,
            QuestOrder = 33,
            Rarities = 34,
            Regions = 35,
            ResourcePacks = 36,
            Resources = 37,
            Shop = 38,
            Skins = 39,
            SpellSets = 40,
            SpellsBuildings = 41,
            SpellsCharacters = 42,
            SpellsHeroes = 43,
            SpellsOther = 44,
            SurvivalModes = 45,
            Taunts = 46,
            TournamentTiers = 47,
            TreasureChests = 48,
            TutorialChestOrder = 49,
            TutorialsHome = 50,
            TutorialsNpc = 51,
            TveGamemodes = 52
        }

        public static Dictionary<Files, Type> DataTypes = new Dictionary<Files, Type>();

        static Csv()
        {
            DataTypes.Add(Files.Abilities, typeof(Abilities));
            DataTypes.Add(Files.Achievements, typeof(Achievements));
            DataTypes.Add(Files.AllianceBadges, typeof(AllianceBadges));
            DataTypes.Add(Files.AllianceRoles, typeof(AllianceRoles));
            DataTypes.Add(Files.AreaEffectObjects, typeof(AreaEffectObjects));
            DataTypes.Add(Files.Arenas, typeof(Arenas));
            DataTypes.Add(Files.Buildings, typeof(Buildings));
            DataTypes.Add(Files.CharacterBuffs, typeof(CharacterBuffs));
            DataTypes.Add(Files.Characters, typeof(Characters));
            DataTypes.Add(Files.ChestOrder, typeof(ChestOrder));
            DataTypes.Add(Files.ConfigurationDefinitions, typeof(ConfigurationDefinitions));
            DataTypes.Add(Files.ContentTests, typeof(ContentTests));
            DataTypes.Add(Files.Decos, typeof(Decos));
            DataTypes.Add(Files.DraftDeck, typeof(DraftDeck));
            DataTypes.Add(Files.EventCategories, typeof(EventCategories));
            DataTypes.Add(Files.EventCategoryDefinitions, typeof(EventCategoryDefinitions));
            DataTypes.Add(Files.EventCategoryEnums, typeof(EventCategoryEnums));
            DataTypes.Add(Files.EventCategoryObjectDefinitions, typeof(EventCategoryObjectDefinitions));
            DataTypes.Add(Files.EventTargetingDefinitions, typeof(EventTargetingDefinitions));
            DataTypes.Add(Files.ExpLevels, typeof(ExpLevels));
            DataTypes.Add(Files.GambleChests, typeof(GambleChests));
            DataTypes.Add(Files.GameModes, typeof(GameModes));
            DataTypes.Add(Files.Globals, typeof(Globals));
            DataTypes.Add(Files.Heroes, typeof(Heroes));
            DataTypes.Add(Files.Locales, typeof(Locales));
            DataTypes.Add(Files.Locations, typeof(Locations));
            DataTypes.Add(Files.Npcs, typeof(Npcs));
            DataTypes.Add(Files.PredefinedDecks, typeof(PredefinedDecks));
            DataTypes.Add(Files.Projectiles, typeof(Projectiles));
            DataTypes.Add(Files.PveBoss, typeof(PveBoss));
            DataTypes.Add(Files.PveGamemodes, typeof(PveGamemodes));
            DataTypes.Add(Files.PveWaves, typeof(PveWaves));
            DataTypes.Add(Files.QuestOrder, typeof(QuestOrder));
            DataTypes.Add(Files.Rarities, typeof(Rarities));
            DataTypes.Add(Files.Regions, typeof(Regions));
            DataTypes.Add(Files.ResourcePacks, typeof(ResourcePacks));
            DataTypes.Add(Files.Resources, typeof(CsvLogic.Resources));
            DataTypes.Add(Files.Shop, typeof(Shop));
            DataTypes.Add(Files.Skins, typeof(Skins));
            DataTypes.Add(Files.SpellSets, typeof(SpellSets));
            DataTypes.Add(Files.SpellsBuildings, typeof(SpellsBuildings));
            DataTypes.Add(Files.SpellsCharacters, typeof(SpellsCharacters));
            DataTypes.Add(Files.SpellsHeroes, typeof(SpellsHeroes));
            DataTypes.Add(Files.SpellsOther, typeof(SpellsOther));
            DataTypes.Add(Files.SurvivalModes, typeof(SurvivalModes));
            DataTypes.Add(Files.Taunts, typeof(Taunts));
            DataTypes.Add(Files.TournamentTiers, typeof(TournamentTiers));
            DataTypes.Add(Files.TreasureChests, typeof(TreasureChests));
            DataTypes.Add(Files.TutorialChestOrder, typeof(TutorialChestOrder));
            DataTypes.Add(Files.TutorialsHome, typeof(TutorialsHome));
            DataTypes.Add(Files.TutorialsNpc, typeof(TutorialsNpc));
            DataTypes.Add(Files.TveGamemodes, typeof(TveGamemodes));
        }

        public static Data Create(Files file, Row row, DataTable dataTable)
        {
            if (DataTypes.ContainsKey(file)) return Activator.CreateInstance(DataTypes[file], row, dataTable) as Data;

            return null;
        }
    }
}