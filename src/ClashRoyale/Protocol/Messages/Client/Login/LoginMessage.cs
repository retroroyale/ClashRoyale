using System;
using System.Globalization;
using ClashRoyale.Logic;
using ClashRoyale.Logic.Sessions;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Login
{
    public class LoginMessage : PiranhaMessage
    {
        public LoginMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10101;
            device.CurrentState = Device.State.Login;
            RequiredState = Device.State.Login;
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
            PreferredDeviceLanguage = Reader.ReadScString().Substring(3, 2);
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
                    }.SendAsync();
                    return;
                }

            var player = await Resources.Players.Login(UserId, UserToken);

            if (player != null)
            {
                Device.Player = player;
                player.Device = Device;

                var ip = Device.GetIp();

                if (UserId <= 0) player.Home.CreatedIpAddress = ip;

                Device.Player.Home.PreferredDeviceLanguage = PreferredDeviceLanguage;

                var session = Device.Session;
                session.Ip = ip;
                session.GameVersion = $"{ClientMajorVersion}.{ClientMinorVersion}";
                session.Location = await Location.GetByIpAsync(ip);
                session.DeviceCode = DeviceModel;
                session.SessionId = Guid.NewGuid().ToString();
                session.StartDate = session.SessionStart.ToString(CultureInfo.InvariantCulture);

                player.Home.TotalSessions++;

                await new LoginOkMessage(Device).SendAsync();
                await new OwnHomeDataMessage(Device).SendAsync();
                await new AvatarStreamMessage(Device)
                {
                    Entries = player.Home.Stream
                }.SendAsync();

                if (!player.Home.AllianceInfo.HasAlliance) return;

                var alliance = await Resources.Alliances.GetAllianceAsync(player.Home.AllianceInfo.Id);
                if (alliance == null) return;

                Resources.Alliances.Add(alliance);

                await new AllianceStreamMessage(Device)
                {
                    Entries = alliance.Stream
                }.SendAsync();

                alliance.UpdateOnlineCount();
            }
            else
            {
                await new LoginFailedMessage(Device)
                {
                    Reason = "Account not found. Please clear app data."
                }.SendAsync();
            }
        }
    }
}