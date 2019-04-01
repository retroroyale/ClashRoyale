using ClashRoyale.Extensions;
using ClashRoyale.Logic;
using ClashRoyale.Protocol.Commands.Server;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class ChangeAvatarNameMessage : PiranhaMessage
    {
        public ChangeAvatarNameMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 10212;
        }

        public string Name { get; set; }

        public override void Decode()
        {
            Name = Reader.ReadScString();
        }

        public override async void Process()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                if (Name.Length > 2 && Name.Length <= 15)
                {
                    var home = Device.Player.Home;

                    if (home.NameSet < 2)
                    {
                        home.Name = Name;

                        var info = Device.Player.Home.AllianceInfo;

                        if (info.HasAlliance)
                        {
                            var alliance = await Resources.Alliances.GetAlliance(info.Id);

                            alliance.GetMember(home.Id).Name = Name;

                            alliance.Save();
                        }

                        await new AvailableServerCommand(Device)
                        {
                            Command = new LogicChangeNameCommand(Device)
                            {
                                NameSet = home.NameSet 
                            }
                        }.Send();

                        home.NameSet++;

                        Device.Player.Save();
                    }
                }
            }
        }
    }
}