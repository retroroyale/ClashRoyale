using ClashRoyale.Extensions;
using ClashRoyale.Logic;
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

            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt(); // ResourceClassId
            ResourceInstanceId = Buffer.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.BuyResourcePack(ResourceInstanceId);
        }
    }
}