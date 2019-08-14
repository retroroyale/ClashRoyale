using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;
using SharpRaven.Data;

namespace ClashRoyale.Battles.Core.Network.Cluster.Protocol.Messages.Server
{
    public class ConnectionFailedMessage : ClusterMessage
    {
        public ConnectionFailedMessage(IByteBuffer buffer) : base(buffer)
        {
            Id = 20103;
        }

        public int Error { get; set; }

        public override void Decode()
        {
            Error = Reader.ReadVInt();
        }

        public override void Process()
        {
            switch (Error)
            {
                case 1:
                {
                    Logger.Log("The server was unable to decrypt the last message.", null, ErrorLevel.Error);
                    break;
                }

                default:
                {
                    Logger.Log($"A unknown error ({Error}) occured.", null, ErrorLevel.Error);
                    break;
                }
            }
        }
    }
}