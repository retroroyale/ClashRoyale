using System.Net;
using System.Threading.Tasks;
using ClashRoyale.Battles.Core.Network.Handlers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ClashRoyale.Battles.Core.Network
{
    public class NettyService
    {
        public MultithreadEventLoopGroup Group { get; set; }
        public Bootstrap Bootstrap { get; set; }

        public async Task RunServerAsync()
        {
            Group = new MultithreadEventLoopGroup();

            Bootstrap = new Bootstrap();
            Bootstrap.Group(Group);
            Bootstrap.Channel<SocketDatagramChannel>();

            Bootstrap
                .Option(ChannelOption.SoBroadcast, true)
                .Handler(new LoggingHandler("SRV-ICR"))
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddFirst("FrameDecoder", new LengthFieldBasedFrameDecoder(512, 2, 3, 2, 0));
                    pipeline.AddLast("ReadTimeoutHandler", new ReadTimeoutHandler(20));
                    pipeline.AddLast("WriteTimeoutHandler", new WriteTimeoutHandler(20));
                    pipeline.AddLast("PacketHandler", new PacketHandler());
                }));

            var boundChannel = await Bootstrap.BindAsync(9449);
            var endpoint = (IPEndPoint) boundChannel.LocalAddress;

            Logger.Log(
                $"Listening on {endpoint.Address.MapToIPv4()}:{endpoint.Port}. Time to fight!",
                GetType());
        }
    }
}