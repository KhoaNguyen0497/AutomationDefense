using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationDefense.Objects.Transportation
{
    public class BaseTransportationItem<T, T2> : ModItem
        where T : BaseTransportationTile<T2>
        where T2 : BaseTransportationTileEntity
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 5;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<T>();
            Item.value = 5000;
        }
    }
}
