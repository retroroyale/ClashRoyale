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

        public override void Decode()
        {
            Type = Reader.ReadVInt();
            Reader.ReadVInt();
            Reader.ReadVInt();
            Reader.ReadVInt();
            Tick = Reader.ReadVInt();
            Reader.ReadVInt();

            Value1 = Reader.ReadVInt();
            Value2 = Reader.ReadVInt();
        }

        public override async void Process()
        {
            var battle = Device.Player.Battle;
            var duoBattle = Device.Player.DuoBattle;

            if (battle != null)
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
                                Value2 = Value2,
                                HighId = Device.Player.Home.HighId,
                                LowId = Device.Player.Home.LowId
                            }.SendAsync();

                        break;
                    }
                }

                //battle.Replay.AddEvent(Type, home.HighId, home.LowId, Tick, Value);
            }
            else if (duoBattle != null)
            {
                var home = Device.Player.Home;

                switch (Type)
                {
                    case 6:
                    {
                        var unknown = Reader.ReadVInt();
                        var handIndex = Reader.ReadVInt();
                        var unknown2 = Reader.ReadVInt();

                        var teammate = duoBattle.GetTeammate(home.Id);

                        await new BattleEventMessage(teammate.Device)
                        {
                            Type = Type,
                            Tick = Tick,
                            Value1 = Value1,
                            Value2 = Value2,
                            HighId = Device.Player.Home.HighId,
                            LowId = Device.Player.Home.LowId,
                            Unknown = unknown,
                            Unknown2 = unknown2,
                            HandIndex = handIndex
                        }.SendAsync();

                            break;
                    }

                    case 3:
                    {
                        var players = duoBattle.GetAllOthers(home.Id);

                        foreach (var player in players)
                        {
                            if (player?.Device != null)
                                await new BattleEventMessage(player.Device)
                                {
                                    Type = Type,
                                    Tick = Tick,
                                    Value2 = Value2,
                                    HighId = Device.Player.Home.HighId,
                                    LowId = Device.Player.Home.LowId
                                }.SendAsync();
                        }

                        break;
                    }

                    case 1:
                    {
                        var teammate = duoBattle.GetTeammate(home.Id);

                        await new BattleEventMessage(teammate.Device)
                        {
                            Type = Type,
                            Tick = Tick,
                            Value2 = Value2,
                            HighId = Device.Player.Home.HighId,
                            LowId = Device.Player.Home.LowId
                        }.SendAsync();

                        break;
                    }
                }
            }
        }
    }
}