using System;
using ClashRoyale.Battles.Core.Network.Cluster.Handlers;
using ClashRoyale.Battles.Core.Network.Cluster.Protocol;
using ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Client;
using ClashRoyale.Utilities.Crypto;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core.Network.Cluster
{
    public class ClusterClient
    {
        public void Process(IByteBuffer buffer)
        {
            var id = buffer.ReadUnsignedShort();
            var length = buffer.ReadMedium();

            if (id < 20000 || id >= 30000) return;

            if (!ClusterMessageFactory.Messages.ContainsKey(id))
            {
                Logger.Log($"Message ID: {id}, L: {length} is not known.", GetType(),
                    ErrorLevel.Warning);
                return;
            }

            if (!(Activator.CreateInstance(ClusterMessageFactory.Messages[id], buffer) is ClusterMessage
                message)) return;

            try
            {
                message.Id = id;
                message.Length = length;

                if (id != 20103)
                    message.Decrypt();

                message.Decode();
                message.Process();

                Logger.Log($"[S] Message {id} ({message.GetType().Name}) handled.", GetType(),
                    ErrorLevel.Debug);
            }
            catch (Exception exception)
            {
                Logger.Log($"Failed to process {id}: " + exception, GetType(), ErrorLevel.Error);
            }
        }

        public async void Login()
        {
            Rc4 = new Rc4Core(Resources.Configuration.ClusterKey, Resources.Configuration.ClusterNonce);
            await new ConnectionCheckMessage().SendAsync();
        }

        #region Objects

        public Rc4Core Rc4 { get; set; }
        public ClusterPacketHandler Handler { get; set; }

        #endregion Objects
    }
}