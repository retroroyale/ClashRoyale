using System.Net;
using System.Threading.Tasks;
using ClashRoyale.Core.Network.Handlers;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace ClashRoyale.Core.Network
{
    public class NettyService
    {
        public MultithreadEventLoopGroup BossGroup { get; set; }
        public MultithreadEventLoopGroup WorkerGroup { get; set; }

        public ServerBootstrap ServerBootstrap { get; set; }

        public async Task RunServerAsync()
        {
            BossGroup = new MultithreadEventLoopGroup();
            WorkerGroup = new MultithreadEventLoopGroup();

            ServerBootstrap = new ServerBootstrap();
            ServerBootstrap.Group(BossGroup, WorkerGroup);
            ServerBootstrap.Channel<TcpServerSocketChannel>();

            ServerBootstrap
                .Option(ChannelOption.SoBacklog, 100)
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Handler(new LoggingHandler("SRV-ICR"))
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddFirst("FrameDecoder", new LengthFieldBasedFrameDecoder(512, 2, 3, 2, 0));
                    pipeline.AddLast("ReadTimeoutHandler", new ReadTimeoutHandler(20));
                    pipeline.AddLast("WriteTimeoutHandler", new WriteTimeoutHandler(20));
                    pipeline.AddLast("PacketHandler", new PacketHandler());
                    pipeline.AddLast("PacketEncoder", new PacketEncoder());
                }));

            var boundChannel = await ServerBootstrap.BindAsync(Resources.Configuration.ServerPort);
            var endpoint = (IPEndPoint) boundChannel.LocalAddress;

            Logger.Log(
                $"Listening on {endpoint.Address.MapToIPv4()}:{endpoint.Port}. Let's play ClashRoyale!",
                GetType());
        }
    }
}