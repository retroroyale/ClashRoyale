using ClashRoyale.Logic;
using ClashRoyale.Logic.Battle;
using ClashRoyale.Logic.Clan.StreamEntry.Entries;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class AcceptChallengeMessage : PiranhaMessage
    {
        public AcceptChallengeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14120;
        }

        public long EntryId { get; set; }

        public override void Decode()
        {
            EntryId = Reader.ReadLong();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

            if (!(alliance?.Stream.Find(e => e.Id == EntryId && e.StreamEntryType == 10) is ChallengeStreamEntry entry)) return;

            alliance.RemoveEntry(entry);

            var enemy = await Resources.Players.GetPlayerAsync(entry.SenderId);

            if (enemy.Device != null)
            {
                var battle = new LogicBattle(true, entry.Arena)
                {
                    Device.Player, enemy
                };

                Resources.Battles.Add(battle);

                Device.Player.Battle = battle;
                enemy.Battle = battle;

                battle.Start();
            }

            alliance.Save();

            // TODO: Update Entry + Battle Result + Card levels
        }
    }
}