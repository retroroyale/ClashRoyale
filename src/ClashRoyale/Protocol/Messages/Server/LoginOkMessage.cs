using ClashRoyale.Extensions;
using ClashRoyale.Logic;

namespace ClashRoyale.Protocol.Messages.Server
{
    public class LoginOkMessage : PiranhaMessage
    {
        public LoginOkMessage(Device device) : base(device)
        {
            Id = 20104;
            Version = 1;
        }

        public override void Encode()
        {
            Writer.WriteLong(Device.Player.Home.Id);
            Writer.WriteLong(Device.Player.Home.Id);
            Writer.WriteScString(Device.Player.Home.UserToken);

            Writer.WriteScString(string.Empty); // GamecenterId
            Writer.WriteScString(string.Empty); // FacebookId

            Writer.WriteVInt(Resources.Fingerprint.GetMajorVersion);
            Writer.WriteVInt(Resources.Fingerprint.GetBuildVersion);
            Writer.WriteVInt(Resources.Fingerprint.GetContentVersion);

            Writer.WriteScString("prod");
            Writer.WriteVInt(0); // SessionCount
            Writer.WriteVInt(0); // PlayTime
            Writer.WriteVInt(0); // DaysSinceStartedPlaying

            Writer.WriteScString(string.Empty); // FacebookAppId
            Writer.WriteScString(string.Empty); // ServerTime
            Writer.WriteScString(string.Empty); // AccountCreateDate

            Writer.WriteVInt(0);
            Writer.WriteScString("G:1"); // GoogleServiceId
            Writer.WriteScString(string.Empty);

            Writer.WriteScString("DE");
            Writer.WriteScString("Berlin");

            Writer.WriteScString("https://game-assets.clashroyaleapp.com");
            Writer.WriteScString(Resources.Configuration.PatchUrl);
            Writer.WriteScString("https://event-assets.clashroyale.com");
        }
    }
}