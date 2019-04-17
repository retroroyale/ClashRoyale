using System;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class EndClientTurnMessage : PiranhaMessage
    {
        public EndClientTurnMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14102;
        }

        public int Tick { get; set; }
        public int Count { get; set; }

        public override void Decode()
        {
            Tick = Reader.ReadVInt();
            Reader.ReadVInt();
            Count = Reader.ReadVInt();
        }

        public override async void Process()
        {
            if (Tick < 0)
            {
                Logger.Log($"Client Tick ({Tick}) is corrupted. Disconnecting.", GetType(), ErrorLevel.Warning);
                await Device.DisconnectAsync();
                return;
            }

            if (Math.Abs(Tick - Device.ServerTick) > 500)
            {
                Logger.Log($"OutOfSync! Client Tick: {Tick}, Server Tick: {Device.ServerTick}", GetType(), ErrorLevel.Debug);
                await Device.DisconnectAsync();
                return;
            }

            if (Count < 0 && Count > 128)
            {
                return;
            }

            for (var i = 0; i < Count; i++)
            {
                var type = Reader.ReadVInt();

                if (type < 500) break;

                if (LogicCommandManager.Commands.ContainsKey(type))
                {
                    try
                    {
                        if (Activator.CreateInstance(LogicCommandManager.Commands[type], Device,
                                Reader) is
                            LogicCommand
                            command)
                        {
                            command.Tick = Reader.ReadVInt();
                            Reader.ReadVInt();

                            command.Decode();
                            command.Process();

                            Logger.Log($"Command {type} with Tick {command.Tick} has been processed.",
                                GetType(), ErrorLevel.Debug);

                            Save = true;
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(exception, GetType(), ErrorLevel.Error);
                    }
                }
                else
                {
                    Logger.Log(
                        $"Command {type} is unhandled.",
                        GetType(), ErrorLevel.Warning);
                    break;
                }
            }
        }
    }
}
