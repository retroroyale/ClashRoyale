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
            UserId = Reader.ReadLong();
            UserToken = Reader.ReadScString();

            ClientMajorVersion = Reader.ReadVInt();
            ClientMinorVersion = Reader.ReadVInt();
            ClientBuild = Reader.ReadVInt();

            FingerprintSha = Reader.ReadScString();

            Reader.ReadInt();

            OpenUdid = Reader.ReadScString();
            MacAddress = Reader.ReadScString();
            DeviceModel = Reader.ReadScString();

            AdvertisingGuid = Reader.ReadScString();
            OsVersion = Reader.ReadScString();

            IsAndroid = Reader.ReadByte();

            Reader.ReadScString();

            AndroidId = Reader.ReadScString();
            PreferredDeviceLanguage = Reader.ReadScString().Remove(0, 3);
        }

        public override async void Process()
        {
            if (Resources.Configuration.UseContentPatch)
                if (FingerprintSha != Resources.Fingerprint.Sha)
                {
                    await new LoginFailedMessage(Device)
                    {
                        ErrorCode = 7,
                        ContentUrl = Resources.Configuration.PatchUrl,
                        ResourceFingerprintData = Resources.Fingerprint.Json
                    }.Send();
                    return;
                }

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

                    if (player.Home.AllianceInfo.HasAlliance)
                    {
                        var alliance = await Resources.Alliances.GetAlliance(player.Home.AllianceInfo.Id);

                        if (alliance != null)
                        {
                            Resources.Alliances.Add(alliance);

                            await new AllianceStreamMessage(Device)
                            {
                                Entries = alliance.Stream
                            }.Send();

                            alliance.UpdateOnlineCount();
                        }
                    }

                    await new OwnHomeDataMessage(Device).Send();
                }
                else
                {
                    // If the account was not found we send LoginFailed
                    await new LoginFailedMessage(Device)
                    {
                        ErrorCode = 10
                    }.Send();
                }
            }
        }
    }
}