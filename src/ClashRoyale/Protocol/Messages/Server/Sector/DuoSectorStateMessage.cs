using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class DuoSectorStateMessage : PiranhaMessage
    {
        public DuoSectorStateMessage(Device device) : base(device)
        {
            Id = 21903;
            device.CurrentState = Device.State.Battle;
        }

        public LogicDuoBattle Battle { get; set; }

        public override void Encode()
        {
            var home1 = Battle[0];
            var home2 = Battle[2];
            var enemy1 = Battle[1];
            var enemy2 = Battle[3];

            Writer.WriteBoolean(false); // IsCompressed
            Writer.WriteVInt(Battle.BattleTime); // Time
            Writer.WriteVInt(0); // Checksum
            Writer.WriteVInt(TimeUtils.CurrentUnixTimestamp); // Timestamp

            Writer.WriteVInt(11);
            Writer.WriteVInt(0); // Time
            Writer.WriteVInt(38); // Random

            Writer.WriteVInt(9);
            Writer.WriteVInt(4);

            Writer.WriteVInt(7419667);
            Writer.WriteVInt(1);

            /*for (var p = 0; p < Battle.Count; p++)
            {
                var player = Battle[p];

                for (var i = 0; i < 3; i++)
                {
                    Writer.WriteVInt(player.Home.HighId);
                    Writer.WriteVInt(player.Home.LowId);
                }

                Writer.WriteScString(player.Home.Name); // Player Name
                Writer.WriteVInt(player.Home.ExpLevel); // Level
                Writer.WriteVInt(player.Home.Arena.Trophies);

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

                var info = player.Home.AllianceInfo;
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

                Writer.WriteVInt(p == 0 ? 2 : 0); // ??
            }

            Battle.Encode(Writer);*/

            //Home Player Data
            for (var i = 0; i < 3; i++)
            {
                Writer.WriteVInt(home1.Home.HighId);
                Writer.WriteVInt(home1.Home.LowId);
            }

            Writer.WriteScString(home1.Home.Name);
            Writer.WriteVInt(10);
            Writer.WriteVInt(home1.Home.Arena.Trophies);
            Writer.WriteHex(
                "9C0396250000000000200000000000080B0501AEED020502BF02050304050401050CAD03050D00050E05050FA30505169704051C01051D9788D544000000050506842805079A04050B20051407051B0888011A00001A01001A02001A03011A04001A05001A06001A07001A08031A09001A0A001A0B001A0C001A0D001A0E001A0F001A10001A11001A12001A13001A14001A15001A16031A17B2011A18001A19001A1A021A1B001A1C001A1D001A1E001A1F031A208B011A21001A22251A232F1A24001A258C011A26351A27001A2889011A29311A2A3A1A2B001A2D2C1B00001B01001B02001B03001B04001B05001B06001B07001B08001B09001B0A001C00001C01001C02001C03001C04011C05001C06001C07001C08021C09001C0A001C0B3B1C0C001C0D0E1C10001A2E0700000802169FC71C000000094E657720416C676172AC01811000209007BA0704070000000002");

            //Enemy1 Data
            for (var i = 0; i < 3; i++)
            {
                Writer.WriteVInt(enemy1.Home.HighId);
                Writer.WriteVInt(enemy1.Home.LowId);
            }

            Writer.WriteScString(enemy1.Home.Name);
            Writer.WriteVInt(10); //Unk
            Writer.WriteVInt(enemy1.Home.Arena.Trophies);
            Writer.WriteHex(
                "9C0396250000000000200000000000080B0501AEED020502BF02050304050401050CAD03050D00050E05050FA30505169704051C01051D9788D544000000050506842805079A04050B20051407051B0888011A00001A01001A02001A03011A04001A05001A06001A07001A08031A09001A0A001A0B001A0C001A0D001A0E001A0F001A10001A11001A12001A13001A14001A15001A16031A17B2011A18001A19001A1A021A1B001A1C001A1D001A1E001A1F031A208B011A21001A22251A232F1A24001A258C011A26351A27001A2889011A29311A2A3A1A2B001A2D2C1B00001B01001B02001B03001B04001B05001B06001B07001B08001B09001B0A001C00001C01001C02001C03001C04011C05001C06001C07001C08021C09001C0A001C0B3B1C0C001C0D0E1C10001A2E0700000802169FC71C000000094E657720416C676172AC01811000209007BA0704070000000002");

            //Home2 Data 
            for (var i = 0; i < 3; i++)
            {
                Writer.WriteVInt(home2.Home.HighId);
                Writer.WriteVInt(home2.Home.LowId);
            }

            Writer.WriteScString(home2.Home.Name);
            Writer.WriteVInt(10); //Unk
            Writer.WriteVInt(home2.Home.Arena.Trophies);
            Writer.WriteHex(
                "850395250000000000200000000000080B0501B4F6010502AC06050304050403050CA909050D10050E06050F890B05169D07051C03051D9788D5440000000505069B280507AD04050B20051406051B0885011A00001A01001A02001A03001A04001A05001A06001A07001A08001A09001A0A001A0B011A0C011A0D001A0E001A0F001A10001A11001A12001A13001A15031A16021A170A1A18001A1A171A1B001A1C001A1D031A1E001A1F001A20031A21041A22001A24001A25021A26001A27001A28151A2A011A2B001A2D081A300F1B00001B01001B03001B04001B05001B06001B07001B08001B09001B0A001C00001C01011C03001C04001C05001C06001C07001C08001C09001C0A001C0B0B1C0C001C0D001A14001B02001C02001A19000000080236A2BB300000000A534B444E2047616D65732D98130000B2089C087E090000000002");

            //Enemy2 Data
            for (var i = 0; i < 3; i++)
            {
                Writer.WriteVInt(enemy2.Home.HighId);
                Writer.WriteVInt(enemy2.Home.LowId);
            }

            Writer.WriteScString(enemy2.Home.Name);
            Writer.WriteVInt(10); //Unk
            Writer.WriteVInt(enemy2.Home.Arena.Trophies);
            Writer.WriteHex(
                "B10198250000000000200000000000080B0501AABD010502B402050303050403050CB704050D0F050E0A050F890F0516910D051C03051D9788D544000000050506912505079803050B20051406051B0889011A00001A01001A02001A03031A04001A05001A06001A07001A08001A09001A0A001A0B001A0C021A0D001A0E001A0F001A10001A11021A12001A13001A14001A15001A16001A17A0011A18001A19001A1AAD021A1B001A1C001A1D121A1E001A1F031A20B8021A21251A22131A2385021A24001A25341A26001A27001A2891011A2989011A2AAA011A2B001A2DA7011A2E0A1A300C1B00001B01001B02001B03011B04001B05001B06001B07001B08001B09001B0A001C00001C01001C02001C03001C04001C05001C06001C07001C08001C09001C0A001C0B84041C0C031C0D101C1004000008021992A645000000084C65756B5A696A4E90019A1104009306A906021001000000000D040009");


            //Home
            Writer.WriteVInt(home1.Home.HighId);
            Writer.WriteVInt(home1.Home.LowId);

            Writer.WriteVInt(0); //00

            Writer.WriteVInt(enemy1.Home.HighId);
            Writer.WriteVInt(enemy1.Home.LowId);

            Writer.WriteVInt(0); //00

            Writer.WriteVInt(home2.Home.HighId);
            Writer.WriteVInt(home2.Home.LowId);

            Writer.WriteVInt(0); //00

            Writer.WriteVInt(enemy2.Home.HighId);
            Writer.WriteVInt(enemy2.Home.LowId);

            Writer.WriteHex("00000000000000000000000000009401EC7E00000A0A2301230123012301230023002300230023102310010203000001020301000500050105020503050405050506050705080509070DA4E2019C8E0300007F00C07C0002000000000000070DAC36A46500007F0080040001000000000000070DAC369C8E0300007F00C07C0001000000000000070DA4E201A46500007F0080040002000000000000070DB8AB01B82E00007F00800400000200000000000005");

            var m_aDeck = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

            //Own Deck Rotation
            Writer.WriteByte(4);
            int current = 0;
            for (int i = 0; i < 4; i++)
            {
                current = m_aDeck[i] - current;
                if (current >= 0)
                    Writer.WriteByte(current);
                else
                    Writer.WriteByte(128 + current);
                current = m_aDeck[i];
            }

            //next cards
            Writer.WriteByte(4);
            for (int i = 4; i < 8; i++)
                Writer.WriteByte(m_aDeck[i]);

            //FULL Writer.WriteHex("007F7F0000000500000000007F7F7F7F7F7F7F7F00070DB8AB0188C50300007F00C07C0000020000000000000400000500000000007F7F7F7F7F7F7F7F00070D986DB82E00007F0080040000010000000000000504010101040405040600007F7F0000000500000000007F7F7F7F7F7F7F7F00070D986D88C50300007F00C07C0000010000000000000400000500000000007F7F7F7F7F7F7F7F000009A88C0188C50300007F00C07C00000000000000000009A88C01B82E00007F00800400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B02400B02400B02400B02400AA4600AA4600AA4600AA460000000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A401");

            Writer.WriteHex("007F7F0000000500000000007F7F7F7F7F7F7F7F00070DB8AB0188C50300007F00C07C0000020000000000000400000500000000007F7F7F7F7F7F7F7F00070D986DB82E00007F00800400000100000000000005");

            var m_aDeck2 = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

            //Team Mate Deck Rotation
            Writer.WriteByte(4);
            int current2 = 0;
            for (int i = 0; i < 4; i++)
            {
                current2 = m_aDeck2[i] - current2;
                if (current2 >= 0)
                    Writer.WriteByte(current2);
                else
                    Writer.WriteByte(128 + current2);
                current2 = m_aDeck2[i];
            }

            //next cards
            Writer.WriteByte(4);
            for (int i = 4; i < 8; i++)
                Writer.WriteByte(m_aDeck2[i]);


            Writer.WriteHex("007F7F0000000500000000007F7F7F7F7F7F7F7F00070D986D88C50300007F00C07C0000010000000000000400000500000000007F7F7F7F7F7F7F7F000009A88C0188C50300007F00C07C00000000000000000009A88C01B82E00007F00800400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B02400B02400B02400B02400AA4600AA4600AA4600AA460000000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A40100000000000000A401A401");

            Writer.WriteHex("FF01");

            home1.Home.Deck.EncodeAttack(Writer);

            Writer.WriteVInt(0);

            Writer.WriteHex("FE01");

            home2.Home.Deck.EncodeAttack(Writer);

            Writer.WriteHex("00000506070802040202010300000000000000010200001800000C000000CCE9D7B507002A002B");
        }
    }
}