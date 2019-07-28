using System.Collections.Generic;
using ClashRoyale.Battles.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Battles.Protocol
{
    public class UdpPacket
    {
        private readonly List<UdpMessage> _udpMessages = new List<UdpMessage>();
        public SessionContext SessionContext { get; set; }
        public IByteBuffer Reader { get; set; }

        public UdpPacket(SessionContext ctx, IByteBuffer content)
        {
            SessionContext = ctx;
            Reader = content;
        }

        public void Encode()
        {
            // TODO
        }

        public void Decode()
        {
            // TODO
        }

        public List<UdpMessage> GetMessages()
        {
            return _udpMessages;
        }

        public void AddMessage(UdpMessage message)
        {
            if (!_udpMessages.Contains(message))
            {
                _udpMessages.Add(message);
            }
        }
    }
}
