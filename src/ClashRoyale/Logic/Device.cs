using System;
using System.Net;
using ClashRoyale.Core.Network.Handlers;
using ClashRoyale.Logic.Sessions;
using ClashRoyale.Logic.Time;
using ClashRoyale.Protocol;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Crypto;
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
        public void Process(IByteBuffer buffer)
        {
            var id = buffer.ReadUnsignedShort();
            var length = buffer.ReadMedium();
            var version = buffer.ReadUnsignedShort();

            if (id < 10000 || id >= 20000) return;

            if (!LogicScrollMessageFactory.Messages.ContainsKey(id))
            {
                Logger.Log($"Message ID: {id}, V: {version}, L: {length} is not known.", GetType(),
                    ErrorLevel.Warning);
                Disconnect();
                return;
            }

            if (!(Activator.CreateInstance(LogicScrollMessageFactory.Messages[id], this, buffer) is PiranhaMessage
                message)) return;

            try
            {
                if (message.RequiredState != CurrentState && message.RequiredState != State.NotDefinied)
                {
                    Logger.Log($"[C] Message {id} is not allowed in this state!", GetType(),
                        ErrorLevel.Warning);
                    Disconnect();
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

        /// <summary>
        ///     Returns the Ipv4 Address of the client
        /// </summary>
        /// <returns></returns>
        public string GetIp()
        {
            return ((IPEndPoint) Handler.Channel.RemoteAddress).Address.MapToIPv4().ToString();
        }

        /// <summary>
        ///     Disconnect a client by sending OutOfSyncMessage
        /// </summary>
        /// <returns></returns>
        public async void Disconnect()
        {
            await new OutOfSyncMessage(this).SendAsync();

            try
            {
                await Handler.Channel.CloseAsync();
            }
            catch (Exception)
            {
                Logger.Log("Failed to close channel", GetType(), ErrorLevel.Error);
            }
        }

        /// <summary>
        /// Adjusts the ServerTick
        /// </summary>
        /// <param name="tick"></param>
        public void AdjustTick(int tick)
        {
            var secDiff = Math.Abs(tick - ServerTick) / 20;

            LastVisitHome = secDiff > 0 ? LastVisitHome.Subtract(TimeSpan.FromSeconds(secDiff)) : LastVisitHome.AddSeconds(secDiff);
        }

        #region Objects

        public Session Session = new Session();
        public Rc4Core Rc4 = new Rc4Core(Resources.Configuration.EncryptionKey, "nonce");
        public PacketHandler Handler { get; set; }

        public Player Player { get; set; }

        public DateTime LastVisitHome { get; set; }
        public DateTime LastSectorCommand { get; set; }

        public int ServerTick =>
            LogicTime.GetSecondsInTicks((int) DateTime.UtcNow.Subtract(LastVisitHome).TotalSeconds);

        public int SecondsSinceLastCommand => (int) DateTime.UtcNow.Subtract(LastSectorCommand).TotalSeconds;

        public State CurrentState { get; set; }

        public enum State
        {
            Disconnected = 0,
            Login = 1,
            Battle = 2,
            Home = 3,
            NotDefinied = 4
        }

        #endregion Objects
    }
}