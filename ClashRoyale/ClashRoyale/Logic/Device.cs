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
        public enum State
        {
            Disconnected = 0,
            Login = 1,
            Replay = 2,
            Battle = 3,
            Home = 4
        }

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
        public async Task Process(IByteBuffer buffer)
        {
            while (true)
            {
                if (buffer.ReadableBytes >= 7)
                {
                    var identifier = buffer.ReadUnsignedShort();
                    var length = (ushort) buffer.ReadUnsignedMedium();

                    if (identifier >= 10000 && identifier < 20000)
                    {
                        if (!LogicScrollMessageFactory.Messages.ContainsKey(identifier))
                        {
                            Logger.Log($"Message {identifier} is not known.", GetType(), ErrorLevel.Warning);

                            await Disconnect();
                            break;
                        }

                        if (Activator.CreateInstance(LogicScrollMessageFactory.Messages[identifier], this, buffer) is PiranhaMessage message)
                            try
                            {
                                message.Id = identifier;
                                message.Length = length;
                                message.Version = buffer.ReadUnsignedShort();

                                message.Decrypt();
                                message.Decode();
                                message.Process();

                                Logger.Log($"[C] Message {identifier} has been handled.", GetType(), ErrorLevel.Debug);

                                if (message.Save && CurrentState == State.Home) Player.Save();
                            }
                            catch (Exception exception)
                            {
                                Logger.Log($"Failed to process {identifier}: " + exception, GetType(), ErrorLevel.Error);
                            }

                        if (buffer.ReadableBytes >= 7) continue;
                    }
                    else
                    {
                        await Disconnect();
                    }
                }

                break;
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

        public Rc4Core Rc4 = new Rc4Core("fhsd6f86f67rt8fw78fw789we78r9789wer6re", "nonce");
        public PacketHandler Handler { get; set; }

        public Player Player { get; set; }

        public DateTime LastVisitHome { get; set; }
        public DateTime LastSectorCommand { get; set; }

        public int ServerTick => TimeUtils.ToTick(DateTime.UtcNow.Subtract(LastVisitHome));
        public int SecondsSinceLastCommand => (int) DateTime.UtcNow.Subtract(LastSectorCommand).TotalSeconds;

        public State CurrentState { get; set; }

        #endregion Objects
    }
}