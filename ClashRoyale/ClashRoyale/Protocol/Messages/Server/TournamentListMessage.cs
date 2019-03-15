using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class TournamentListMessage : PiranhaMessage
    {
        public TournamentListMessage(Device device) : base(device)
        {
            Id = 26101;
        }

        public override void Encode()
        {
            Packet.WriteVInt(0);
        }
    }
}