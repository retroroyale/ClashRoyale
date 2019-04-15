using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForTvContentMessage : PiranhaMessage
    {
        public AskForTvContentMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14402;
        }

        public int ClassId { set; get; }
        public int InstanceId { set; get; }

        public override void Decode()
        {
            ClassId = Reader.ReadVInt();
            InstanceId = Reader.ReadVInt();
        }

        public override async void Process()
        {
            await new RoyalTvContentMessage(Device)
            {
                ClassId = ClassId,
                InstanceId = InstanceId
            }.SendAsync();
        }
    }
}