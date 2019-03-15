using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class ClientHelloMessage : PiranhaMessage
    {
        public ClientHelloMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10100;
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
            Protocol = Buffer.ReadInt();
            KeyVersion = Buffer.ReadInt();
            MajorVersion = Buffer.ReadInt();
            MinorVersion = Buffer.ReadInt();
            Build = Buffer.ReadInt();
            FingerprintSha = Buffer.ReadScString();
            DeviceType = Buffer.ReadInt();
            AppStore = Buffer.ReadInt();
        }

        public override async void Process()
        {
            await new LoginFailedMessage(Device)
            {
                ErrorCode = 8
            }.Send();
        }
    }
}