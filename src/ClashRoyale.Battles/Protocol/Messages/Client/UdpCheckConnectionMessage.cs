using ClashRoyale.Battles.Logic.Session;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Protocol.Messages.Client
{
    public class UdpCheckConnectionMessage : PiranhaMessage
    {
        public UdpCheckConnectionMessage(SessionContext ctx, IByteBuffer reader) : base(ctx, reader)
        {
            Id = 10108;
        }
    }
}