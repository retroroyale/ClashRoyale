using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class ClientCapabilitiesMessage : PiranhaMessage
    {
        public ClientCapabilitiesMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10107;
        }

        public int Ping { get; set; }
        public string ConnectionInterface { get; set; }

        public override void Decode()
        {
            Ping = Buffer.ReadVInt();
            ConnectionInterface = Buffer.ReadScString();
        }
    }
}