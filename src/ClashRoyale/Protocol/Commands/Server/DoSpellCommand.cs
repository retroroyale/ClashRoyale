using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Home.Decks.Items;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Server
{
    public class DoSpellCommand : LogicCommand
    {
        public DoSpellCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Type = 1;
        }

        public int ClientTick { get; set; }
        public int Checksum { get; set; }
        public int SenderHighId { get; set; }
        public int SenderLowId { get; set; }
        public int SpellDeckIndex { get; set; }
        public int SpellIndex { get; set; }
        public int TroopType { get; set; }
        public int TroopId { get; set; }
        public int TroopLevel { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public override void Decode()
        {
            // Header
            {
                ClientTick = Buffer.ReadVInt();
                Checksum = Buffer.ReadVInt();

                SenderHighId = Buffer.ReadVInt();
                SenderLowId = Buffer.ReadVInt();
            }

            SpellDeckIndex = Buffer.ReadVInt();

            TroopType = Buffer.ReadVInt();
            TroopId = Buffer.ReadVInt();

            SpellIndex = Buffer.ReadVInt();

            TroopLevel = Buffer.ReadVInt();

            X = Buffer.ReadVInt();
            Y = Buffer.ReadVInt();
        }

        public override void Encode()
        {
            // Header
            {
                Data.WriteVInt(Type);

                Data.WriteVInt(ClientTick);
                Data.WriteVInt(Checksum);

                Data.WriteVInt(SenderHighId);
                Data.WriteVInt(SenderLowId);
            }

            Data.WriteVInt(SpellDeckIndex);

            Data.WriteVInt(TroopType);
            Data.WriteVInt(TroopId);

            Data.WriteVInt(SpellIndex);
        }

        public override void Process()
        {
            var battle = Device.Player.Battle;

            if (battle != null)
            {
                var data = new byte[Data.ReadableBytes];
                Data.GetBytes(Data.ReaderIndex, data);

                var buffer = Unpooled.Buffer();
                {
                    buffer.WriteBuffer(data);

                    buffer.WriteVInt(TroopLevel);

                    buffer.WriteVInt(X);
                    buffer.WriteVInt(Y);

                    battle.GetOwnQueue(Device.Player.Home.Id).Enqueue(buffer.Array);
                }

                var attackBuffer = Unpooled.Buffer();
                {
                    attackBuffer.WriteBuffer(data);

                    attackBuffer.WriteVInt(1); // IsAttack
                    {
                        attackBuffer.WriteVInt(Card.Id(TroopType, TroopId));
                    }

                    attackBuffer.WriteVInt(TroopLevel);

                    attackBuffer.WriteVInt(X);
                    attackBuffer.WriteVInt(Y);

                    battle.GetEnemyQueue(Device.Player.Home.Id).Enqueue(attackBuffer.Array);
                }
            }
        }
    }
}