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

        public override void Encode()
        {
            const int count = 6;

            Packet.WriteVInt(0);

            Packet.WriteVInt(0); // Time
            Packet.WriteVInt(0); // Checksum
            Packet.WriteVInt(TimeUtils.CurrentUnixTimestamp); // Timestamp
            Packet.WriteVInt(11);

            Packet.WriteVInt(0);
            Packet.WriteByte(38);
            Packet.WriteVInt(9);
            Packet.WriteVInt(4);
            Packet.WriteVInt(7419667);
            Packet.WriteByte(1);

            // Player 1
            {
                for (var i = 0; i < 3; i++)
                {
                    Packet.WriteVInt(Player1.Home.HighId);
                    Packet.WriteVInt(Player1.Home.LowId);
                }

                Packet.WriteScString(Player1.Home.Name);
                Packet.WriteVInt(13); // Level
                Packet.WriteVInt(3800);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(32);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(8);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(1);

                Packet.WriteVInt(0);
                Packet.WriteVInt(29);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(2);
                Packet.WriteVInt(2);
                Packet.WriteVInt(1);
                Packet.WriteVInt(5);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(1);
                Packet.WriteVInt(2);
            }

            // Player 2
            {
                for (var i = 0; i < 3; i++)
                {
                    Packet.WriteVInt(Player2.Home.HighId);
                    Packet.WriteVInt(Player2.Home.LowId);
                }

                Packet.WriteScString(Player2.Home.Name); // Player 2 Name
                Packet.WriteVInt(13); // Level
                Packet.WriteVInt(3800);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(32);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(8);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(2);
                Packet.WriteVInt(2);

                Packet.WriteVInt(0); // HighId
                Packet.WriteVInt(1); // LowId
                Packet.WriteScString("Test"); // Name 
                Packet.WriteVInt(4); // Badge 
                Packet.WriteVInt(3); // Role

                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(2);

                Packet.WriteVInt(0);
                Packet.WriteVInt(2);
                Packet.WriteVInt(5);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(0);
                Packet.WriteVInt(1);
                Packet.WriteVInt(0);
            }

            Packet.WriteVInt(14);
            Packet.WriteVInt(2);
            Packet.WriteVInt(0);
            Packet.WriteVInt(20); // Arena 

            Packet.WriteVInt(Player1.Home.HighId);
            Packet.WriteVInt(Player1.Home.LowId);

            Packet.WriteVInt(0);

            Packet.WriteVInt(Player2.Home.HighId); // Player 2 High
            Packet.WriteVInt(Player2.Home.LowId); // Player 2 Low

            Packet.WriteVInt(1);
            Packet.WriteVInt(1);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteVInt(7);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteByte(2);
            Packet.WriteVInt(1);

            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteVInt(count);
            Packet.WriteVInt(count);

            Packet.WriteData(Csv.Tables.Get(Csv.Types.Buildings).GetDataWithInstanceId<Buildings>(1));
            Packet.WriteData(Csv.Tables.Get(Csv.Types.Buildings).GetDataWithInstanceId<Buildings>(1));
            Packet.WriteData(Csv.Tables.Get(Csv.Types.Buildings).GetDataWithInstanceId<Buildings>(1));
            Packet.WriteData(Csv.Tables.Get(Csv.Types.Buildings).GetDataWithInstanceId<Buildings>(1));

            Packet.WriteData(Csv.Tables.Get(Csv.Types.Buildings).GetDataWithInstanceId<Buildings>(0));
            Packet.WriteData(Csv.Tables.Get(Csv.Types.Buildings).GetDataWithInstanceId<Buildings>(0));

            Packet.WriteVInt(1);
            Packet.WriteVInt(0);
            Packet.WriteVInt(1);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(1);

            for (var index = 0; index < count; index++)
            {
                Packet.WriteVInt(5);
                Packet.WriteVInt(index);
            }

            // Player Right Princess Tower
            Packet.WriteVInt(12);
            Packet.WriteVInt(13);
            Packet.WriteVInt(14500); // X
            Packet.WriteVInt(25500); // Y
            Packet.WriteHex("00007F00C07C0002000000000000");

            // Enemy Left Princess Tower
            Packet.WriteVInt(12);
            Packet.WriteVInt(13);
            Packet.WriteVInt(3500); // X
            Packet.WriteVInt(6500); // Y
            Packet.WriteHex("00007F0080040001000000000000");

            // Player Left Princess Tower
            Packet.WriteVInt(12);
            Packet.WriteVInt(13);
            Packet.WriteVInt(3500); // X
            Packet.WriteVInt(25500); // Y
            Packet.WriteHex("00007F00C07C0001000000000000");

            // Enemy Right Princess Tower
            Packet.WriteVInt(12);
            Packet.WriteVInt(13);
            Packet.WriteVInt(14500); // X
            Packet.WriteVInt(6500); // Y
            Packet.WriteHex("00007F0080040002000000000000");

            // Enemy Crown Tower
            Packet.WriteVInt(12);
            Packet.WriteVInt(13);
            Packet.WriteVInt(9000); // X
            Packet.WriteVInt(3000); // Y
            Packet.WriteHex("00007F0080040000000000000000");

            Packet.WriteHex("000504077F7D7F0400050401007F7F0000");
            Packet.WriteVInt(0); // Ms before regen mana
            Packet.WriteVInt(8); // Mana Start 
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteHex("00007F7F7F7F7F7F7F7F00");

            // Player Crown Tower
            Packet.WriteVInt(12);
            Packet.WriteVInt(13);
            Packet.WriteVInt(9000); // X
            Packet.WriteVInt(29000); // Y
            Packet.WriteHex("00007F00C07C0000000000000000");

            Packet.WriteHex("00050401047D010400040706007F7F0000");
            Packet.WriteVInt(0); // Ms before regen mana
            Packet.WriteVInt(8); // Elexir Start Enemy
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);

            Packet.WriteHex("00007F7F7F7F7F7F7F7F");

            for (var index = 0; index < 48; index++)
                Packet.WriteVInt(0);

            Packet.WriteVInt(3668); // Enemy 
            Packet.WriteVInt(0);
            Packet.WriteVInt(3668); // Player
            Packet.WriteVInt(0);
            Packet.WriteVInt(3668); // Enemy
            Packet.WriteVInt(0);
            Packet.WriteVInt(3668); // Player
            Packet.WriteVInt(0);

            Packet.WriteVInt(5832); // Enemy
            Packet.WriteVInt(0);
            Packet.WriteVInt(5832); // Player
            Packet.WriteVInt(0);

            for (var index = 0; index < count; index++)
                Packet.WriteHex("00000000000000A401A401");

            Packet.WriteHex("FF01");
            /*for (var i = 0; i < 8; i++)
            {
                Packet.WriteVInt(i + 1 + 30);
                Packet.WriteVInt(4);
            }*/
            Player1.Home.Deck.EncodeAttack(Packet);

            Packet.WriteVInt(0);

            Packet.WriteHex("FE03");
            /*for (var i = 0; i < 8; i++)
            {
                Packet.WriteVInt(i + 1 + 10);
                Packet.WriteVInt(4);
            }*/
            Player2.Home.Deck.EncodeAttack(Packet);

            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(5);
            Packet.WriteVInt(6);
            Packet.WriteVInt(2);
            Packet.WriteVInt(2);
            Packet.WriteVInt(4);
            Packet.WriteVInt(2);
            Packet.WriteVInt(1);
            Packet.WriteVInt(3);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(6);
            Packet.WriteVInt(1);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(9);
            Packet.WriteVInt(0);
            Packet.WriteVInt(0);
            Packet.WriteVInt(12);

            Packet.WriteHex("000000F69686FF0A002A002B");

            Packet.WriteVInt(0);
            Packet.WriteVInt(13);
            Packet.WriteVInt(14500);
            Packet.WriteVInt(25500);
            Packet.WriteHex("00007F00C07C0002000000000000");

            Packet.WriteVInt(0);
            Packet.WriteVInt(13);
            Packet.WriteVInt(3500);
            Packet.WriteVInt(6500);
            Packet.WriteHex("00007F0080040001000000000000");

            Packet.WriteVInt(0);
            Packet.WriteVInt(13);
            Packet.WriteVInt(3500);
            Packet.WriteVInt(25500);
            Packet.WriteHex("00007F00C07C0001000000000000");

            Packet.WriteVInt(0);
            Packet.WriteVInt(13);
            Packet.WriteVInt(14500);
            Packet.WriteVInt(6500);
            Packet.WriteHex("00007F0080040002000000000000");

            Packet.WriteVInt(0);
            Packet.WriteVInt(13);
            Packet.WriteVInt(9000);
            Packet.WriteVInt(3000);
            Packet.WriteHex("00007F0080040000000000000000");

            Packet.WriteVInt(0);
            Packet.WriteVInt(5);
            Packet.WriteVInt(1);
            Packet.WriteVInt(0);

            Packet.WriteHex("7F000000007F7F0000000100000000007F7F7F7F7F7F7F7F");
            Packet.WriteVInt(0);

            Packet.WriteVInt(0);
            Packet.WriteVInt(13);
            Packet.WriteVInt(9000);
            Packet.WriteVInt(29000);
            Packet.WriteHex("00007F00C07C0000000000000000");

            Packet.WriteVInt(0);
            Packet.WriteVInt(5);
            Packet.WriteVInt(4);
            Packet.WriteVInt(0);
            Packet.WriteVInt(1);
            Packet.WriteVInt(4);

            Packet.WriteHex(
                "7F020203007F7F0000000500000000007F7F7F7F7F7F7F7F0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");

            Packet.WriteVInt(0);
            Packet.WriteVInt(1400);

            Packet.WriteVInt(0);
            Packet.WriteVInt(560);

            Packet.WriteVInt(0);
            Packet.WriteVInt(1400);

            Packet.WriteVInt(0);
            Packet.WriteVInt(560);

            Packet.WriteVInt(0);
            Packet.WriteVInt(960);

            Packet.WriteVInt(0);
            Packet.WriteVInt(2400);

            for (var index = 0; index < count; index++)
                Packet.WriteHex("00000000000000A401A401");
        }
    }
}