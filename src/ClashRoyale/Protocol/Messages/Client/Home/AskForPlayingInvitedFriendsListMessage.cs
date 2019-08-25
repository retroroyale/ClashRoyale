using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Home
{
    public class AskForPlayingInvitedFriendsListMessage : PiranhaMessage
    {
        public AskForPlayingInvitedFriendsListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10504;
        }
    }
}