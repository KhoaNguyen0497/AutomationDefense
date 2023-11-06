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
using AutomationDefense.GUI.UIStates;
using AutomationDefense.GUI;
using AutomationDefense.Helpers;
using AutomationDefense.Objects.AutoCrafter;

namespace AutomationDefense.Objects
{
    public abstract class BaseMultilTile<T> : ModTile where T : BaseMultiTileEntity
    {
        public abstract int ItemType { get; }
        public virtual Point16 Origin { get;} = Point16.Zero;
        public void SetStaticDefaults(int width, int height)
        {

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            
            TileObjectData.newTile.CoordinateHeights = new int[height];
            for (int i = 0; i < height; i++)
            {
                TileObjectData.newTile.CoordinateHeights[i] = 16;
            }

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = Origin;
            AnimationFrameHeight = 18 * height;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, width, 0);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<T>().Hook_AfterPlacement, -1, 0, false);

            SetCustomAttributes();

            TileObjectData.addTile(Type);
        }

        public virtual void SetCustomAttributes()
        {

        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType;
            player.noThrow = 2;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ItemType);
        }
    }
}
