using System;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
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
            Tick = Buffer.ReadVInt();
            Buffer.ReadVInt();
            Count = Buffer.ReadVInt();

            Logger.Log($"Client Tick: {Tick}, Server Tick: {Device.ServerTick}", GetType(), ErrorLevel.Debug);
        }

        public override async void Process()
        {
            if (Tick < 0)
            {
                Logger.Log($"Client Tick ({Tick}) is corrupted. Disconnecting.", GetType(), ErrorLevel.Warning);
                await new OutOfSyncMessage(Device).Send();
            }

            if (Count >= 0 && Count <= 512)
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
                                command.Tick = Buffer.ReadVInt();
                                Buffer.ReadVInt();

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
                    else
                        Logger.Log(
                            $"Command {type} is unhandled.\nData: {BitConverter.ToString(Buffer.ReadBytes(Buffer.ReadableBytes).Array).Replace("-", "")}",
                            GetType(), ErrorLevel.Warning);
                }
        }
    }
}