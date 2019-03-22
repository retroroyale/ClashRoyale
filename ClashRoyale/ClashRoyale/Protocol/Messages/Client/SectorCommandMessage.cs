using System;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class SectorCommandMessage : PiranhaMessage
    {
        public SectorCommandMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 12904;
        }

        public int Tick { get; set; }
        public int Count { get; set; }

        public override void Decode()
        {
            Buffer.ReadVInt();
            Tick = Buffer.ReadVInt();
            Count = Buffer.ReadVInt();
        }

        public override void Process()
        {
            Device.LastSectorCommand = DateTime.UtcNow;

            if (Count >= 0 && Count <= 512)
            {
                for (var index = 0; index < Count; index++)
                    {
                        var type = Buffer.ReadVInt();

                        if (type < 0) break;

                        if (LogicCommandManager.Commands.ContainsKey(type))
                            try
                            {
                                if (Activator.CreateInstance(LogicCommandManager.Commands[type], Device,
                                        Buffer) is
                                    LogicCommand
                                    command)
                                {
                                    command.Decode();

                                    command.Encode();
                                    command.Process();

                                    Logger.Log($"BattleCommand {type} with Tick {Tick} has been processed.",
                                        GetType(), ErrorLevel.Debug);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                Logger.Log(
                                    $"The operation for command {type} was aborted after 2 second(s).", GetType(),
                                    ErrorLevel.Warning);
                            }
                            catch (Exception exception)
                            {
                                Logger.Log(exception, GetType(), ErrorLevel.Error);
                            }
                        else
                            Logger.Log($"BattleCommand {type} is unhandled.", GetType(), ErrorLevel.Warning);
                    }
            }
        }
    }
}