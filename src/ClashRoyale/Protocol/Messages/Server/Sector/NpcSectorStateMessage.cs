using System;
using ClashRoyale.Extensions;
using ClashRoyale.Files;
using ClashRoyale.Files.CsvLogic;
using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class NpcSectorStateMessage : PiranhaMessage
    {
        public NpcSectorStateMessage(Device device) : base(device)
        {
            Id = 21903;
            device.CurrentState = Device.State.Battle;
            device.LastVisitHome = DateTime.UtcNow;
        }

        public override void Encode()
        {
            const int towers = 6; // Tower Count

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

            Writer.WriteHex("7F7F7F7F");

            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(1);
            Writer.WriteVInt(6400); // Trophies Trainer ??

            for (var i = 0; i < 13; i++)
            {
                Writer.WriteByte(0);
            }

            Writer.WriteVInt(8);

            for (var i = 0; i < 8; i++)
            {
                Writer.WriteByte(0);
            }

            Writer.WriteVInt(10);

            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);
            Writer.WriteVInt(0);

            Writer.WriteVInt(1);
            Writer.WriteVInt(2);

            for (var i = 0; i < 3; i++)
            {
                Writer.WriteVInt(Device.Player.Home.HighId);
                Writer.WriteVInt(Device.Player.Home.LowId);
            }
            Writer.WriteScString(Device.Player.Home.Name);

            Writer.WriteHex("08982FBE02972F0000000000200000000000080D05019A750502990B050304050400050CB90C050D00050E00050FBA0C05169F0E051991AFC6C90E051A10051C00051D9788D544000000050506843205079906050B20051409051B0A89011A00001A01001A02001A03001A04001A05001A06001A07001A08001A09001A0A001A0B001A0C001A0D001A0E001A0F001A10001A11001A12001A13001A14001A15001A16001A17001A18001A19001A1A001A1B001A1C001A1D001A1E001A1F001A20001A21001A22001A23001A24001A25001A26001A27001A28001A29001A2A001A2B001A2D001A2E001A30021B00001B01001B02001B03001B04001B05001B06001B07001B08001B09001B0A001C00001C01001C02001C03001C04001C05001C06001C07001C08001C09001C0A001C0B001C0C001C0D001C100000000B020C96AD14");
            Writer.WriteScString("Training");
            Writer.WriteHex("8B02B21F3100BC0DB20D099F010200000000020224017F7F00");

            Writer.WriteVInt(Device.Player.Home.HighId);
            Writer.WriteVInt(Device.Player.Home.LowId);
            Writer.WriteByte(0);

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

            Writer.WriteBoolean(true); // IsNpc

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
            for (var index = 0; index < towers; index++)
                Writer.WriteHex("00000000000000A401A401");

            // Trainer
            Writer.WriteHex("FF01");
            Device.Player.Home.Deck.EncodeAttack(Writer);

            Writer.WriteByte(0);

            // Player        
            Writer.WriteHex("FE03");
            Device.Player.Home.Deck.EncodeAttack(Writer);

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