using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicBuySpellCommand : LogicCommand
    {
        public LogicBuySpellCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int Amount { get; set; }
        public int ClassId { get; set; }
        public int InstanceId { get; set; }
        public int Index { get; set; }

        public override void Decode()
        {
            base.Decode();

            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt();

            Amount = Buffer.ReadVInt();
            ClassId = Buffer.ReadVInt();
            InstanceId = Buffer.ReadVInt();
            Index = Buffer.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.Shop.BuyItem(Amount, ClassId, InstanceId, Index);
        }
    }
}