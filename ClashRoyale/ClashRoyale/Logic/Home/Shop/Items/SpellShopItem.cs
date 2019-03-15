using ClashRoyale.Extensions;
using DotNetty.Buffers;

namespace ClashRoyale.Logic.Home.Shop.Items
{
    public class SpellShopItem : ShopItem
    {
        public SpellShopItem()
        {
            Type = 1;
        }

        public int ClassId { get; set; }
        public int InstanceId { get; set; }
        public int Rarity { get; set; }
        public int Bought { get; set; }

        public override void Encode(IByteBuffer packet)
        {
            base.Encode(packet);

            packet.WriteVInt(Bought);

            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);
            packet.WriteVInt(0);

            packet.WriteVInt(ClassId);
            packet.WriteVInt(InstanceId);
            packet.WriteVInt(Rarity);
        }
    }
}