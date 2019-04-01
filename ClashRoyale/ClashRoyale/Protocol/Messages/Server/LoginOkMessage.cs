using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class LoginOkMessage : PiranhaMessage
    {
        public LoginOkMessage(Device device) : base(device)
        {
            Id = 20104;
        }

        public override void Encode()
        {
            Writer.WriteLong(Device.Player.Home.Id);
            Writer.WriteLong(Device.Player.Home.Id);
            Writer.WriteScString(Device.Player.Home.UserToken);

            Writer.WriteScString(string.Empty);
            Writer.WriteScString("G:1");

            Writer.WriteInt(Resources.Fingerprint.GetMajorVersion);
            Writer.WriteInt(Resources.Fingerprint.GetBuildVersion);
            Writer.WriteInt(Resources.Fingerprint.GetContentVersion);

            Writer.WriteScString("prod");
            Writer.WriteScString(string.Empty);

            Writer.WriteScString("DE");
            Writer.WriteScString("Berlin");

            Writer.WriteScString(string.Empty);

            Writer.WriteScString("https://event-assets.clashroyale.com");
            Writer.WriteVInt(3);
        }
    }
}