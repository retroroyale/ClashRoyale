using ClashRoyale.Logic;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicFuseSpellsCommand : LogicCommand
    {
        public LogicFuseSpellsCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int ClassId { get; set; }
        public int InstanceId { get; set; }

        public override void Decode()
        {
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            ClassId = Reader.ReadVInt();
            InstanceId = Reader.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.Deck.UpgradeCard(ClassId, InstanceId);
        }
    }
}