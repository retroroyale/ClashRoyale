using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class TournamentJoinedFailedMessage : PiranhaMessage
    {
        public TournamentJoinedFailedMessage(Device device) : base(device)
        {
            Id = 26106;
        }
    }
}