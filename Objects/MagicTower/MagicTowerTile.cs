using AutomationDefense.GUI;
using AutomationDefense.GUI.UIStates;
using AutomationDefense.Helpers;
using AutomationDefense.Objects.AutoCrafter;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationDefense.Objects.MagicTower
{
    public class MagicTowerTile : BaseMultilTile<MagicTowerTileEntity>
    {
        public override int ItemType => ModContent.ItemType<MagicTower>();
        public override Point16 Origin { get; } = new Point16(1,6);
        public override void SetStaticDefaults()
        {
            SetStaticDefaults(3, 7);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = ++frame % 30;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<MagicTowerTileEntity>().Kill(i, j);
        }



    }
}
