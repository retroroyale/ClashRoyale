using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Core.Cluster.Protocol
{
    public class ClusterMessage
    {
        /// <summary>
        ///     Client Message
        /// </summary>
        /// <param name="server"></param>
        public ClusterMessage(Node server)
        {
            Server = server;
            Writer = Unpooled.Buffer(5);
        }

        /// <summary>
        ///     Server Message
        /// </summary>
        /// <param name="server"></param>
        /// <param name="buffer"></param>
        public ClusterMessage(Node server, IByteBuffer buffer)
        {
            Server = server;
            Reader = buffer;
        }

        public IByteBuffer Writer { get; set; }
        public IByteBuffer Reader { get; set; }
        public Node Server { get; set; }
        public ushort Id { get; set; }
        public int Length { get; set; }

        /// <summary>
        ///     Decrypts the current message
        /// </summary>
        public virtual void Decrypt()
        {
            if (Length <= 0) return;

            var buffer = Reader.ReadBytes(Length);

            Server.Rc4.Decrypt(ref buffer);

            Reader = buffer;
            Length = buffer.ReadableBytes;
        }

        /// <summary>
        ///     Encrypts the current message
        /// </summary>
        public virtual void Encrypt()
        {
            if (Writer.ReadableBytes <= 0) return;

            var buffer = Writer;

            Server.Rc4.Encrypt(ref buffer);

            Length = buffer.ReadableBytes;
        }

        /// <summary>
        ///     Decodes the current message
        /// </summary>
        public virtual void Decode()
        {
        }

        /// <summary>
        ///     Encodes the current message
        /// </summary>
        public virtual void Encode()
        {
        }

        /// <summary>
        ///     Processes the current message
        /// </summary>
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