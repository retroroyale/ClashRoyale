using System.Diagnostics;
using ClashRoyale.Database;
using ClashRoyale.Extensions;
using ClashRoyale.Extensions.Utils;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Logic
{
    public class Player
    {
        public Player(long id)
        {
            Home = new Home.Home(id, GameUtils.GenerateToken);
        }

        public Player()
        {
            // Player.
        }

        public Home.Home Home { get; set; }

        [JsonIgnore] public Battle Battle { get; set; }

        [JsonIgnore] public Device Device { get; set; }

        [JsonIgnore] public bool IsEmpty => Home == null;

        public void LogicClientHome(IByteBuffer packet)
        {
            packet.WriteLong(Home.PlayerId);

            // Unknown
            {
                packet.WriteVInt(4597);
                packet.WriteVInt(1816);

                packet.WriteVInt(1584540);
                packet.WriteVInt(1645300);

                packet.WriteVInt(1500268361); // Last Login

                packet.WriteByte(0);
            }

            // Decks
            Home.Deck.Encode(packet);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteNullVInt();

            packet.WriteVInt(33);
            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
            packet.WriteVInt(1);
            packet.WriteVInt(0);

            // Events
            packet.WriteVInt(1);
            {
                packet.WriteVInt(1109);
                packet.WriteScString("2v2 Button");

                packet.WriteVInt(8);
                packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
                packet.WriteVInt(1601510400);
                packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteScString("2v2 Button");
                packet.WriteScString("{\"HideTimer\":true,\"HidePopupTimer\":true}\"");
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteNullVInt();

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0); // Challenge Events?

            // Events
            packet.WriteVInt(1);
            {
                packet.WriteVInt(1109);
            }

            // Events?
            packet.WriteVInt(2);
            {
                packet.WriteVInt(2);
                packet.WriteScString("{\"ID\":\"CARD_RELEASE\",\"Params\":{}});");

                packet.WriteVInt(4);
                packet.WriteScString("{\"ID\":\"CLAN_CHEST\",\"Params\":{}}");
            }

            packet.WriteVInt(4);

            // Chests
            packet.WriteVInt(0);
            {
                /*packet.WriteVInt(19); // Instance Id
                packet.WriteVInt(263); // Class Id
                packet.WriteVInt(0); // Unlocked
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0); // Slot Index
                packet.WriteVInt(0);
                packet.WriteVInt(0);*/
            }

            for (var i = 0; i < 2; i++)
            {
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteNullVInt();
            }

            packet.WriteVInt(1);
            packet.WriteVInt(19);
            packet.WriteVInt(12);

            packet.WriteVInt(1);
            packet.WriteVInt(18);
            packet.WriteVInt(0);

            packet.WriteNullVInt();
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            // Crown Chest
            {
                packet.WriteVInt(Home.Crowns); // Crowns
                packet.WriteVInt(0); // Locked
                packet.WriteVInt(0); // Time locked
            }

            packet.WriteVInt(0);
            packet.WriteVInt(63);

            // Request Cooldown
            packet.WriteVInt(1714640);
            packet.WriteVInt(1726960);
            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(63);

            packet.WriteVInt(15); // Tutorial?

            for (var i = 0; i < 7; i++)
                packet.WriteVInt(0);

            packet.WriteVInt(2); // Page Opened
            packet.WriteVInt(Home.ExpLevel); // ExpLevel

            // Arena
            {
                packet.WriteVInt(54); // Class Id
                packet.WriteVInt(11); // Instance Id
            }

            // Shop
            {
                Home.Shop.Encode(packet);
            }

            for (var i = 0; i < 3; i++)
            {
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteNullVInt();
            }

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(23);

            packet.WriteVInt(0);
            packet.WriteVInt(1);

            packet.WriteVInt(0);
            packet.WriteVInt(1);

            packet.WriteHex("F807");

            packet.WriteVInt(1);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(11);
            packet.WriteVInt(0);
            packet.WriteVInt(2);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(4);
            packet.WriteVInt(3);
            packet.WriteVInt(17);
            packet.WriteVInt(1);
            packet.WriteVInt(14);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(4);
            packet.WriteVInt(14);
            packet.WriteVInt(1);
            packet.WriteVInt(74);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(4);
            packet.WriteVInt(1);
            packet.WriteVInt(1);
            packet.WriteVInt(73);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(5);
            packet.WriteVInt(0);
            packet.WriteVInt(4);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);
            packet.WriteVInt(0);
            packet.WriteVInt(9);
            packet.WriteVInt(0);
            packet.WriteVInt(15);
            packet.WriteVInt(0);

            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

            packet.WriteVInt(1);
            packet.WriteVInt(1);
            packet.WriteVInt(6);
            packet.WriteVInt(2);
            packet.WriteVInt(16);
            packet.WriteVInt(0);


            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

            packet.WriteVInt(1);
            packet.WriteVInt(1);
            packet.WriteVInt(6);

            packet.WriteVInt(2);
            packet.WriteVInt(0);

            // Missions
            packet.WriteVInt(2);
            {
                packet.WriteVInt(26);
                packet.WriteVInt(46);

                packet.WriteVInt(28);
                packet.WriteVInt(16);
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(1);

            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(1);
            packet.WriteVInt(54000010); // New Arena

            packet.WriteVInt(0); // Session Reward = 2
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(7); // ?           
        }

        public void LogicClientAvatar(IByteBuffer packet)
        {
            // PlayerId
            for (var i = 0; i < 3; i++)
            {
                packet.WriteVInt(Home.HighId);
                packet.WriteVInt(Home.LowId);
            }


            // Name
            {
                packet.WriteScString(Home.Name);
                packet.WriteBoolean(false); // Name changed
            }

            // Profile
            {
                packet.WriteVInt(12); // Arena 
                packet.WriteVInt(3800); // Trophies 

                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(100); // Legendary Trophies

                packet.WriteVInt(0); // Current Session Trophies
                packet.WriteVInt(0);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(0); // Rank
                packet.WriteVInt(0); // Trophies
            }

            packet.WriteVInt(0);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(8);

            // Game Variables
            packet.WriteVInt(10);
            {
                packet.WriteVInt(5);
                packet.WriteVInt(1);
                packet.WriteVInt(Home.Gold); // Gold

                packet.WriteVInt(5);
                packet.WriteVInt(0);
                packet.WriteVInt(0);

                packet.WriteVInt(5);
                packet.WriteVInt(3);
                packet.WriteVInt(2);

                packet.WriteVInt(5); // New Crowns
                packet.WriteVInt(4);
                packet.WriteVInt(Home.NewCrowns);

                packet.WriteVInt(5);
                packet.WriteVInt(5);
                packet.WriteVInt(Home.Gold); // Gold

                packet.WriteVInt(5);
                packet.WriteVInt(13);
                packet.WriteVInt(0);

                packet.WriteVInt(5);
                packet.WriteVInt(14);
                packet.WriteVInt(0);

                packet.WriteVInt(5);
                packet.WriteVInt(16);
                packet.WriteVInt(51);

                packet.WriteVInt(5);
                packet.WriteVInt(28);
                packet.WriteVInt(0);

                packet.WriteVInt(5);
                packet.WriteVInt(29);
                packet.WriteVInt(72000006);
            }

            packet.WriteVInt(0); // Completed Achievements?

            // Achievements
            {
                packet.WriteVInt(0); // Achievement Count
                packet.WriteVInt(0); // Achievement Count
            }

            // Profile Statistics
            packet.WriteVInt(6);
            {
                packet.WriteVInt(5);
                packet.WriteVInt(6);
                packet.WriteVInt(30);

                packet.WriteVInt(5);
                packet.WriteVInt(7);
                packet.WriteVInt(0); // Three Crown Win Count

                packet.WriteVInt(5);
                packet.WriteVInt(8);
                packet.WriteVInt(1); // Cards found

                packet.WriteVInt(5);
                packet.WriteVInt(1); // Count
                packet.WriteVInt(26000048); // CardId

                packet.WriteVInt(5);
                packet.WriteVInt(11);
                packet.WriteVInt(32);

                packet.WriteVInt(5);
                packet.WriteVInt(27);
                packet.WriteVInt(1);
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(Home.Diamonds); // Diamonds
            packet.WriteVInt(Home.Diamonds); // FreeDiamonds

            packet.WriteVInt(Home.ExpPoints); // ExpPoints
            packet.WriteVInt(Home.ExpLevel); // ExpLevel

            packet.WriteVInt(0); // AvatarUserLevelTier

            packet.WriteVInt(7); // HasAlliance

            // Battle Statistics
            {
                packet.WriteVInt(0); 
                packet.WriteVInt(0); // Tournament Matches Played
                packet.WriteVInt(0);
                packet.WriteVInt(0); // Wins
                packet.WriteVInt(0);
                packet.WriteVInt(0);
            }

            // Tutorials
            {
                packet.WriteVInt(7);
            }

            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(0); // Has Challenge

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(TimeUtils.CurrentUnixTimestamp);
            packet.WriteVInt(0); // AccountCreated
            packet.WriteVInt(0); // PlayTime
        }

        public async void Save()
        {
            var st = new Stopwatch();
            st.Start();

            await Redis.CachePlayer(this);
            await PlayerDb.Save(this);

            st.Stop();
            Logger.Log($"Player {Home.PlayerId} saved in {st.ElapsedMilliseconds}ms.", GetType());
        }
    }
}