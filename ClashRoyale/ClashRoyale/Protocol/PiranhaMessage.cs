using System;
using System.Threading.Tasks;
using ClashRoyale.Logic;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Protocol
{
    public class PiranhaMessage
    {
        public PiranhaMessage(Device device)
        {
            Device = device;
            Packet = Unpooled.Buffer();
        }

        public PiranhaMessage(Device device, IByteBuffer buffer)
        {
            Device = device;
            Buffer = buffer;
        }

        public IByteBuffer Packet { get; set; }
        public IByteBuffer Buffer { get; set; }
        public Device Device { get; set; }
        public ushort Id { get; set; }
        public int Length { get; set; }
        public ushort Version { get; set; }
        public bool Save { get; set; }

        public virtual void Decrypt()
        {
            if (Length > 0)
            {
                var buffer = Buffer.ReadBytes(Length).Array;

                Device.Rc4.Decrypt(ref buffer);

                Buffer = Unpooled.CopiedBuffer(buffer);
                Length = (ushort) buffer.Length;
            }
        }

        public virtual void Encrypt()
        {
            if (Packet.ReadableBytes > 0)
            {
                var buffer = Packet.ReadBytes(Packet.ReadableBytes).Array;

                Device.Rc4.Encrypt(ref buffer);

                Packet = Unpooled.CopiedBuffer(buffer);
                Length = (ushort) buffer.Length;
            }
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
        /// Writes this message to the clients channel
        /// </summary>
        /// <returns></returns>
        public async Task Send()
        {
            try
            {
                await Device.Handler.Channel.WriteAndFlushAsync(this);

                Logger.Log($"[S] Message {Id} has been sent.", GetType(), ErrorLevel.Debug);
            }
            catch (Exception)
            {
                Logger.Log($"[S] Failed to send {Id}.", GetType(), ErrorLevel.Debug);
            }
        }
    }
}