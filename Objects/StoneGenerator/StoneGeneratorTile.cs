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

namespace AutomationDefense.Objects.StoneGenerator
{
    public class StoneGeneratorTile : BaseMultilTile<StoneGeneratorTileEntity>
    {
        public override int ItemType => ModContent.ItemType<StoneGenerator>();
        public override Point16 Origin { get; } = new Point16(2, 4);

        public override void SetStaticDefaults()
        {

            SetStaticDefaults(5, 5);
        }

        public override bool RightClick(int i, int j)
        {
            return base.RightClick(i, j);
        }

        public override void SetAlternate()
        {
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = ++frame % 19;
            }
        }


        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<StoneGeneratorTileEntity>().Kill(i, j);
        }

    }
}
