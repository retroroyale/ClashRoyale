using System;
using System.Threading.Tasks;
using ClashRoyale.Core.Network.Handlers;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Protocol;
using ClashRoyale.Protocol.Crypto;
using ClashRoyale.Protocol.Messages.Server;
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
        /// <param name="messageHeader"></param>
        /// <returns></returns>
        public async Task Process(PiranhaMessageHeader messageHeader)
        {
            if (messageHeader.Id >= 10000 && messageHeader.Id < 20000)
            {
                if (!LogicScrollMessageFactory.Messages.ContainsKey(messageHeader.Id))
                {
                    Logger.Log($"Message {messageHeader.Id} is not known.", GetType(), ErrorLevel.Warning);

                    await Disconnect();
                    return;
                }

                if (Activator.CreateInstance(LogicScrollMessageFactory.Messages[messageHeader.Id], this,
                    messageHeader.Buffer) is PiranhaMessage message)
                    try
                    {
                        message.Id = messageHeader.Id;
                        message.Length = messageHeader.Length;
                        message.Version = messageHeader.Version;

                        message.Decrypt();
                        message.Decode();
                        message.Process();

                        Logger.Log($"[C] Message {messageHeader.Id} has been handled.", GetType(), ErrorLevel.Debug);

                        if (message.Save && CurrentState == State.Home) Player.Save();
                    }
                    catch (Exception exception)
                    {
                        Logger.Log($"Failed to process {messageHeader.Id}: " + exception, GetType(), ErrorLevel.Error);
                    }
            }
        }

        /// <summary>
        ///     Disconnect a client by sending OutOfSyncMessage
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            await new OutOfSyncMessage(this).Send();
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
            Home = 4
        }

        #endregion Objects
    }
}