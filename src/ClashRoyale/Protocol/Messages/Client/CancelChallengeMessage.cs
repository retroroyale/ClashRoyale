using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class CancelChallengeMessage : PiranhaMessage
    {
        public CancelChallengeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14123;
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);
            if (alliance == null) return;

            var entry = alliance.Stream.Find(e => e.SenderId == home.Id && e.StreamEntryType == 10);

            if (entry != null) alliance.RemoveEntry(entry);

            await new CancelChallengeDoneMessage(Device).SendAsync();
        }
    }
}