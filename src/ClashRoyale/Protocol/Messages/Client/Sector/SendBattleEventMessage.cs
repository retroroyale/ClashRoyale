using System;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Sector
{
    public class SendBattleEventMessage : PiranhaMessage
    {
        public SendBattleEventMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 12951;
            RequiredState = Device.State.Battle;
        }

        public int EventId { get; set; }
        public int Tick { get; set; }
        public int Value { get; set; }

        public override void Decode()
        {
            EventId = Reader.ReadVInt();
            Reader.ReadVInt();
            Reader.ReadVInt();
            Reader.ReadVInt();
            Tick = Reader.ReadVInt();
            Reader.ReadVInt();
            Reader.ReadVInt();
            Value = Reader.ReadVInt();
        }

        public override async void Process()
        {
            switch (EventId)
            {
                case 3:
                {
                    var battle = Device.Player.Battle;

                    var enemy = battle?.GetEnemy(Device.Player.Home.Id);

                    if (enemy != null)
                        await new BattleEventMessage(enemy)
                        {
                            EventId = EventId,
                            Tick = Tick,
                            Value = Value,
                            HighId = Device.Player.Home.HighId,
                            LowId = Device.Player.Home.LowId
                        }.SendAsync();

                    break;
                }

                case 1:
                {
                    // CARD SELECTED // FOR REPLAY OR SPECTATORS
                    break;
                }
            }
        }
    }
}