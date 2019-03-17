using ClashRoyale.Database;
using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class LoginMessage : PiranhaMessage
    {
        public LoginMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10101;
            device.CurrentState = Device.State.Login;
        }

        public long UserId { get; set; }
        public string UserToken { get; set; }
        public int ClientMajorVersion { get; set; }
        public int ClientBuild { get; set; }
        public int ClientMinorVersion { get; set; }
        public string FingerprintSha { get; set; }
        public string OpenUdid { get; set; }
        public string MacAddress { get; set; }
        public string DeviceModel { get; set; }
        public string AdvertisingGuid { get; set; }
        public string OsVersion { get; set; }
        public byte IsAndroid { get; set; }
        public string AndroidId { get; set; }
        public string PreferredDeviceLanguage { get; set; }

        public override void Decode()
        {
            UserId = Buffer.ReadLong();
            UserToken = Buffer.ReadScString();

            ClientMajorVersion = Buffer.ReadVInt();
            ClientMinorVersion = Buffer.ReadVInt();
            ClientBuild = Buffer.ReadVInt();

            FingerprintSha = Buffer.ReadScString();

            Buffer.ReadInt();

            OpenUdid = Buffer.ReadScString();
            MacAddress = Buffer.ReadScString();
            DeviceModel = Buffer.ReadScString();

            AdvertisingGuid = Buffer.ReadScString();
            OsVersion = Buffer.ReadScString();

            IsAndroid = Buffer.ReadByte();

            Buffer.ReadScString();

            AndroidId = Buffer.ReadScString();
            PreferredDeviceLanguage = Buffer.ReadScString().Remove(0, 3);
        }

        public override async void Process()
        {
            if (UserId <= 0 && string.IsNullOrEmpty(UserToken))
            {
                Device.Player = await PlayerDb.Create();

                var home = Device.Player.Home;

                home.PreferredDeviceLanguage = PreferredDeviceLanguage;

                Device.Player.Device = Device;

                await new LoginOkMessage(Device).Send();

                Resources.Players.Login(Device.Player);

                await new OwnHomeDataMessage(Device).Send();
            }
            else
            {
                var player = await Resources.Players.GetPlayer(UserId);

                if (player != null)
                {
                    Device.Player = player;
                    player.Device = Device;

                    Resources.Players.Login(Device.Player);

                    await new LoginOkMessage(Device).Send();

                    await new OwnHomeDataMessage(Device).Send();
                }
                else
                {
                    // If the account is not found we send LoginFailed
                    await new LoginFailedMessage(Device)
                    {
                        ErrorCode = 10
                    }.Send();
                }
            }
        }
    }
}