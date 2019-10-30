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
                    Writer.WriteVInt(0);

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

                Writer.WriteVInt(p == 3 ? 9 : 2);
            }

            Battle.Encode(Writer);
        }
    }
}