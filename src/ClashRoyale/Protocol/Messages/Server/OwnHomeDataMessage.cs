using System;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class OwnHomeDataMessage : PiranhaMessage
    {
        public OwnHomeDataMessage(Device device) : base(device)
        {
            Id = 24101;
            device.CurrentState = Device.State.Home;
            Device.LastVisitHome = DateTime.UtcNow;
        }

        public override void Encode()
        {
            Device.Player.LogicClientHome(Writer);
            Device.Player.LogicClientAvatar(Writer);

            Device.Player.Home.Reset();
        }
    }
}