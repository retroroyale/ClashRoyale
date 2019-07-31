using ClashRoyale.Extensions;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class TestSectorStateMessage : PiranhaMessage
    {
        public TestSectorStateMessage(Device device) : base(device)
        {
            Id = 21903;
            device.CurrentState = Device.State.Battle;
        }

        public LogicBattle Battle { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public override void Encode()
        {
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

            for (var p = 0; p < Battle.Count; p++)
            {
                var player = Battle[p];

                for (var i = 0; i < 3; i++)
                {
                    Writer.WriteVInt(player.Home.HighId);
                    Writer.WriteVInt(player.Home.LowId);
                }

                Writer.WriteScString(player.Home.Name); // Player 2 Name
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

            const int towers = 6;

            Writer.WriteVInt(Csv.Tables.Get(Csv.Files.Locations)
                                 .GetData<Locations>(Csv.Tables.Get(Csv.Files.Arenas)
                                     .GetDataWithInstanceId<Arenas>(Battle.Arena - 1).PvpLocation).GetInstanceId() +
                             1); // LocationData

            Writer.WriteVInt(Battle.Count); // PlayerCount
            Writer.WriteVInt(0); // NpcData
            Writer.WriteVInt(Battle.Arena); // ArenaData

            foreach (var player in Battle)
            {
                Writer.WriteVInt(player.Home.HighId);
                Writer.WriteVInt(player.Home.LowId);
                Writer.WriteVInt(0);
            }

            // ConstantSizeIntArray
            {
                Writer.WriteVInt(1);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);

                Writer.WriteVInt(7);
                Writer.WriteVInt(0);
                Writer.WriteVInt(0);
            }

            Writer.WriteBoolean(false); // IsReplay / Type?
            Writer.WriteBoolean(false); // IsEndConditionMatched

            Writer.WriteBoolean(false);
            Writer.WriteBoolean(false);

            Writer.WriteBoolean(false); // isBattleEndedWithTimeOut
            Writer.WriteBoolean(false);

            Writer.WriteBoolean(false); // hasPlayerFinishedNpcLevel
            Writer.WriteBoolean(false);

            Writer.WriteBoolean(false); // isInOvertime
            Writer.WriteBoolean(false); // isTournamentMode

            Writer.WriteVInt(0);

            Writer.WriteVInt(towers);
            Writer.WriteVInt(towers);

            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(1));

            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));
            Writer.WriteData(Csv.Tables.Get(Csv.Files.Buildings).GetDataWithInstanceId<Buildings>(0));

            // LogicGameObject::encodeComponent
            Writer.WriteVInt(1);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);

            for (var i = 0; i < towers; i++)
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
            Writer.WriteVInt(X); // X 3500
            Writer.WriteVInt(Y); // Y 6500
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
            for (var index = 0; index < towers; index++)
                Writer.WriteHex("00000000000000A401A401");

            Writer.WriteHex("FF01");
            Battle[0].Home.Deck.EncodeAttack(Writer);

            Writer.WriteVInt(0);

            Writer.WriteHex("FE03");
            Battle[1].Home.Deck.EncodeAttack(Writer);

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

            for (var index = 0; index < towers; index++)
                Writer.WriteHex("00000000000000A401A401");
        }
    }
}