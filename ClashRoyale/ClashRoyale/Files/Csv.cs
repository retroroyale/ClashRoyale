using System;
using System.Collections.Generic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files
{
    public class Csv
    {
        public enum Types
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

        public static readonly List<Tuple<string, int>> Gamefiles = new List<Tuple<string, int>>();
        public static Gamefiles Tables;

        public Csv()
        {
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/abilities.csv", 1));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/achievements.csv", 2));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/alliance_badges.csv", 3));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/alliance_roles.csv", 4));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/area_effect_objects.csv", 5));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/arenas.csv", 6));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/buildings.csv", 7));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/character_buffs.csv", 8));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/characters.csv", 9));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/chest_order.csv", 10));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/configuration_definitions.csv", 11));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/content_tests.csv", 12));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/decos.csv", 13));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/draft_deck.csv", 14));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/event_categories.csv", 15));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/event_category_definitions.csv", 16));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/event_category_enums.csv", 17));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/event_category_object_definitions.csv", 18));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/event_targeting_definitions.csv", 19));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/exp_levels.csv", 20));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/gamble_chests.csv", 21));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/game_modes.csv", 22));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/globals.csv", 23));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/heroes.csv", 24));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/locales.csv", 25));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/locations.csv", 26));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/npcs.csv", 27));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/predefined_decks.csv", 28));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/projectiles.csv", 29));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/pve_boss.csv", 30));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/pve_gamemodes.csv", 31));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/pve_waves.csv", 32));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/quest_order.csv", 33));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/rarities.csv", 34));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/regions.csv", 35));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/resource_packs.csv", 36));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/resources.csv", 37));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/shop.csv", 38));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/skins.csv", 39));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/spell_sets.csv", 40));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/spells_buildings.csv", 41));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/spells_characters.csv", 42));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/spells_heroes.csv", 43));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/spells_other.csv", 44));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/survival_modes.csv", 45));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/taunts.csv", 46));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/tournament_tiers.csv", 47));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/treasure_chests.csv", 48));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/tutorial_chest_order.csv", 49));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/tutorials_home.csv", 50));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/tutorials_npc.csv", 51));
            Gamefiles.Add(new Tuple<string, int>("GameAssets/csv_logic/tve_gamemodes.csv", 52));

            Tables = new Gamefiles();

            foreach (var file in Gamefiles)
                Tables.Initialize(new Table(file.Item1), file.Item2);

            Logger.Log($"Succesfully loaded {Gamefiles.Count} Gamefiles into memory.", GetType());
        }
    }
}