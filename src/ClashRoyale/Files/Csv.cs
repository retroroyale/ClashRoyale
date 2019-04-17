using System.Collections.Generic;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files
{
    public partial class Csv
    {
        public static readonly List<string> Gamefiles = new List<string>();
        public static Gamefiles Tables;

        public Csv()
        {
            Gamefiles.Add("GameAssets/csv_logic/abilities.csv");
            Gamefiles.Add("GameAssets/csv_logic/achievements.csv");
            Gamefiles.Add("GameAssets/csv_logic/alliance_badges.csv");
            Gamefiles.Add("GameAssets/csv_logic/alliance_roles.csv");
            Gamefiles.Add("GameAssets/csv_logic/area_effect_objects.csv");
            Gamefiles.Add("GameAssets/csv_logic/arenas.csv");
            Gamefiles.Add("GameAssets/csv_logic/buildings.csv");
            Gamefiles.Add("GameAssets/csv_logic/character_buffs.csv");
            Gamefiles.Add("GameAssets/csv_logic/characters.csv");
            Gamefiles.Add("GameAssets/csv_logic/chest_order.csv");
            Gamefiles.Add("GameAssets/csv_logic/configuration_definitions.csv");
            Gamefiles.Add("GameAssets/csv_logic/content_tests.csv");
            Gamefiles.Add("GameAssets/csv_logic/decos.csv");
            Gamefiles.Add("GameAssets/csv_logic/draft_deck.csv");
            Gamefiles.Add("GameAssets/csv_logic/event_categories.csv");
            Gamefiles.Add("GameAssets/csv_logic/event_category_definitions.csv");
            Gamefiles.Add("GameAssets/csv_logic/event_category_enums.csv");
            Gamefiles.Add("GameAssets/csv_logic/event_category_object_definitions.csv");
            Gamefiles.Add("GameAssets/csv_logic/event_targeting_definitions.csv");
            Gamefiles.Add("GameAssets/csv_logic/exp_levels.csv");
            Gamefiles.Add("GameAssets/csv_logic/gamble_chests.csv");
            Gamefiles.Add("GameAssets/csv_logic/game_modes.csv");
            Gamefiles.Add("GameAssets/csv_logic/globals.csv");
            Gamefiles.Add("GameAssets/csv_logic/heroes.csv");
            Gamefiles.Add("GameAssets/csv_logic/locales.csv");
            Gamefiles.Add("GameAssets/csv_logic/locations.csv");
            Gamefiles.Add("GameAssets/csv_logic/npcs.csv");
            Gamefiles.Add("GameAssets/csv_logic/predefined_decks.csv");
            Gamefiles.Add("GameAssets/csv_logic/projectiles.csv");
            Gamefiles.Add("GameAssets/csv_logic/pve_boss.csv");
            Gamefiles.Add("GameAssets/csv_logic/pve_gamemodes.csv");
            Gamefiles.Add("GameAssets/csv_logic/pve_waves.csv");
            Gamefiles.Add("GameAssets/csv_logic/quest_order.csv");
            Gamefiles.Add("GameAssets/csv_logic/rarities.csv");
            Gamefiles.Add("GameAssets/csv_logic/regions.csv");
            Gamefiles.Add("GameAssets/csv_logic/resource_packs.csv");
            Gamefiles.Add("GameAssets/csv_logic/resources.csv");
            Gamefiles.Add("GameAssets/csv_logic/shop.csv");
            Gamefiles.Add("GameAssets/csv_logic/skins.csv");
            Gamefiles.Add("GameAssets/csv_logic/spell_sets.csv");
            Gamefiles.Add("GameAssets/csv_logic/spells_buildings.csv");
            Gamefiles.Add("GameAssets/csv_logic/spells_characters.csv");
            Gamefiles.Add("GameAssets/csv_logic/spells_heroes.csv");
            Gamefiles.Add("GameAssets/csv_logic/spells_other.csv");
            Gamefiles.Add("GameAssets/csv_logic/survival_modes.csv");
            Gamefiles.Add("GameAssets/csv_logic/taunts.csv");
            Gamefiles.Add("GameAssets/csv_logic/tournament_tiers.csv");
            Gamefiles.Add("GameAssets/csv_logic/treasure_chests.csv");
            Gamefiles.Add("GameAssets/csv_logic/tutorial_chest_order.csv");
            Gamefiles.Add("GameAssets/csv_logic/tutorials_home.csv");
            Gamefiles.Add("GameAssets/csv_logic/tutorials_npc.csv");
            Gamefiles.Add("GameAssets/csv_logic/tve_gamemodes.csv");

            // TODO: csv_client

            Tables = new Gamefiles();

            foreach (var file in Gamefiles)
                Tables.Initialize(new Table(file), (Files)Gamefiles.IndexOf(file) + 1);

            Logger.Log($"{Gamefiles.Count} Gamefiles loaded.", GetType());
        }
    }
}