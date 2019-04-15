using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class RequestSectorStateMessage : PiranhaMessage
    {
        public RequestSectorStateMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 12903;
        }

        public int LastTick { get; set; }

        public override void Decode()
        {
            LastTick = Reader.ReadVInt();
        }
    }
}