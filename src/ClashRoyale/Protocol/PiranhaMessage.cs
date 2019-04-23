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
            Writer = Unpooled.Buffer();
        }

        public PiranhaMessage(Device device, IByteBuffer buffer)
        {
            Device = device;
            Reader = buffer;
        }

        public IByteBuffer Writer { get; set; }
        public IByteBuffer Reader { get; set; }
        public Device Device { get; set; }
        public Device.State RequiredState = Device.State.Home;
        public ushort Id { get; set; }
        public int Length { get; set; }
        public ushort Version { get; set; }
        public bool Save { get; set; }

        public virtual void Decrypt()
        {
            if (Length <= 0) return;

            var buffer = Reader.ReadBytes(Length).Array;

            Device.Rc4.Decrypt(ref buffer);

            Reader = Unpooled.CopiedBuffer(buffer);
            Length = (ushort) buffer.Length;
        }

        public virtual void Encrypt()
        {
            if (Writer.ReadableBytes <= 0) return;

            var buffer = Writer.ReadBytes(Writer.ReadableBytes).Array;

            Device.Rc4.Encrypt(ref buffer);

            Writer = Unpooled.CopiedBuffer(buffer);
            Length = (ushort) buffer.Length;
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
                await Device.Handler.Channel.WriteAndFlushAsync(this);

                Logger.Log($"[S] Message {Id} ({GetType().Name}) sent.", GetType(), ErrorLevel.Debug);
            }
            catch (Exception)
            {
                Logger.Log($"[S] Failed to send {Id}.", GetType(), ErrorLevel.Debug);
            }
        }
    }
}