using ClashRoyale.Battles.Logic.Session;
using DotNetty.Buffers;
using DotNetty.Transport.Channels.Sockets;

namespace ClashRoyale.Battles.Protocol.Client
{
    public class UdpCheckConnectionMessage : PiranhaMessage
    {
        public UdpCheckConnectionMessage(SessionContext ctx, IByteBuffer reader) : base(ctx, reader)
        {
            Id = 10108;
        }

        public override async void Process()
        {
            var testBuffer = Unpooled.Buffer();
            testBuffer.WriteLong(SessionContext.PlayerId);
            testBuffer.WriteBytes(new byte[2]);
            testBuffer.WriteByte(1);
            testBuffer.WriteByte(1);

            await SessionContext.Channel.WriteAndFlushAsync(new DatagramPacket(testBuffer, SessionContext.EndPoint));
        }
    }
}
