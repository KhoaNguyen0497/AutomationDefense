using AutomationDefense.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationDefense.Objects.Transportation.ItemTransporter
{
    public class ItemTransporterTile : BaseTransportationTile<ItemTransporterTileEntity>
    {
        public override int ItemType => ModContent.ItemType<ItemTransporter>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public int test;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType;
            player.noThrow = 2;
        }

        public override bool RightClick(int i, int j)
        {
            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(i, j, out var tileEntity))
            {
                switch (tileEntity.Direction)
                {
                    case Direction.Up:
                        tileEntity.Direction = Direction.Right;
                        break;
                    case Direction.Right:
                        tileEntity.Direction = Direction.Down;
                        break;
                    case Direction.Down:
                        tileEntity.Direction = Direction.Left;
                        break;
                    case Direction.Left:
                        tileEntity.Direction = Direction.Up;
                        break;
                }

                tileEntity.UpdateState = true;
                tileEntity.UpdateNearbyTilesState(true, true, true);
            }

            return base.RightClick(i, j);
        }

    }
}
