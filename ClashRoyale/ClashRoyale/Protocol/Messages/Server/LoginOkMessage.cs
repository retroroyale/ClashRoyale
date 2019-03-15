using System;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class LoginOkMessage : PiranhaMessage
    {
        public LoginOkMessage(Device device) : base(device)
        {
            Id = 20104;
            Device.LastVisitHome = DateTime.UtcNow;
        }

        public override void Encode()
        {
            Packet.WriteLong(Device.Player.Home.PlayerId);
            Packet.WriteLong(Device.Player.Home.PlayerId);
            Packet.WriteScString(Device.Player.Home.UserToken);

            Packet.WriteScString(string.Empty);
            Packet.WriteScString("G:1");

            Packet.WriteInt(Resources.Fingerprint.GetMajorVersion);
            Packet.WriteInt(Resources.Fingerprint.GetBuildVersion);
            Packet.WriteInt(Resources.Fingerprint.GetContentVersion);

            Packet.WriteScString("prod");
            Packet.WriteScString(string.Empty);

            Packet.WriteScString("DE");
            Packet.WriteScString("Berlin");

            Packet.WriteScString(string.Empty);

            Packet.WriteScString("https://event-assets.clashroyale.com");
            Packet.WriteVInt(3);
        }
    }
}