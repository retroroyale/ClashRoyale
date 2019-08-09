using ClashRoyale.Utilities.Netty;
using DotNetty.Buffers;

namespace ClashRoyale.Logic.Home.Shop
{
    public class ShopItem
    {
        public int Type { get; set; }
        public int ShopIndex { get; set; }

        public virtual void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(Type);
            packet.WriteVInt(1);
            packet.WriteVInt(ShopIndex);
        }
    }
}