using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Tournament
{
    public class AskForTournamentListMessage : PiranhaMessage
    {
        public AskForTournamentListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 16103;
        }

        public override async void Process()
        {
            await new TournamentListMessage(Device).SendAsync();
        }
    }
}