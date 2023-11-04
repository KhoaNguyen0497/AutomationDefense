using AutomationDefense.Helpers;
using AutomationDefense.Objects;
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

namespace AutomationDefense.Objects.StoneGenerator
{
    public class StoneGeneratorTileEntity : BaseMultiTileEntity
    {
        public override Point16 Origin { get; } = new Point16(2, 4);

        public int StoneGenerated = 5;
        public override bool IsTileValidForEntity(int x, int y)
        {

            Tile tile = Main.tile[x, y];

            return tile.HasTile && tile.TileType == ModContent.TileType<StoneGeneratorTile>();
        }

        public override void Update()
        {

            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
                var chestX = Alternate == 1 ? Position.X + 5 : Position.X - 2;
                var chestIndex = Chest.FindChest(chestX, Position.Y + 3);
                if (chestIndex > -1)
                {
                    Chest chest = Main.chest[chestIndex];
                    var stoneblocks = new Item(ItemID.StoneBlock, StoneGenerated);
                    chest.DepositIntoChest(stoneblocks);
                }
            }
        }

        public override void SetTilePropeties(int x, int y, int type, int style, int direction, int alternate)
        {
            if (TileHelper.TryGetTileEntity<StoneGeneratorTileEntity>(x, y, out var tileEntity))
            {
                tileEntity.Alternate = alternate;
            }
        }
    }
}
