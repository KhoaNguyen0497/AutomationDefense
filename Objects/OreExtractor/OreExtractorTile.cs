using AutomationDefense.GUI;
using AutomationDefense.GUI.UIStates;
using AutomationDefense.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationDefense.Objects.OreExtractor
{
    public class OreExtractorTile : BaseMultilTile<OreExtractorTileEntity>
    {
        public override int ItemType => ModContent.ItemType<OreExtractor>();
        public override Point16 Origin { get; } = new Point16(3, 4);

        public override void SetStaticDefaults()
        {
            SetStaticDefaults(7, 5);
        }
        public override void SetCustomAttributes()
        {
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
        }


        public override bool RightClick(int i, int j)
        {
            if (TileHelper.TryGetTileEntity<OreExtractorTileEntity>(i, j, out var OreExtractor))
            {
                ModContent.GetInstance<UISystem>().ToggleUI<OreExtractorUIState>(OreExtractor);
            }

            return base.RightClick(i, j);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = ++frame % 24;
            }
        }


        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<OreExtractorTileEntity>().Kill(i, j);
        }
    }
}
