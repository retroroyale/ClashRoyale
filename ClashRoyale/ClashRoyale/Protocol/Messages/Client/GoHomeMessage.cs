using System;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class GoHomeMessage : PiranhaMessage
    {
        public GoHomeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14101;
            Device.LastVisitHome = DateTime.UtcNow;
            Save = true;
        }

        public override async void Process()
        {
            await new OwnHomeDataMessage(Device).Send();
        }
    }
}