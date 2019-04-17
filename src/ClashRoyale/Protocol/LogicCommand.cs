using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol
{
    public class LogicCommand
    {
        public LogicCommand(Device device)
        {
            Device = device;
            Data = Unpooled.Buffer();
        }

        public LogicCommand(Device device, IByteBuffer buffer)
        {
            Device = device;
            Buffer = buffer;
            Data = Unpooled.Buffer();
        }

        public IByteBuffer Data { get; set; }
        public Device Device { get; set; }

        public int Type { get; set; }
        public int Tick { get; set; }
        public IByteBuffer Buffer { get; set; }

        public virtual void Decode()
        {
            Tick = Buffer.ReadVInt();
            Buffer.ReadVInt();
        }

        public virtual void Encode()
        {
        }

        public virtual void Process()
        {
        }
    }
}