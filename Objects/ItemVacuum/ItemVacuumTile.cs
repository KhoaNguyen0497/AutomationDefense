using AutomationDefense.GUI;
using AutomationDefense.GUI.UIStates;
using AutomationDefense.Helpers;
using AutomationDefense.Objects.AutoCrafter;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationDefense.Objects.ItemVacuum
{
    public class ItemVacuumTile : BaseMultilTile<ItemVacuumTileEntity>
    {
        public override int ItemType => ModContent.ItemType<ItemVacuum>();
        public override Point16 Origin { get; } = new Point16(2,2);
        public override void SetStaticDefaults()
        {
            SetStaticDefaults(5, 4);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = ++frame % 13;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<ItemVacuumTileEntity>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            if (TileHelper.TryGetTileEntity<ItemVacuumTileEntity>(i, j, out var entity))
            {
                entity.UpdateRadius();
            }

            return base.RightClick(i, j);
        }

    }
}
