using AutomationDefense.Helpers;
using AutomationDefense.Objects;
using AutomationDefense.Objects.WoodHarvester;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationDefense.Objects.WoodHarvesterBase
{
    public class WoodHarvesterBaseTileEntity : BaseMultiTileEntity
    {
        public override Point16 Origin { get; } = new Point16(0, 1);

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<WoodHarvesterBaseTile>();
        }

        public Item WoodStored { get; set; }

        public override void SetTilePropeties(int x, int y, int type, int style, int direction, int alternate)
        {
            if (TileHelper.TryGetTileEntity<WoodHarvesterBaseTileEntity>(x,y, out var entity))
            {

            }
            base.SetTilePropeties(x, y, type, style, direction, alternate);
        }

        public void UpdateTileState(bool capOn)
        {
            int sideLength = 36;
            var currentTileFrameX = Main.tile[Position.X, Position.Y].TileFrameX;
            short xOffset = 0;

            if (capOn)
            {
                // If already cap
                if (currentTileFrameX < 36)
                {
                    return;
                }

                xOffset = (short)(-sideLength);
            }
            else
            {
                // If already no cap
                if (currentTileFrameX >= 36)
                {
                    return;
                }

                xOffset = (short)(sideLength);

            }

            Main.tile[Position.X, Position.Y].TileFrameX += xOffset;
            Main.tile[Position.X + 1, Position.Y].TileFrameX += xOffset;
            Main.tile[Position.X, Position.Y + 1].TileFrameX += xOffset;
            Main.tile[Position.X + 1, Position.Y + 1].TileFrameX += xOffset;
            Main.tile[Position.X, Position.Y + 2].TileFrameX += xOffset;
            Main.tile[Position.X + 1, Position.Y + 2].TileFrameX += xOffset;
        }

        public Chest GetChest()
        {   
            var leftChestIndex = Chest.FindChest(Position.X - 2, Position.Y + 1);
            var rightChestIndex = Chest.FindChest(Position.X + 2, Position.Y + 1);

            if (leftChestIndex != -1)
            {
                return Main.chest[leftChestIndex];
            }
            if (rightChestIndex != -1)
            {
                return Main.chest[rightChestIndex];
            }

            return null;
        }

        public override void Update()
        {
            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
               var chest = GetChest();

                if (chest != null)
                {
                    chest.DepositIntoChest(WoodStored);
                }
            }
        }

    }
}
