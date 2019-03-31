using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AskForAllianceDataMessage : PiranhaMessage
    {
        public AskForAllianceDataMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14302;
        }

        public long AllianceId { get; set; }

        public override void Decode()
        {
            AllianceId = Buffer.ReadLong();
        }

        public override async void Process()
        {
            var clan = await Resources.Alliances.GetAlliance(AllianceId);

            if (clan != null)
                await new AllianceDataMessage(Device)
                {
                    Alliance = clan
                }.Send();
        }
    }
}