using System.Net;
using System.Threading.Tasks;
using ClashRoyale.Core.Network.Handlers;
using ClashRoyale.Core.Network.Handlers.Cluster;
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

        public MultithreadEventLoopGroup ClusterBossGroup { get; set; }
        public MultithreadEventLoopGroup ClusterWorkerGroup { get; set; }

        public ServerBootstrap ServerBootstrap { get; set; }
        public ServerBootstrap ClusterBootstrap { get; set; }

        public IChannel ServerChannel { get; set; }
        public IChannel ClusterServerChannel { get; set; }

        /// <summary>
        ///     Run the server and optionally the cluster server
        /// </summary>
        /// <returns></returns>
        public async Task RunServerAsync()
        {
            BossGroup = new MultithreadEventLoopGroup();
            WorkerGroup = new MultithreadEventLoopGroup();

            ClusterBossGroup = new MultithreadEventLoopGroup();
            ClusterWorkerGroup = new MultithreadEventLoopGroup();

            // Main Server
            {
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
            }

            // Cluster Server 
            {
                ClusterBootstrap = new ServerBootstrap();
                ClusterBootstrap.Group(ClusterBossGroup, ClusterWorkerGroup);
                ClusterBootstrap.Channel<TcpServerSocketChannel>();

                ClusterBootstrap
                    .Option(ChannelOption.SoBacklog, 100)
                    .Option(ChannelOption.TcpNodelay, true)
                    .Option(ChannelOption.SoKeepalive, true)
                    .Handler(new LoggingHandler("SRV-ICR"))
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddFirst("FrameDecoder", new LengthFieldBasedFrameDecoder(short.MaxValue, 2, 3, 0, 0));
                        //pipeline.AddLast("ReadTimeoutHandler", new ReadTimeoutHandler(60));
                        //pipeline.AddLast("WriteTimeoutHandler", new WriteTimeoutHandler(60));
                        pipeline.AddLast("ClusterPacketHandler", new ClusterPacketHandler());
                        pipeline.AddLast("ClusterPacketEncoder", new ClusterPacketEncoder());
                    }));
            }

            if (Resources.Configuration.UseUdp)
            {
                ClusterServerChannel = await ClusterBootstrap.BindAsync(Resources.Configuration.ClusterServerPort);
                var clusterEndpoint = (IPEndPoint) ClusterServerChannel.LocalAddress;

                Logger.Log(
                    $"Cluster started on {clusterEndpoint.Address.MapToIPv4()}:{clusterEndpoint.Port}.",
                    GetType());
            }

            ServerChannel = await ServerBootstrap.BindAsync(Resources.Configuration.ServerPort);
            var endpoint = (IPEndPoint) ServerChannel.LocalAddress;

            Logger.Log(
                $"Listening on {endpoint.Address.MapToIPv4()}:{endpoint.Port}. Let's play ClashRoyale!",
                GetType());
        }

        /// <summary>
        ///     Close all channels and disconnects clients
        /// </summary>
        /// <returns></returns>
        public async Task Shutdown()
        {
            await ServerChannel.CloseAsync();

            if (Resources.Configuration.UseUdp)
                await ClusterServerChannel.CloseAsync();
        }

        /// <summary>
        ///     Shutdown all workers of netty
        /// </summary>
        /// <returns></returns>
        public async Task ShutdownWorkers()
        {
            await WorkerGroup.ShutdownGracefullyAsync();
            await BossGroup.ShutdownGracefullyAsync();

            await ClusterWorkerGroup.ShutdownGracefullyAsync();
            await ClusterBossGroup.ShutdownGracefullyAsync();
        }
    }
}