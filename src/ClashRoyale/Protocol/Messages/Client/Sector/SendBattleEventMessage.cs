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

        public int Value1 { get; set; }
        public int Value2 { get; set; }

        public int SenderHighId { get; set; }
        public int SenderLowId { get; set; }

        public int Unknown3 { get; set; }

        public override void Decode()
        {
            Type = Reader.ReadVInt();
            SenderHighId = Reader.ReadVInt();
            SenderLowId = Reader.ReadVInt();

            Reader.ReadVInt();
            Tick = Reader.ReadVInt();
            Unknown3 = Reader.ReadVInt();

            Value1 = Reader.ReadVInt();
            Value2 = Reader.ReadVInt();
        }

        public override async void Process()
        {
            var battle = Device.Player.Battle;

            if (battle != null)
            {
                if (battle.Is2v2)
                {
                    var home = Device.Player.Home;

                    switch (Type)
                    {
                        case 6:
                            {
                                var unknown = Reader.ReadVInt();
                                var handIndex = Reader.ReadVInt();
                                var unknown2 = Reader.ReadVInt();

                                var teammate = battle.GetTeammate(home.Id);

                                if (teammate != null)
                                    await new BattleEventMessage(teammate.Device)
                                    {
                                        Type = Type,
                                        Tick = Tick,
                                        Value1 = Value1,
                                        Value2 = Value2,
                                        HighId = SenderHighId,
                                        LowId = SenderLowId,
                                        Unknown = unknown,
                                        Unknown2 = unknown2,
                                        Unknown3 = Unknown3,
                                        HandIndex = handIndex
                                    }.SendAsync();

                                break;
                            }

                        case 3:
                            {
                                var players = battle.GetAllOthers(home.Id);

                                foreach (var player in players)
                                {
                                    if (player?.Device != null)
                                        await new BattleEventMessage(player.Device)
                                        {
                                            Type = Type,
                                            Tick = Tick,
                                            Value1 = Value1,
                                            Value2 = Value2,
                                            HighId = SenderHighId,
                                            LowId = SenderLowId
                                        }.SendAsync();
                                }

                                break;
                            }

                        case 1:
                            {
                                var teammate = battle.GetTeammate(home.Id);

                                if (teammate != null)
                                    await new BattleEventMessage(teammate.Device)
                                    {
                                        Type = Type,
                                        Tick = Tick,
                                        Value1 = Value1,
                                        Value2 = Value2,
                                        HighId = SenderHighId,
                                        LowId = SenderLowId
                                    }.SendAsync();

                                break;
                            }
                    }
                }
                else
                {
                    var home = Device.Player.Home;

                    switch (Type)
                    {
                        case 3:
                        {
                            var enemy = battle.GetEnemy(home.Id);

                            if (enemy != null)
                                await new BattleEventMessage(enemy)
                                {
                                    Type = Type,
                                    Tick = Tick,
                                    Value1 = Value1,
                                    Value2 = Value2,
                                    HighId = SenderHighId,
                                    LowId = SenderLowId
                                }.SendAsync();

                            break;
                        }
                    }
                }

                //battle.Replay.AddEvent(Type, home.HighId, home.LowId, Tick, Value);
            }
        }
    }
}