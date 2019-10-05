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

        public int Type { get; set; }
        public int Tick { get; set; }
        public int Value { get; set; }

        public override void Decode()
        {
            Type = Reader.ReadVInt();
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
            var battle = Device.Player.Battle;

            if (battle == null)
                return;

            var home = Device.Player.Home;

            switch (Type)
            {
                case 3:
                {
                    var enemy = battle.GetEnemy(home.Id);

                    if (enemy != null)
                        await new BattleEventMessage(enemy)
                        {
                            EventId = Type,
                            Tick = Tick,
                            Value = Value,
                            HighId = Device.Player.Home.HighId,
                            LowId = Device.Player.Home.LowId
                        }.SendAsync();

                    break;
                }

                case 1:
                {
                    // CARD SELECTED // FOR REPLAY/SPECTATORS/DUO
                    break;
                }
            }

            battle.Replay.AddEvent(Type, home.HighId, home.LowId, Tick, Value);
        }
    }
}