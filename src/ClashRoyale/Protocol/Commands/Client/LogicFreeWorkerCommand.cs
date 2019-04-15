using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicFreeWorkerCommand : LogicCommand
    {
        public LogicFreeWorkerCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public int ClassId { get; set; }
        public int InstanceId { get; set; }

        public override void Decode()
        {
            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt();

            ClassId = Buffer.ReadVInt();
            InstanceId = Buffer.ReadVInt();
        }

        public override void Process()
        {
            Device.Player.Home.Deck.SawCard(ClassId, InstanceId);
        }
    }
}