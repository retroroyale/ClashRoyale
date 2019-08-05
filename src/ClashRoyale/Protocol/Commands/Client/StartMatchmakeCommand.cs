using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class StartMatchmakeCommand : LogicCommand
    {
        public StartMatchmakeCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public bool Is2V2 { get; set; }

        public override void Decode()
        {
            base.Decode();

            Reader.ReadVInt();
            Reader.ReadVInt();

            Is2V2 = Reader.ReadBoolean();
        }

        public override async void Process()
        {
            if (Is2V2)
            {
                await new MatchmakeFailedMessage(Device).SendAsync();
                await new CancelMatchmakeDoneMessage(Device).SendAsync();

                /*var players = Resources.Battles.DequeueDuo;
                if (players != null)
                {
                    var battle = new LogicBattle(false, Device.Player.Home.Arena.CurrentArena + 1)
                    {
                        Device.Player, players
                    };

                    Resources.Battles.Add(battle);

                    Device.Player.Battle = battle;
                    players.Battle = battle;

                    battle.Start();
                }
                else
                {
                    Resources.Battles.Enqueue(Device.Player, Is2V2);
                }*/
            }
            else
            {
                var enemy = Resources.Battles.Dequeue;
                if (enemy != null)
                {
                    var battle = new LogicBattle(false, enemy.Home.Arena.CurrentArena + 1)
                    {
                        Device.Player, enemy
                    };

                    Resources.Battles.Add(battle);

                    Device.Player.Battle = battle;
                    enemy.Battle = battle;

                    battle.Start();
                }
                else
                {
                    Resources.Battles.Enqueue(Device.Player, Is2V2);
                }
            }
        }
    }
}