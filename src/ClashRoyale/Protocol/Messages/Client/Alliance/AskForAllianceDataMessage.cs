using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
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
            AllianceId = Reader.ReadLong();
        }

        public override async void Process()
        {
            var alliance = await Resources.Alliances.GetAllianceAsync(AllianceId);

            if (alliance != null)
                await new AllianceDataMessage(Device)
                {
                    Alliance = alliance
                }.SendAsync();
        }
    }
}