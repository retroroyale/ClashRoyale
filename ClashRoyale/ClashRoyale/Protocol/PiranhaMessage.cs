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
            var buffer = Buffer.ReadBytes(Length).Array;

            Device.Rc4.Decrypt(ref buffer);

            Buffer = Unpooled.CopiedBuffer(buffer);
            Length = (ushort)buffer.Length;
        }

        public virtual void Encrypt()
        {
            var buffer = new byte[Packet.ReadableBytes];
            Packet.GetBytes(0, buffer);

            Device.Rc4.Encrypt(ref buffer);

            Packet = Unpooled.CopiedBuffer(buffer);
            Length = (ushort)buffer.Length;
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
        ///     Send the current message to the client by encoding and encrypting it
        /// </summary>
        /// <returns></returns>
        public async Task Send()
        {
            Encode();
            Encrypt();

            var buffer = Unpooled.Buffer();

            Length = (ushort) Packet.ReadableBytes;

            buffer.WriteUnsignedShort(Id);
            buffer.WriteMedium(Length);
            buffer.WriteUnsignedShort(Version);

            buffer.WriteBytes(Packet);

            await Device.Handler.Channel.WriteAndFlushAsync(buffer);

            Logger.Log($"[S] Message {Id} has been sent.", GetType(), ErrorLevel.Debug);
        }
    }
}