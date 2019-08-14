using System;
using System.Net;
using ClashRoyale.Core.Cluster.Protocol;
using ClashRoyale.Core.Network.Handlers.Cluster;
using ClashRoyale.Utilities.Crypto;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Core.Cluster
{
    public class Server
    {
        public Server(ClusterPacketHandler handler)
        {
            Handler = handler;
        }

        /// <summary>
        ///     Process the buffer sent by the client
        /// </summary>
        /// <param name="buffer"></param>
        public void Process(IByteBuffer buffer)
        {
            var id = buffer.ReadUnsignedShort();
            var length = buffer.ReadMedium();

            if (id < 10000 || id >= 20000) return;

            if (!ClusterMessageFactory.Messages.ContainsKey(id))
            {
                Logger.Log($"Message ID: {id}, L: {length} is not known.", GetType(),
                    ErrorLevel.Warning);
                return;
            }

            if (Activator.CreateInstance(ClusterMessageFactory.Messages[id], this, buffer) is ClusterMessage
                message
            )
                try
                {
                    message.Id = id;
                    message.Length = length;

                    message.Decrypt();
                    message.Decode();
                    message.Process();

                    Logger.Log($"[C] Message {id} ({message.GetType().Name}) handled.", GetType(),
                        ErrorLevel.Debug);
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

        #region Objects

        public ClusterPacketHandler Handler { get; set; }
        public Rc4Core Rc4 = new Rc4Core(Resources.Configuration.ClusterKey, Resources.Configuration.ClusterNonce);
        public ServerInfo ServerInfo { get; set; }

        #endregion Objects
    }
}