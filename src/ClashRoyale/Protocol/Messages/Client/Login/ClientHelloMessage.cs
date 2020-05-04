using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client.Login
{
    public class ClientHelloMessage : PiranhaMessage
    {
        public ClientHelloMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10100;
            RequiredState = Device.State.Disconnected;
        }

        public int Protocol { get; set; }
        public int KeyVersion { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public int Build { get; set; }
        public string FingerprintSha { get; set; }
        public int DeviceType { get; set; }
        public int AppStore { get; set; }

        public override void Decrypt()
        {
            // already decrypted
        }

        public override void Decode()
        {
            Protocol = Reader.ReadInt();
            KeyVersion = Reader.ReadInt();
            MajorVersion = Reader.ReadInt();
            MinorVersion = Reader.ReadInt();
            Build = Reader.ReadInt();
            FingerprintSha = Reader.ReadScString();
            DeviceType = Reader.ReadInt();
            AppStore = Reader.ReadInt();
        }

        public override async void Process()
        {
            await new LoginFailedMessage(Device)
            {
                ErrorCode = 8,
                SkipCrypto = true
            }.SendAsync();
        }
    }
}