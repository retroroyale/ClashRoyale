using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class SearchAlliancesMessage : PiranhaMessage
    {
        public SearchAlliancesMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14324;
        }

        public override async void Process()
        {
            // TODO:
            await new AllianceListMessage(Device)
            {
                Alliances = await Resources.Alliances.GetRandomAlliances()
            }.Send();
        }
    }
}