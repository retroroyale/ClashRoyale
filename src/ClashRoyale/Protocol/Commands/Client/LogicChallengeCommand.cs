using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicChallengeCommand : LogicCommand
    {
        public LogicChallengeCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public string Message { get; set; }
        public int Arena { get; set; }

        public override void Decode()
        {
            Message = Buffer.ReadScString();
            Buffer.ReadBoolean();

            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt();
            Buffer.ReadVInt();

            Buffer.ReadVInt();

            Arena = Buffer.ReadVInt();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

            if (alliance != null)
            {
                var entry = new ChallengeStreamEntry
                {
                    Message = Message,
                    Arena = Arena + 1
                };

                entry.SetSender(Device.Player);
                alliance.AddEntry(entry);
            }
        }
    }
}