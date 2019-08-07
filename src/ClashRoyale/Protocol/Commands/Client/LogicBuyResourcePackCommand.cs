using ClashRoyale.Logic;
using ClashRoyale.Protocol.Messages.Server;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Commands.Client
{
    public class LogicBuyResourcePackCommand : LogicCommand
    {
        public LogicBuyResourcePackCommand(Device device, IByteBuffer buffer) : base(device, buffer)
        {
        }

        public override void Decode()
        {
            /*Console.WriteLine(Reader.ReadVInt());
            Console.WriteLine(Reader.ReadVInt());
            Console.WriteLine(Reader.ReadVInt());*/
        }

        public override async void Process()
        {
            await new ServerErrorMessage(Device)
            {
                Message = "Not implemented yet."
            }.SendAsync();
        }
    }
}