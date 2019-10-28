using System;
using System.Collections.Generic;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Home.Decks;
using ClashRoyale.Logic.Home.StreamEntry;
using ClashRoyale.Logic.Sessions;
using ClashRoyale.Utilities.Models.Battle;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home
{
    public class Home
    {
        [JsonProperty("clan_info")] public AllianceInfo AllianceInfo = new AllianceInfo();
        [JsonProperty("arena")] public Arena Arena = new Arena();
        [JsonProperty("chests")] public Chests.Chests Chests = new Chests.Chests();
        [JsonProperty("crownChestCooldown")] public DateTime CrownChestCooldown = DateTime.UtcNow;
        [JsonProperty("deck")] public Deck Deck = new Deck();

        // Chests
        [JsonProperty("freeChestTime")] public DateTime FreeChestTime = DateTime.UtcNow;

        [JsonIgnore] public List<Session> Sessions = new List<Session>(50);
        [JsonProperty("shop")] public Shop.Shop Shop = new Shop.Shop();
        [JsonProperty("stream")] public List<AvatarStreamEntry> Stream = new List<AvatarStreamEntry>(40);

        public Home()
        {
            Deck.Home = this;
            Shop.Home = this;
            Arena.Home = this;
            Chests.Home = this;
        }

        public Home(long id, string token)
        {
            Id = id;
            UserToken = token;

            PreferredDeviceLanguage = "EN";

            Gold = 100;
            Diamonds = 1000000;

            Name = "NoName";
            ExpLevel = 1;

            Decks = new int[5][];
            for (var i = 0; i < 5; i++) Decks[i] = new int[8];

            Deck.Home = this;
            Deck.Initialize();

            Shop.Home = this;
            Shop.Refresh();

            Chests.Home = this;
            Arena.Home = this;
        }

        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("token")] public string UserToken { get; set; }
        [JsonProperty("name_set")] public int NameSet { get; set; }
        [JsonProperty("created_ip")] public string CreatedIpAddress { get; set; }
        [JsonProperty("high_id")] public int HighId { get; set; }
        [JsonProperty("low_id")] public int LowId { get; set; }
        [JsonProperty("language")] public string PreferredDeviceLanguage { get; set; }
        [JsonProperty("fcb_id")] public string FacebookId { get; set; }
        [JsonProperty("totalSessions")] public int TotalSessions { get; set; }
        [JsonProperty("totalPlayTimeSeconds")] public int TotalPlayTimeSeconds { get; set; }

        // Shop
        [JsonProperty("shop_day")] public int ShopDay { get; set; }

        // Resources
        [JsonProperty("diamonds")] public int Diamonds { get; set; }
        [JsonProperty("gold")] public int Gold { get; set; }

        // Crownchest
        [JsonProperty("crowns")] public int Crowns { get; set; }
        [JsonProperty("new_crowns")] public int NewCrowns { get; set; }

        // Player Stats
        [JsonProperty("exp_level")] public int ExpLevel { get; set; }
        [JsonProperty("exp_points")] public int ExpPoints { get; set; }

        // Deck
        [JsonProperty("selected_deck")] public int SelectedDeck { get; set; }
        [JsonProperty("decks")] public int[][] Decks { get; set; }

        [JsonIgnore]
        public long Id
        {
            get => ((long) HighId << 32) | (LowId & 0xFFFFFFFFL);
            set
            {
                HighId = Convert.ToInt32(value >> 32);
                LowId = (int) value;
            }
        }

        [JsonIgnore]
        public LogicBattleAvatar BattleAvatar
        {
            get
            {
                var avatar = new LogicBattleAvatar
                {
                    Name = Name,
                    HighId = HighId,
                    LowId = LowId,
                    ExpLevel = 13,
                    ExpPoints = ExpPoints,
                    Arena = 54000000 + Arena.CurrentArena
                };

                if (!AllianceInfo.HasAlliance) return avatar;

                avatar.ClanName = AllianceInfo.Name;
                avatar.ClanHighId = AllianceInfo.HighId;
                avatar.ClanLowId = AllianceInfo.LowId;
                avatar.ClanBadge = AllianceInfo.Badge;

                return avatar;
            }
        }

        [JsonIgnore]
        public List<LogicBattleSpell> BattleDeck
        {
            get
            {
                var spells = new List<LogicBattleSpell>(8);

                for (var i = 0; i < 8; i++)
                {
                    var spell = Deck[i];
                    spells.Add(spell.BattleSpell);
                }

                return spells;
            }
        }

        /// <summary>
        ///     Buy a resource pack with gems by the given id
        /// </summary>
        /// <param name="id"></param>
        public void BuyResourcePack(int id)
        {
            var packs = Csv.Tables.Get(Csv.Files.ResourcePacks).GetDataWithInstanceId<ResourcePacks>(id);
            var amount = packs.Amount;
            var diamondCost = 1;

            if (amount > 100)
            {
                if (amount > 1000)
                    if (amount > 10000)
                        if (amount > 100000)
                        {
                            if (amount >= 1000000)
                                diamondCost = 45000;
                        }
                        else
                        {
                            diamondCost = 4500;
                        }
                    else
                        diamondCost = 500;
                else
                    diamondCost = 60;
            }
            else
            {
                diamondCost = 8;
            }


            Gold += amount;
            Diamonds -= diamondCost;
        }

        /// <summary>
        ///     Add's experience Points to the players account and increments the players level if available
        /// </summary>
        /// <param name="expPoints"></param>
        public void AddExpPoints(int expPoints)
        {
            if (ExpLevel >= 13) return;

            ExpPoints += expPoints;

            for (var i = ExpLevel; i < 13; i++)
            {
                var data = Csv.Tables.Get(Csv.Files.ExpLevels).GetDataWithInstanceId<ExpLevels>(ExpLevel - 1);
                if (data.ExpToNextLevel <= ExpPoints)
                {
                    ExpLevel++;
                    ExpPoints -= data.ExpToNextLevel;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Add up to 20 crowns - TODO: check cooldown
        /// </summary>
        /// <param name="crowns"></param>
        public void AddCrowns(int crowns)
        {
            var maxCrowns = Csv.Tables.Get(Csv.Files.Globals).GetData<Globals>("CROWN_CHEST_CROWN_COUNT").NumberValue *
                            2;

            if (Crowns + crowns <= maxCrowns)
            {
                NewCrowns += crowns;
            }
            else
            {
                NewCrowns = maxCrowns - Crowns;
                Crowns = maxCrowns;
            }
        }

        /// <summary>
        ///     Returns true if it was able to remove the amount of gold from the players account
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool UseGold(int amount)
        {
            if (Gold - amount < 0) return false;

            Gold -= amount;
            return true;
        }

        /// <summary>
        ///     Returns true when the first free chest is available
        /// </summary>
        /// <returns></returns>
        public bool IsFirstFreeChestAvailable()
        {
            return DateTime.UtcNow.Subtract(FreeChestTime).TotalHours >= 4;
        }

        /// <summary>
        ///     Returns true when the first and second free chest is available
        /// </summary>
        /// <returns></returns>
        public bool IsSecondFreeChestAvailable()
        {
            return DateTime.UtcNow.Subtract(FreeChestTime).TotalHours >= 8;
        }

        /// <summary>
        ///     Get's the current free chest id
        /// </summary>
        /// <returns></returns>
        public int GetFreeChestId()
        {
            var id = 1;

            if (IsFirstFreeChestAvailable() && !IsSecondFreeChestAvailable())
                id = 0;
            else if (IsSecondFreeChestAvailable())
                id = 2;

            return id;
        }

        /// <summary>
        ///     This will be called when a user is in home state
        /// </summary>
        public void Reset()
        {
            Crowns += NewCrowns;
            NewCrowns = 0;
        }
    }
}