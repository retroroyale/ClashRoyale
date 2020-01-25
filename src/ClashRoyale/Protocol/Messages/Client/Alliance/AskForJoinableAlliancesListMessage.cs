using System.Collections.Generic;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Alliance
{
    public class AskForJoinableAlliancesListMessage : PiranhaMessage
    {
        public AskForJoinableAlliancesListMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14303;
        }

        public override async void Process()
        {
            await new JoinableAllianceListMessage(Device)
            {
                Alliances = new List<Logic.Clan.Alliance>(0)
            }.SendAsync();
        }
    }
}