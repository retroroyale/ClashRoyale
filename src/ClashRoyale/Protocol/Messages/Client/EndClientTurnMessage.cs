using System;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class EndClientTurnMessage : PiranhaMessage
    {
        public EndClientTurnMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14102;
            RequiredState = Device.State.NotDefinied;
        }

        public int Tick { get; set; }
        public int Count { get; set; }
        public int Checksum { get; set; }

        public override void Decode()
        {
            Tick = Reader.ReadVInt();
            Checksum = Reader.ReadVInt();
            Count = Reader.ReadVInt();
        }

        public override async void Process()
        {
            if (Tick < 0)
            {
                Logger.Log($"Client Tick ({Tick}) is corrupted. Disconnecting.", GetType(), ErrorLevel.Warning);
                Device.Disconnect();
                return;
            }

            if (Device.CurrentState < Device.State.Battle)
            {
                if (Math.Abs(Tick - Device.ServerTick) > 500)
                {
                    Logger.Log($"OutOfSync! Client Tick: {Tick}, Server Tick: {Device.ServerTick}", GetType(),
                        ErrorLevel.Warning);
                    Device.Disconnect();
                    return;
                }
            }
            else
            {
                Device.AdjustTick(Tick);
            }

            if (Count < 0 && Count > 128) return;

            var home = Device.Player.Home;
            var cardCount = home.Deck.Count - 8;
            var oldChestOpenedCount = home.ChestsOpened;

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
                            command.Decode();
                            command.Process();

                            Logger.Log(
                                $"Command {type} ({command.GetType().Name}) with Tick {command.Tick} has been processed.",
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

            var serverChecksum = home.ChestsOpened == oldChestOpenedCount
                ? ((home.Deck.Count - 8) << 16) | home.ChestsOpened
                : (cardCount << 16) | home.ChestsOpened;

            if (Device.CurrentState == Device.State.Home && serverChecksum != Checksum)
            {
                await new OutOfSyncMessage(Device).SendAsync();
                Console.WriteLine($"Checksum: {Checksum}, Server: {serverChecksum}");
            }
        }
    }
}