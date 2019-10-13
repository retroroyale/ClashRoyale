using System.Collections.Generic;
using ClashRoyale.Utilities.Utils;
using Newtonsoft.Json;

namespace ClashRoyale.Utilities.Models.Battle.Replay
{
    public class LogicReplay
    {
        [JsonProperty("battle")] public LogicBattleInfo Battle = new LogicBattleInfo();
        [JsonProperty("cmd")] public List<LogicBattleCommandStorage> Commands = new List<LogicBattleCommandStorage>();
        [JsonProperty("evt")] public List<LogicBattleEvent> Events = new List<LogicBattleEvent>();
        [JsonProperty("endTick")] public int EndTick { get; set; }
        [JsonProperty("rndSeed")] public int RandomSeed { get; set; }
        [JsonProperty("time")] public int Time = TimeUtils.CurrentUnixTimestamp;

        /// <summary>
        /// Add an event to the replay
        /// </summary>
        /// <param name="type"></param>
        /// <param name="highId"></param>
        /// <param name="lowId"></param>
        /// <param name="tick"></param>
        /// <param name="param"></param>
        public void AddEvent(int type, int highId, int lowId, int tick, int param)
        {
            if (Events.FindIndex(x => x.Ticks.Contains(tick)) != -1) return; // required ??

            var evt = new LogicBattleEvent {Type = type, HighId = highId, LowId = lowId};
            evt.Ticks.Add(tick);
            evt.Params.Add(param);
            Events.Add(evt);
        }

        /// <summary>
        /// Add a command to the replay
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tick"></param>
        /// <param name="tick2"></param>
        /// <param name="highId"></param>
        /// <param name="lowId"></param>
        /// <param name="globalId"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        public void AddCommand(int type, int tick, int tick2, int highId, int lowId, int globalId, int positionX, int positionY, int idx)
        {
            var cmdStorage = new LogicBattleCommandStorage
            {
                CommandType = type
            };

            var cmd = cmdStorage.Command;
            cmd.Tick = tick;
            cmd.Tick2 = tick2;
            cmd.HighId = highId;
            cmd.LowId = lowId;
            cmd.GlobalId = globalId;
            cmd.PositionX = positionX;
            cmd.PositionY = positionY;
            cmd.Idx = idx;

            Commands.Add(cmdStorage);
        }
    }
}
