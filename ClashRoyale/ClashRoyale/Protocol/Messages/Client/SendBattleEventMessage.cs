using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class SendBattleEventMessage : PiranhaMessage
    {
        public SendBattleEventMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 12951;
        }

        public int EventId { get; set; }
        public int Tick { get; set; }
        public int Value { get; set; }

        public override void Decode()
        {
            EventId = Buffer.ReadVInt();
            Buffer.ReadVInt();
            Buffer.ReadVInt();
            Buffer.ReadVInt();
            Tick = Buffer.ReadVInt();
            Buffer.ReadVInt();
            Buffer.ReadVInt();
            Value = Buffer.ReadVInt();
        }

        public override async void Process()
        {
            switch (EventId)
            {
                case 3:
                {
                    var battle = Device.Player.Battle;

                    var enemy = battle?.GetEnemy(Device.Player.Home.PlayerId);

                    if(enemy != null)
                        await new BattleEventData(enemy)
                        {
                            EventId = EventId,
                            Tick = Tick,
                            Value = Value,
                            HighId = Device.Player.Home.HighId,
                            LowId = Device.Player.Home.LowId
                        }.Send();

                    break;
                }
            }
        }
    }
}