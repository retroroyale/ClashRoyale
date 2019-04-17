using System;
using System.Collections.Generic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files
{
    public partial class Csv
    {
        public static readonly List<Tuple<string, Files>> Gamefiles = new List<Tuple<string, Files>>();
        public static Gamefiles Tables;

        public Csv()
        {
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/abilities.csv", Files.Abilities));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/achievements.csv", Files.Achievements));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/alliance_badges.csv", Files.AllianceBadges));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/alliance_roles.csv", Files.AllianceRoles));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/area_effect_objects.csv",
                Files.AreaEffectObjects));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/arenas.csv", Files.Arenas));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/buildings.csv", Files.Buildings));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/character_buffs.csv", Files.CharacterBuffs));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/characters.csv", Files.Characters));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/chest_order.csv", Files.ChestOrder));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/configuration_definitions.csv",
                Files.ConfigurationDefinitions));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/content_tests.csv", Files.ContentTests));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/decos.csv", Files.Decos));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/draft_deck.csv", Files.DraftDeck));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/event_categories.csv", Files.EventCategories));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/event_category_definitions.csv",
                Files.EventCategoryDefinitions));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/event_category_enums.csv",
                Files.EventCategoryEnums));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/event_category_object_definitions.csv",
                Files.EventCategoryObjectDefinitions));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/event_targeting_definitions.csv",
                Files.EventTargetingDefinitions));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/exp_levels.csv", Files.ExpLevels));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/gamble_chests.csv", Files.GambleChests));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/game_modes.csv", Files.GameModes));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/globals.csv", Files.Globals));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/heroes.csv", Files.Heroes));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/locales.csv", Files.Locales));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/locations.csv", Files.Locations));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/npcs.csv", Files.Npcs));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/predefined_decks.csv", Files.PredefinedDecks));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/projectiles.csv", Files.Projectiles));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/pve_boss.csv", Files.PveBoss));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/pve_gamemodes.csv", Files.PveGamemodes));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/pve_waves.csv", Files.PveWaves));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/quest_order.csv", Files.QuestOrder));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/rarities.csv", Files.Rarities));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/regions.csv", Files.Regions));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/resource_packs.csv", Files.ResourcePacks));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/resources.csv", Files.Resources));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/shop.csv", Files.Shop));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/skins.csv", Files.Skins));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/spell_sets.csv", Files.SpellSets));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/spells_buildings.csv", Files.SpellsBuildings));
            Gamefiles.Add(
                new Tuple<string, Files>("GameAssets/csv_logic/spells_characters.csv", Files.SpellsCharacters));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/spells_heroes.csv", Files.SpellsHeroes));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/spells_other.csv", Files.SpellsOther));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/survival_modes.csv", Files.SurvivalModes));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/taunts.csv", Files.Taunts));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/tournament_tiers.csv", Files.TournamentTiers));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/treasure_chests.csv", Files.TreasureChests));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/tutorial_chest_order.csv",
                Files.TutorialChestOrder));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/tutorials_home.csv", Files.TutorialsHome));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/tutorials_npc.csv", Files.TutorialsNpc));
            Gamefiles.Add(new Tuple<string, Files>("GameAssets/csv_logic/tve_gamemodes.csv", Files.TveGamemodes));

            Tables = new Gamefiles();

            foreach (var (item1, item2) in Gamefiles)
                Tables.Initialize(new Table(item1), item2);

            Logger.Log($"{Gamefiles.Count} Gamefiles loaded.", GetType());
        }
    }
}