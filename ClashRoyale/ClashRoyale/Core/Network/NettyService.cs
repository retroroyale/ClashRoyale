using System.Net;
using System.Threading.Tasks;
using ClashRoyale.Core.Network.Handlers;
using ClashRoyale.Extensions.Utils;
using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
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
                .Option(ChannelOption.SoRcvbuf, 512)
                .Option(ChannelOption.SoSndbuf, 512)
                .Option(ChannelOption.SoKeepalive, true)
                .Handler(new LoggingHandler("SRV-ICR"))
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;          
                    pipeline.AddFirst("FrameDecoder", new LengthFieldBasedFrameDecoder(512, 2, 3, 2, 0));
                    pipeline.AddLast("TimeoutHandler", new TimeoutHandler());
                    pipeline.AddLast("PacketProcessor", new PacketHandler());
                    pipeline.AddLast("PacketEncoder", new PacketEncoder());
                }));

            var boundChannel = await ServerBootstrap.BindAsync(Resources.Configuration.ServerPort);

            Logger.Log(
                $"Listening on {ServerUtils.GetIp4Address()}:{((IPEndPoint) boundChannel.LocalAddress).Port}. Let's play ClashRoyale!",
                GetType());
        }
    }
}