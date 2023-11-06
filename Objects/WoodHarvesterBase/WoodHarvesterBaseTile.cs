using AutomationDefense.GUI;
using AutomationDefense.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationDefense.Objects.WoodHarvesterBase
{
    public class WoodHarvesterBaseTile : BaseMultilTile<WoodHarvesterBaseTileEntity>
    {
        public override int ItemType => ModContent.ItemType<WoodHarvesterBase>();
        public override Point16 Origin { get; } = new Point16(0, 1);

        public override void SetStaticDefaults()
        {
            SetStaticDefaults(2, 3);
        }

        public override bool CanPlace(int i, int j)
        {
            return false;
        }

        public override void SetCustomAttributes()
        {
                   
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = ++frame % 16;
            }
        }

   
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<WoodHarvesterBaseTileEntity>().Kill(i, j);
        }
    }
}
