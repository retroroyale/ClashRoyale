using System.Collections.Generic;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
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
                Alliances = new List<Logic.Clan.Alliance>(0)
            }.SendAsync();
        }
    }
}