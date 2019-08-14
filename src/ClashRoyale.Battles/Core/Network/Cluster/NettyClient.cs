using System;
using System.Net;
using System.Threading.Tasks;
using ClashRoyale.Battles.Core.Network.Cluster.Handlers;
using DotNetty.Codecs;
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

        public async Task RunClientAsync()
        {
            try
            {
                Group = new MultithreadEventLoopGroup();

                Bootstrap = new Bootstrap();
                Bootstrap.Group(Group);
                Bootstrap.Channel<TcpSocketChannel>();

                Bootstrap
                    .Option(ChannelOption.TcpNodelay, true)
                    .Option(ChannelOption.SoKeepalive, true)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddFirst("FrameDecoder", new LengthFieldBasedFrameDecoder(512, 2, 3));
                        pipeline.AddLast("ClusterPacketHandler", new ClusterPacketHandler());
                        pipeline.AddLast("ClusterPacketEncoder", new ClusterPacketEncoder());
                    }));

                var connectedChannel =
                    await Bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9876));
                var endpoint = (IPEndPoint) connectedChannel.LocalAddress;

                Logger.Log(
                    $"Connected to the cluster on {endpoint.Address.MapToIPv4()}:{endpoint.Port}.",
                    GetType());

                Resources.ClusterClient.Login();
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