using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using AutomationDefense.Helpers;

namespace AutomationDefense.Objects.Transportation
{
    public abstract class BaseTransportationTile<T> : ModTile where T : BaseTransportationTileEntity
    {
        public abstract int ItemType { get; }
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<T>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
        }


        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<T>().Kill(i, j);
        }

        // Because we have custom style we have to do this
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ItemType);
        }
    }
}
