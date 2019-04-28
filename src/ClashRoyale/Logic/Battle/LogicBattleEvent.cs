using System;
using System.Collections.Generic;
using SharpRaven.Data;

namespace ClashRoyale.Logic.Battle
{
    public class LogicBattleEvent
    {
        private readonly List<int> _coords;
        private readonly List<int> _params;
        private readonly List<int> _ticks;

        /// <summary>
        ///     Initialize the instance
        /// </summary>
        public LogicBattleEvent()
        {
            _ticks = new List<int>();
            _coords = new List<int>();
            _params = new List<int>();
        }

        /// <summary>
        ///     Save this instance as Json
        /// </summary>
        public void SaveJson()
        {
            // TODO
        }

        /// <summary>
        ///     Load this instance from Json
        /// </summary>
        public void LoadJson()
        {
            // TODO
        }

        /// <summary>
        ///     Returns the AccountId
        /// </summary>
        /// <returns></returns>
        public long GetAccountId()
        {
            return 0;

            // TODO
        }

        /// <summary>
        ///     Set the type of this Event
        /// </summary>
        /// <param name="type"></param>
        public void SetType(byte type)
        {
            // TODO
        }

        /// <summary>
        ///     Get the type of this event
        /// </summary>
        /// <returns></returns>
        public new byte GetType()
        {
            return 0;

            // TODO
        }

        /// <summary>
        ///     Set the tick of this instance
        /// </summary>
        /// <param name="tick"></param>
        public void SetTick(int tick)
        {
            if (_ticks.Count > 0)
            {
                Logger.Log("replay event: setting tick will clear old ticks and coords", base.GetType(),
                    ErrorLevel.Debug);
                return;
            }

            _ticks.Add(tick);
        }

        /// <summary>
        ///     Returns the tick of this instance
        /// </summary>
        /// <returns></returns>
        public int GetTick(int index)
        {
            if (index <= 0) throw new ArgumentOutOfRangeException(nameof(index));

            if (_ticks.Count >= 1) return _ticks[index];

            return -1;
        }

        /// <summary>
        ///     Add coordinates to this event
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void AddCoord(int x, int y, int z)
        {
            if (GetNumCoords() < 1)
            {
                Logger.Log("replay event: ticks array should never be smaller than coords array", base.GetType(),
                    ErrorLevel.Debug);
            }
            else
            {
                Logger.Log("replay event: clearing ticks to keep array sizes consistent", base.GetType(),
                    ErrorLevel.Debug);
                _ticks.Clear();
            }

            // TODO: fix params

            /*_coords.Add(x);
            _coords.Add(y);
            _coords.Add(z);*/
        }

        /// <summary>
        ///     Returns the num of coords
        /// </summary>
        /// <returns></returns>
        public int GetNumCoords()
        {
            return _coords.Count;
        }

        /// <summary>
        ///     Returns the X coordinate
        /// </summary>
        /// <returns></returns>
        public int GetCoordX()
        {
            return 0;

            // TODO
        }

        /// <summary>
        ///     Returns the Y coordinate
        /// </summary>
        /// <returns></returns>
        public int GetCoordY()
        {
            return 0;

            // TODO
        }

        /// <summary>
        ///     Returns the param count
        /// </summary>
        /// <returns></returns>
        public int GetParamCount()
        {
            return _params.Count;
        }

        /// <summary>
        ///     Returns the param
        /// </summary>
        /// <returns></returns>
        public int GetParam()
        {
            return 0;

            // TODO
        }

        /// <summary>
        ///     Set the param for this instance
        /// </summary>
        public void SetParam()
        {
            // TODO
        }
    }
}