using System;
using ClashRoyale.Battles.Logic.Session;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Protocol.Messages.Client
{
    public class SectorCommandMessage : PiranhaMessage
    {
        public SectorCommandMessage(SessionContext ctx, IByteBuffer reader) : base(ctx, reader)
        {
            Id = 12904;
        }

        public int Tick { get; set; }
        public int Count { get; set; }

        public override void Decode()
        {
            Reader.ReadVInt();
            Tick = Reader.ReadVInt();
            Count = Reader.ReadVInt();
        }

        public override void Process()
        {
            SessionContext.BattleActive = true;

            if (Count < 0 || Count > 128) return;

            Logger.Log($"Tick: {Tick}", GetType(), ErrorLevel.Warning);

            /*var battle = Device.Player.Battle;
            if (battle != null)
                if (!battle.IsRunning)
                    battle.BattleTimer.Start();*/

            for (var i = 0; i < Count; i++)
            {
                var type = Reader.ReadVInt();

                if (type >= 500) break;

                if (LogicCommandManager.Commands.ContainsKey(type))
                    try
                    {
                        if (Activator.CreateInstance(LogicCommandManager.Commands[type], SessionContext,
                                Reader) is
                            LogicCommand
                            command)
                        {
                            command.Decode();
                            command.Encode();
                            command.Process();

                            Logger.Log($"SectorCommand {type} with Tick {Tick} has been processed.",
                                GetType(), ErrorLevel.Debug);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception, GetType(), ErrorLevel.Error);
                    }
                else
                    Logger.Log($"SectorCommand {type} is unhandled.", GetType(), ErrorLevel.Warning);
            }
        }
    }
}