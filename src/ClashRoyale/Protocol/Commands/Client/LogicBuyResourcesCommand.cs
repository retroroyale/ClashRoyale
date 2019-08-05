using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicBuyResourcesCommand : LogicCommand
    {
        public LogicBuyResourcesCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int ResourceInstanceId { get; set; }

        public override void Decode()
        {
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            Reader.ReadVInt(); // ResourceClassId
            ResourceInstanceId = Reader.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.BuyResourcePack(ResourceInstanceId);
        }
    }
}