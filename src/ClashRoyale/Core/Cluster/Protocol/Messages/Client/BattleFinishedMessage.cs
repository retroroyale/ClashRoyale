using ClashRoyale.Utilities.Models.Battle.Replay;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using Newtonsoft.Json;

namespace ClashRoyale.Core.Cluster.Protocol.Messages.Client
{
    public class BattleFinishedMessage : ClusterMessage
    {
        public BattleFinishedMessage(Node server, IByteBuffer buffer) : base(server, buffer)
        {
            Id = 11000;
        }

        public long SessionId { get; set; }
        public byte Gamemode { get; set; }
        public byte Index { get; set; }
        public string ReplayJson { get; set; }

        public override void Decode()
        {
            SessionId = Reader.ReadLong();
            Gamemode = Reader.ReadByte();
            Index = Reader.ReadByte();
            ReplayJson = Reader.ReadScString();
        }

        public override void Process()
        {
            if (Gamemode == 0)
            {
                var battle = Resources.Battles.Get(SessionId);
                if (battle == null) return;

                /*var replay = JsonConvert.DeserializeObject<LogicReplay>(ReplayJson);

                battle.Replay.Commands = replay.Commands;
                battle.Replay.RandomSeed = replay.RandomSeed;
                battle.Replay.Time = replay.Time;
                battle.Replay.EndTick = replay.EndTick;*/

                battle.Stop(Index);
            }

            // TODO: DUO
        }
    }
}