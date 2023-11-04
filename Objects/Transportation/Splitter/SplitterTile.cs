
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using AutomationDefense.Helpers;
using AutomationDefense.GUI;
using AutomationDefense.GUI.UIStates;

namespace AutomationDefense.Objects.Transportation.Splitter
{
    public class SplitterTile : BaseTransportationTile<SplitterTileEntity>
    {
        public override int ItemType => ModContent.ItemType<Splitter>();

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<SplitterTileEntity>().Kill(i, j);
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<SplitterTileEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType;
            player.noThrow = 2;
        }

        public override bool RightClick(int i, int j)
        {
            if (TileHelper.TryGetTileEntity<SplitterTileEntity>(i, j, out var splitter))
            {
                ModContent.GetInstance<UISystem>().ToggleUI<SplitterFilterUIState>(splitter);
            }

            return base.RightClick(i, j);
        }
    }
}
