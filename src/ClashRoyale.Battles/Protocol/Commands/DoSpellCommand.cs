using ClashRoyale.Battles.Logic.Session;
using ClashRoyale.Utilities.Netty;
using ClashRoyale.Utilities.Utils;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Protocol.Commands
{
    public class DoSpellCommand : LogicCommand
    {
        public DoSpellCommand(SessionContext ctx, IByteBuffer buffer) : base(ctx, buffer)
        {
            Type = 1;
        }

        public int ClientTick { get; set; }
        public int Checksum { get; set; }
        public int SenderHighId { get; set; }
        public int SenderLowId { get; set; }
        public int SpellDeckIndex { get; set; }
        public int SpellIndex { get; set; }
        public int ClassId { get; set; }
        public int InstanceId { get; set; }
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

            ClassId = Buffer.ReadVInt();
            InstanceId = Buffer.ReadVInt();

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

            Data.WriteVInt(ClassId);
            Data.WriteVInt(InstanceId);

            Data.WriteVInt(SpellIndex);
        }

        public override void Process()
        {
            var battle = SessionContext.Session.Battle;
            if (battle == null) return;

            var data = Data.ReadBytes(Data.ReadableBytes).Array;

            var buffer = Unpooled.Buffer(12);
            {
                buffer.WriteBytes(data);

                buffer.WriteVInt(TroopLevel);

                buffer.WriteVInt(X);
                buffer.WriteVInt(Y);

                battle.GetOwnQueue(SessionContext.EndPoint).Enqueue(buffer.Array);
            }

            var enemyBuffer = Unpooled.Buffer(14);
            {
                enemyBuffer.WriteBytes(data);

                enemyBuffer.WriteVInt(1); // IsAttack
                {
                    enemyBuffer.WriteVInt(GameUtils.Id(ClassId, InstanceId));
                }

                enemyBuffer.WriteVInt(TroopLevel);

                enemyBuffer.WriteVInt(X);
                enemyBuffer.WriteVInt(Y);

                battle.GetEnemyQueue(SessionContext.EndPoint).Enqueue(enemyBuffer.Array);
            }

            battle.Replay.AddCommand(ClientTick, ClientTick, SenderHighId, SenderLowId, ClassId * 1000000 + InstanceId, X, Y);
        }
    }
}