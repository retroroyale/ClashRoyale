using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
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

            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Is2V2 = Buffer.ReadBoolean();
        }

        public override async void Process()
        {
            if (Is2V2)
            {
                await new MatchmakeFailedMessage(Device).SendAsync();
                await new CancelMatchmakeDoneMessage(Device).SendAsync();
            }
            else
            {
                var enemy = Resources.Battles.Dequeue;
                if (enemy != null)
                {
                    var battle = new Battle {Device.Player, enemy};

                    Resources.Battles.Add(battle);

                    Device.Player.Battle = battle;
                    enemy.Battle = battle;

                    battle.Start();
                }
                else
                {
                    Resources.Battles.Enqueue(Device.Player);
                }
            }
        }
    }
}