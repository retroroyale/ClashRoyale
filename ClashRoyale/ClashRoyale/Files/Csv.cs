using System;
using System.Collections.Generic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files
{
    public partial class Csv
    {
        public static readonly List<Tuple<string, Types>> Gamefiles = new List<Tuple<string, Types>>();
        public static Gamefiles Tables;

        public Csv()
        {
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/abilities.csv", Types.Abilities));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/achievements.csv", Types.Achievements));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/alliance_badges.csv", Types.AllianceBadges));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/alliance_roles.csv", Types.AllianceRoles));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/area_effect_objects.csv",
                Types.AreaEffectObjects));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/arenas.csv", Types.Arenas));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/buildings.csv", Types.Buildings));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/character_buffs.csv", Types.CharacterBuffs));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/characters.csv", Types.Characters));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/chest_order.csv", Types.ChestOrder));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/configuration_definitions.csv",
                Types.ConfigurationDefinitions));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/content_tests.csv", Types.ContentTests));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/decos.csv", Types.Decos));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/draft_deck.csv", Types.DraftDeck));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/event_categories.csv", Types.EventCategories));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/event_category_definitions.csv",
                Types.EventCategoryDefinitions));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/event_category_enums.csv",
                Types.EventCategoryEnums));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/event_category_object_definitions.csv",
                Types.EventCategoryObjectDefinitions));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/event_targeting_definitions.csv",
                Types.EventTargetingDefinitions));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/exp_levels.csv", Types.ExpLevels));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/gamble_chests.csv", Types.GambleChests));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/game_modes.csv", Types.GameModes));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/globals.csv", Types.Globals));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/heroes.csv", Types.Heroes));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/locales.csv", Types.Locales));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/locations.csv", Types.Locations));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/npcs.csv", Types.Npcs));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/predefined_decks.csv", Types.PredefinedDecks));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/projectiles.csv", Types.Projectiles));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/pve_boss.csv", Types.PveBoss));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/pve_gamemodes.csv", Types.PveGamemodes));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/pve_waves.csv", Types.PveWaves));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/quest_order.csv", Types.QuestOrder));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/rarities.csv", Types.Rarities));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/regions.csv", Types.Regions));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/resource_packs.csv", Types.ResourcePacks));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/resources.csv", Types.Resources));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/shop.csv", Types.Shop));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/skins.csv", Types.Skins));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/spell_sets.csv", Types.SpellSets));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/spells_buildings.csv", Types.SpellsBuildings));
            Gamefiles.Add(
                new Tuple<string, Types>("GameAssets/csv_logic/spells_characters.csv", Types.SpellsCharacters));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/spells_heroes.csv", Types.SpellsHeroes));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/spells_other.csv", Types.SpellsOther));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/survival_modes.csv", Types.SurvivalModes));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/taunts.csv", Types.Taunts));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/tournament_tiers.csv", Types.TournamentTiers));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/treasure_chests.csv", Types.TreasureChests));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/tutorial_chest_order.csv",
                Types.TutorialChestOrder));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/tutorials_home.csv", Types.TutorialsHome));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/tutorials_npc.csv", Types.TutorialsNpc));
            Gamefiles.Add(new Tuple<string, Types>("GameAssets/csv_logic/tve_gamemodes.csv", Types.TveGamemodes));

            Tables = new Gamefiles();

            foreach (var (item1, item2) in Gamefiles)
                Tables.Initialize(new Table(item1), item2);

            Logger.Log($"{Gamefiles.Count} Gamefiles loaded.", GetType());
        }
    }
}