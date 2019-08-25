using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Home
{
    public class HomeBattleReplayMessage : PiranhaMessage
    {
        public HomeBattleReplayMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14114;
        }

        //public long ReplayId { get; set; }

        public override void Decode()
        {
            //ReplayId = Reader.ReadLong();
        }

        public override async void Process()
        {
            await new HomeBattleReplayDataMessage(Device).SendAsync();
        }
    }
}