using System.Collections.Generic;
using ClashRoyale.Battles.Logic.Session;
using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Protocol
{
    public class UdpPacket
    {
        private readonly List<UdpMessage> _udpMessages = new List<UdpMessage>();
        public SessionContext SessionContext { get; set; }
        public IByteBuffer Writer { get; set; }

        public UdpPacket(SessionContext ctx)
        {
            SessionContext = ctx;
        }

        public void Encode()
        {
            Writer.WriteLong(SessionContext.PlayerId);

            var count = _udpMessages.Count;
            Writer.WriteVInt(count);

            for (var i = 0; i < count; i++)
            {
                Writer.WriteVInt(1);
            }
        }

        public List<UdpMessage> GetMessages() => _udpMessages;

        public void AddMessage(UdpMessage message)
        {
            if (!_udpMessages.Contains(message))
            {
                _udpMessages.Add(message);
            }
        }
    }
}
