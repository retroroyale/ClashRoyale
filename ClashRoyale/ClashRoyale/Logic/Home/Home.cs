using System;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic.Clan;
using ClashRoyale.Logic.Home.Decks;
using Newtonsoft.Json;

namespace ClashRoyale.Logic.Home
{
    public class Home
    {
        public Arena Arena = new Arena();
        public Chests.Chests Chests = new Chests.Chests();
        public Deck Deck = new Deck();
        public Shop.Shop Shop = new Shop.Shop();
        public AllianceInfo AllianceInfo = new AllianceInfo();

        public Home()
        {
            Deck.Home = this;
            Shop.Home = this;
            Chests.Home = this;
            Arena.Home = this;
        }

        public Home(long id, string token)
        {
            Id = id;
            UserToken = token;

            Name = "NoNameSet";
            PreferredDeviceLanguage = "EN";

            Gold = 100;
            Diamonds = 1000000;

            ExpLevel = 1;

            Deck.Home = this;
            Deck.Initialize();

            Shop.Home = this;
            Shop.Refresh();

            Chests.Home = this;
        }

        public string Name { get; set; }
        public string UserToken { get; set; }
        public int NameChangeCount { get; set; }
        public string IpAddress { get; set; }
        public int HighId { get; set; }
        public int LowId { get; set; }
        public string PreferredDeviceLanguage { get; set; }
        public string FacebookId { get; set; }

        // Shop
        public int ShopDay { get; set; }

        // Resources
        public int Diamonds { get; set; }
        public int Gold { get; set; }

        // Crownchest
        public int Crowns { get; set; }
        public int NewCrowns { get; set; }

        // Player Stats
        public int Trophies { get; set; }
        public int ExpLevel { get; set; }
        public int ExpPoints { get; set; }

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

        public void BuyResourcePack(int id)
        {
            var packs = Csv.Tables.Get(Csv.Types.ResourcePacks).GetDataWithInstanceId<ResourcePacks>(id);
            var amount = packs.Amount;
            var diamondCost = 1;

            if (amount > 100)
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
            else
                diamondCost = 8;


            Gold += amount;
            Diamonds -= diamondCost;
        }

        public void AddExpPoints(int expPoints)
        {
            if (ExpLevel <= 13)
            {
                ExpPoints += expPoints;

                for (var i = ExpLevel; i < 13; i++)
                {
                    var data = Csv.Tables.Get(Csv.Types.ExpLevels).GetDataWithInstanceId<ExpLevels>(ExpLevel - 1);
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
        }

        public void AddCrowns(int crowns)
        {
            if (Crowns + crowns <= 20) NewCrowns += crowns;
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