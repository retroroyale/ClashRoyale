using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Sector
{
    public class RequestSectorStateMessage : PiranhaMessage
    {
        public RequestSectorStateMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 12903;
            RequiredState = Device.State.Battle;
        }

        public int LastTick { get; set; }

        public override void Decode()
        {
            LastTick = Reader.ReadVInt();
        }
    }
}