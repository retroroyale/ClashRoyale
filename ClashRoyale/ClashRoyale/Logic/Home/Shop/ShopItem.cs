using ClashRoyale.Extensions;
using DotNetty.Buffers;

namespace ClashRoyale.Logic.Home.Shop
{
    public class ShopItem
    {
        public int Type { get; set; }

        public virtual void Encode(IByteBuffer packet)
        {
            packet.WriteVInt(Type);
            packet.WriteVInt(66);
            packet.WriteVInt(2);
        }
    }
}