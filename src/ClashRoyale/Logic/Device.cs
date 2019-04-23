using System;
using System.Threading.Tasks;
using ClashRoyale.Core.Network.Handlers;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Protocol;
using ClashRoyale.Protocol.Crypto;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Logic
{
    public class Device
    {
        public Device(PacketHandler handler)
        {
            Handler = handler;
            CurrentState = State.Disconnected;
        }

        public bool IsConnected => Handler.Channel.Registered;

        /// <summary>
        ///     Process a message
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public async Task ProcessAsync(IByteBuffer buffer)
        {
            var id = buffer.ReadUnsignedShort();
            var length = buffer.ReadMedium();
            var version = buffer.ReadUnsignedShort();

            if (id >= 10000 && id < 20000)
            {
                if (!LogicScrollMessageFactory.Messages.ContainsKey(id))
                {
                    Logger.Log($"Message ID: {id}, V: {version}, L: {length} is not known.", GetType(),
                        ErrorLevel.Warning);

                    await DisconnectAsync();
                    return;
                }

                if (Activator.CreateInstance(LogicScrollMessageFactory.Messages[id], this, buffer) is PiranhaMessage
                    message
                )
                    try
                    {
                        if (message.RequiredState != CurrentState && message.RequiredState != State.NotDefinied)
                        {
                            Logger.Log($"[C] Message {id} is not allowed in this state!", GetType(),
                                ErrorLevel.Warning);
                            await DisconnectAsync();
                            return;
                        }

                        message.Id = id;
                        message.Length = length;
                        message.Version = version;

                        message.Decrypt();
                        message.Decode();
                        message.Process();

                        Logger.Log($"[C] Message {id} ({message.GetType().Name}) handled.", GetType(),
                            ErrorLevel.Debug);

                        if (message.Save && CurrentState == State.Home) Player.Save();
                    }
                    catch (Exception exception)
                    {
                        Logger.Log($"Failed to process {id}: " + exception, GetType(), ErrorLevel.Error);
                    }
            }
        }

        /// <summary>
        ///     Disconnect a client by sending OutOfSyncMessage
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsync()
        {
            await new OutOfSyncMessage(this).SendAsync();
        }

        #region Objects

        public Rc4Core Rc4 = new Rc4Core(Resources.Configuration.EncryptionKey, "nonce");
        public PacketHandler Handler { get; set; }

        public Player Player { get; set; }

        public DateTime LastVisitHome { get; set; }
        public DateTime LastSectorCommand { get; set; }

        public int ServerTick => TimeUtils.ToTick(DateTime.UtcNow.Subtract(LastVisitHome));
        public int SecondsSinceLastCommand => (int) DateTime.UtcNow.Subtract(LastSectorCommand).TotalSeconds;

        public State CurrentState { get; set; }

        public enum State
        {
            Disconnected = 0,
            Login = 1,
            Replay = 2,
            Battle = 3,
            Home = 4,
            NotDefinied = 5
        }

        #endregion Objects
    }
}