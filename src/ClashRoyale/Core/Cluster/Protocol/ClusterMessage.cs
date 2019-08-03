using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Core.Cluster.Protocol
{
    public class ClusterMessage
    {
        public ClusterMessage(Server server)
        {
            Server = server;
            Writer = Unpooled.Buffer(5);
        }

        public ClusterMessage(Server server, IByteBuffer buffer)
        {
            Server = server;
            Reader = buffer;
        }

        public IByteBuffer Writer { get; set; }
        public IByteBuffer Reader { get; set; }
        public Server Server { get; set; }
        public ushort Id { get; set; }
        public int Length { get; set; }

        public virtual void Decrypt()
        {
            if (Length <= 0) return;

            var buffer = Reader.ReadBytes(Length);

            Server.Rc4.Decrypt(ref buffer);

            Reader = buffer;
            Length = buffer.ReadableBytes;
        }

        public virtual void Encrypt()
        {
            if (Writer.ReadableBytes <= 0) return;

            var buffer = Writer;

            Server.Rc4.Encrypt(ref buffer);

            Length = buffer.ReadableBytes;
        }

        public virtual void Decode()
        {
        }

        public virtual void Encode()
        {
        }

        public virtual void Process()
        {
        }

        /// <summary>
        ///     Writes this message to the clients channel
        /// </summary>
        /// <returns></returns>
        public async Task SendAsync()
        {
            try
            {
                await Server.Handler.Channel.WriteAndFlushAsync(this);

                Logger.Log($"[S] Message {Id} ({GetType().Name}) sent.", GetType(), ErrorLevel.Debug);
            }
            catch (Exception)
            {
                Logger.Log($"[S] Failed to send {Id}.", GetType(), ErrorLevel.Debug);
            }
        }
    }
}