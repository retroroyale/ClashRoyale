using System;
using System.Net;
using System.Threading.Tasks;
using ClashRoyale.Battles.Core.Network.Cluster.Handlers;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core.Network.Cluster
{
    public class NettyClient
    {
        public MultithreadEventLoopGroup Group { get; set; }
        public Bootstrap Bootstrap { get; set; }

        public NettyClient()
        {
            Group = new MultithreadEventLoopGroup();

            Bootstrap = new Bootstrap();
            Bootstrap.Group(Group);
            Bootstrap.Channel<TcpSocketChannel>();

            Bootstrap
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.SoKeepalive, true)
                .Handler(new LoggingHandler("SRV-ICR"))
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast("ClusterPacketHandler", new ClusterPacketHandler());
                    pipeline.AddLast("ClusterPacketEncoder", new ClusterPacketEncoder());
                }));
        }

        public async Task RunClientAsync()
        {
            try
            {
                var connectedChannel = await Bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9876));
                var endpoint = (IPEndPoint) connectedChannel.LocalAddress;

                Logger.Log(
                    $"Connected to the cluster on {endpoint.Address.MapToIPv4()}:{endpoint.Port}.",
                    GetType());
            }
            catch (Exception)
            {
                Logger.Log(
                    "Failed to connect to the cluster. Retrying in 5sec.",
                    GetType(), ErrorLevel.Warning);

                await Task.Delay(5000);

                await Task.Run(RunClientAsync);
            }
        }
    }
}