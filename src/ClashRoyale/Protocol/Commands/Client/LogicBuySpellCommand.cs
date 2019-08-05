using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
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

            Reader.ReadVInt();
            Reader.ReadVInt();

            Reader.ReadVInt();
            Reader.ReadVInt();

            Reader.ReadVInt();

            Amount = Reader.ReadVInt();
            ClassId = Reader.ReadVInt();
            InstanceId = Reader.ReadVInt();
            Index = Reader.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.Shop.BuyItem(Amount, ClassId, InstanceId, Index);
        }
    }
}