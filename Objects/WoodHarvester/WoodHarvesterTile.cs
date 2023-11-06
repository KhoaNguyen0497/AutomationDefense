using AutomationDefense.GUI;
using AutomationDefense.Objects;
using AutomationDefense.Objects.WoodHarvesterBase;
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

namespace AutomationDefense.Objects.WoodHarvester
{
    public class WoodHarvesterTile : BaseMultilTile<WoodHarvesterTileEntity>
    {
        public override int ItemType => ModContent.ItemType<WoodHarvester>();
        public override Point16 Origin { get; } = new Point16(0, 0);

        public override void SetStaticDefaults()
        {


            SetStaticDefaults(2, 2);
        }

        public override bool CanPlace(int i, int j)
        {
            return false;
        }


        public override void SetCustomAttributes()
        {
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, 2, 0);
            TileObjectData.newTile.AnchorAlternateTiles = new int[] { ModContent.TileType<WoodHarvesterTile>(), ModContent.TileType<WoodHarvesterBaseTile>() };
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
                frame = ++frame % 43;
            }
        }

   
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<WoodHarvesterTileEntity>().Kill(i, j);
        }

    }
}
