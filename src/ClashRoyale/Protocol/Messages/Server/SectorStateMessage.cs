using ClashRoyale.Extensions;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class SectorStateMessage : PiranhaMessage
    {
        public SectorStateMessage(Device device) : base(device)
        {
            Id = 21903;
            device.CurrentState = Device.State.Battle;
        }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int Arena { get; set; } 
        
        public override void Encode()
        {
            const int count = 6;

            Writer.WriteBoolean(false); // IsCompressed
            Writer.WriteVInt(0); // Time
            Writer.WriteVInt(0); // Checksum
            Writer.WriteVInt(TimeUtils.CurrentUnixTimestamp); // Timestamp

            Writer.WriteVInt(11);
            Writer.WriteVInt(0); // Time
            Writer.WriteVInt(38); // Random

            Writer.WriteVInt(9);
            Writer.WriteVInt(4);

            Writer.WriteVInt(7419667);
            Writer.WriteVInt(1);

            // Player 1
            {
                for (var i = 0; i < 3; i++)
                {
                    Writer.WriteVInt(Player1.Home.HighId);
                    Writer.WriteVInt(Player1.Home.LowId);
                }

                Writer.WriteScString(Player1.Home.Name);
                Writer.WriteVInt(Player1.Home.ExpLevel);
                Writer.WriteVInt(Player1.Home.Arena.Trophies);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(32);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(8);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(1);

                var info = Player1.Home.AllianceInfo;
                if (info.HasAlliance)
                {
                    Writer.WriteVInt(2); // Has Clan = 2

                    Writer.WriteVInt(info.HighId); // HighId
                    Writer.WriteVInt(info.LowId); // LowId
                    Writer.WriteScString(info.Name); // Name 
                    Writer.WriteVInt(info.Badge); // Badge 
                }
                else
                {
                    Writer.WriteVInt(0);
                }

                Writer.WriteVInt(29);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(2);

                Writer.WriteVInt(2);
                Writer.WriteVInt(1);
                Writer.WriteVInt(5);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(1);
                Writer.WriteVInt(2);
            }

            // Player 2
            {
                for (var i = 0; i < 3; i++)
                {
                    Writer.WriteVInt(Player2.Home.HighId);
                    Writer.WriteVInt(Player2.Home.LowId);
                }

                Writer.WriteScString(Player2.Home.Name); // Player 2 Name
                Writer.WriteVInt(Player2.Home.ExpLevel); // Level
                Writer.WriteVInt(Player2.Home.Arena.Trophies);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(32);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(8);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(2);
      
                var info = Player2.Home.AllianceInfo;
                if (info.HasAlliance)
                {
                    Writer.WriteVInt(2); // Has Clan = 2

                    Writer.WriteVInt(info.HighId); // HighId
                    Writer.WriteVInt(info.LowId); // LowId
                    Writer.WriteScString(info.Name); // Name 
                    Writer.WriteVInt(info.Badge); // Badge 
                }
                else
                {
                    Writer.WriteVInt(0);
                }

                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
                Writer.WriteVInt(2);
                Writer.WriteVInt(0);

                Writer.WriteVInt(2);
                Writer.WriteVInt(5);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(0);
                Writer.WriteVInt(1);
                Writer.WriteVInt(0);
            }

            Writer.WriteVInt(Csv.Tables.Get(Csv.Files.Locations).GetData<Locations>(Csv.Tables.Get(Csv.Files.Arenas).GetDataWithInstanceId<Arenas>(Arena - 1).PvpLocation).GetInstanceId() + 1); // Location
            Writer.WriteVInt(2); // Players
            Writer.WriteVInt(0); // Npc
            Writer.WriteVInt(Arena); // Arena 

            Writer.WriteVInt(Player1.Home.HighId);
            Writer.WriteVInt(Player1.Home.LowId);

            Writer.WriteVInt(0);

            Writer.WriteVInt(Player2.Home.HighId); // Player 2 High
            Writer.WriteVInt(Player2.Home.LowId); // Player 2 Low

            Writer.WriteVInt(1);
            Writer.WriteVInt(1);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);

            Writer.WriteVInt(7);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);

            Writer.WriteBoolean(false);
            Writer.WriteBoolean(false);

            Writer.WriteBoolean(false);
            Writer.WriteBoolean(false);

            Writer.WriteBoolean(false); 
            Writer.WriteBoolean(false); 

            Writer.WriteVInt(0);
            Writer.WriteVInt(2);
            Writer.WriteVInt(1);

            Writer.WriteVInt(0);
            Writer.WriteVInt(0);

            Writer.WriteVInt(count);
            Writer.WriteVInt(count);

            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));

            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));

            Writer.WriteVInt(1);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);

            for (var i = 0; i < count; i++)
            {
                Writer.WriteVInt(5);
                Writer.WriteVInt(i);
            }

            // Player Right Princess Tower
            Writer.WriteVInt(12);
            Writer.WriteVInt(13);
            Writer.WriteVInt(14500); // X
            Writer.WriteVInt(25500); // Y
            Writer.WriteHex("00007F00C07C0002000000000000");

            // Enemy Left Princess Tower
            Writer.WriteVInt(12);
            Writer.WriteVInt(13);
            Writer.WriteVInt(3500); // X
            Writer.WriteVInt(6500); // Y
            Writer.WriteHex("00007F0080040001000000000000");

            // Player Left Princess Tower
            Writer.WriteVInt(12);
            Writer.WriteVInt(13);
            Writer.WriteVInt(3500); // X
            Writer.WriteVInt(25500); // Y
            Writer.WriteHex("00007F00C07C0001000000000000");

            // Enemy Right Princess Tower
            Writer.WriteVInt(12);
            Writer.WriteVInt(13);
            Writer.WriteVInt(14500); // X
            Writer.WriteVInt(6500); // Y
            Writer.WriteHex("00007F0080040002000000000000");

            // Enemy Crown Tower
            Writer.WriteVInt(12);
            Writer.WriteVInt(13);
            Writer.WriteVInt(9000); // X
            Writer.WriteVInt(3000); // Y
            Writer.WriteHex("00007F0080040000000000000000");

            Writer.WriteHex("000504077F7D7F0400050401007F7F0000");
            Writer.WriteVInt(0); // Ms before regen mana
            Writer.WriteVInt(6); // Mana Start 
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);

            Writer.WriteHex("00007F7F7F7F7F7F7F7F00");

            // Player Crown Tower
            Writer.WriteVInt(12);
            Writer.WriteVInt(13);
            Writer.WriteVInt(9000); // X
            Writer.WriteVInt(29000); // Y
            Writer.WriteHex("00007F00C07C0000000000000000");

            Writer.WriteHex("00050401047D010400040706007F7F0000");
            Writer.WriteVInt(0); // Ms before regen mana
            Writer.WriteVInt(6); // Elexir Start Enemy
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);

            Writer.WriteVInt(0);
            Writer.WriteVInt(0);

            for (var index = 0; index < 8; index++)
                Writer.WriteVInt(-1);

            for (var index = 0; index < 48; index++)
                Writer.WriteVInt(0);

            // LogicHitpointComponent
            Writer.WriteVInt(3668); // Enemy 
            Writer.WriteVInt(0);
            Writer.WriteVInt(3668); // Player
            Writer.WriteVInt(0);
            Writer.WriteVInt(3668); // Enemy
            Writer.WriteVInt(0);
            Writer.WriteVInt(3668); // Player
            Writer.WriteVInt(0);
            Writer.WriteVInt(5832); // Enemy
            Writer.WriteVInt(0);
            Writer.WriteVInt(5832); // Player
            Writer.WriteVInt(0);

            // LogicCharacterBuffComponent
            for (var index = 0; index < count; index++)
                Writer.WriteHex("00000000000000A401A401");

            Writer.WriteShort(-255);
            Player1.Home.Deck.EncodeAttack(Writer);

            Writer.WriteVInt(0);

            Writer.WriteShort(-509);
            Player2.Home.Deck.EncodeAttack(Writer);

            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(5);
            Writer.WriteVInt(6);
            Writer.WriteVInt(2);
            Writer.WriteVInt(2);
            Writer.WriteVInt(4);
            Writer.WriteVInt(2);
            Writer.WriteVInt(1);
            Writer.WriteVInt(3);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(6);
            Writer.WriteVInt(1);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(9);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(12);

            Writer.WriteHex("000000F69686FF0A002A002B");

            Writer.WriteVInt(0);
            Writer.WriteVInt(13);
            Writer.WriteVInt(14500);
            Writer.WriteVInt(25500);
            Writer.WriteHex("00007F00C07C0002000000000000");

            Writer.WriteVInt(0);
            Writer.WriteVInt(13);
            Writer.WriteVInt(3500);
            Writer.WriteVInt(6500);
            Writer.WriteHex("00007F0080040001000000000000");

            Writer.WriteVInt(0);
            Writer.WriteVInt(13);
            Writer.WriteVInt(3500);
            Writer.WriteVInt(25500);
            Writer.WriteHex("00007F00C07C0001000000000000");

            Writer.WriteVInt(0);
            Writer.WriteVInt(13);
            Writer.WriteVInt(14500);
            Writer.WriteVInt(6500);
            Writer.WriteHex("00007F0080040002000000000000");

            Writer.WriteVInt(0);
            Writer.WriteVInt(13);
            Writer.WriteVInt(9000);
            Writer.WriteVInt(3000);
            Writer.WriteHex("00007F0080040000000000000000");

            Writer.WriteVInt(0);
            Writer.WriteVInt(5);
            Writer.WriteVInt(1);
            Writer.WriteVInt(0);

            Writer.WriteHex("7F000000007F7F0000000100000000007F7F7F7F7F7F7F7F");
            Writer.WriteVInt(0);

            Writer.WriteVInt(0);
            Writer.WriteVInt(13);
            Writer.WriteVInt(9000);
            Writer.WriteVInt(29000);
            Writer.WriteHex("00007F00C07C0000000000000000");

            Writer.WriteVInt(0);
            Writer.WriteVInt(5);
            Writer.WriteVInt(4);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);
            Writer.WriteVInt(4);

            Writer.WriteHex(
                "7F020203007F7F0000000500000000007F7F7F7F7F7F7F7F0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");

            Writer.WriteVInt(0);
            Writer.WriteVInt(1400);

            Writer.WriteVInt(0);
            Writer.WriteVInt(560);

            Writer.WriteVInt(0);
            Writer.WriteVInt(1400);

            Writer.WriteVInt(0);
            Writer.WriteVInt(560);

            Writer.WriteVInt(0);
            Writer.WriteVInt(960);

            Writer.WriteVInt(0);
            Writer.WriteVInt(2400);

            for (var index = 0; index < count; index++)
                Writer.WriteHex("00000000000000A401A401");
        }
    }
}