using AutomationDefense.Helpers;
using AutomationDefense.Objects.Transportation;
using AutomationDefense.Objects.Transportation.ItemFunnel;
using AutomationDefense.Objects.Transportation.Splitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace AutomationDefense.Objects.Transportation.ItemTransporter
{
    public class ItemTransporterTileEntity : BaseTransportationTileEntity
    {

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ItemTransporterTile>();
        }

        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            int placedEntity = Place(x, y);

            // Every tile starts at default, row 0
            SetInitialProperties<ItemTransporterTileEntity>(x, y);
            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(x, y, out var tileEntity))
            {
                var leftTile = Main.tile[x - 1, y];
                var rightTile = Main.tile[x + 1, y];
                var downTile = Main.tile[x, y + 1];
                var upTile = Main.tile[x, y - 1];

                if (leftTile.TileType == ModContent.TileType<ItemTransporterTile>())
                {
                    tileEntity.Direction = Direction.Right;
                }
                else if (rightTile.TileType == ModContent.TileType<ItemTransporterTile>())
                {
                    tileEntity.Direction = Direction.Left;
                }
                else if (downTile.TileType == ModContent.TileType<ItemTransporterTile>())
                {
                    tileEntity.Direction = Direction.Up;
                }
                else if (upTile.TileType == ModContent.TileType<ItemTransporterTile>())
                {
                    tileEntity.Direction = Direction.Down;
                }

                tileEntity.UpdateState = true;
                tileEntity.UpdateNearbyTilesState(true, true, true);
            }
            // Check for combination of valid tiles around the current tile and adjust the style
            return placedEntity;
        }



        // direction can be any side of the tile, it is not the Direction of the tile.
        public bool ValidConnection(Direction direction, out BaseTransportationTileEntity tileEntity)
        {
            bool xor(bool c1, bool c2)
            {
                return c1 != c2;
            }
            int x = 0;
            int y = 0;
            tileEntity = null;
            GetSidePosition(direction, out x, out y);

            bool valid = false;

            // Transporter
            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(x, y, out var transporter))
            {
                switch (direction)
                {
                    case Direction.Up:
                        valid = xor(transporter.Direction == Direction.Down, Direction == Direction.Up);
                        break;
                    case Direction.Down:
                        valid = xor(transporter.Direction == Direction.Up, Direction == Direction.Down);
                        break;
                    case Direction.Right:
                        valid = xor(transporter.Direction == Direction.Left, Direction == Direction.Right);
                        break;
                    case Direction.Left:
                        valid = xor(transporter.Direction == Direction.Right, Direction == Direction.Left);
                        break;
                }

                if (valid)
                {
                    tileEntity = transporter;
                    return true;
                }
            }

            // Funnel
            if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(x, y, out var funnel))
            {
                // If its a funnel, its valid for style state if the funnel's direction are on the same side we are looking at
                valid = direction == funnel.Direction;

                // But its only a target for inserting into if the transporter is facing the funnel
                if (direction == Direction)
                {
                    tileEntity = funnel;
                }
            }

            // Splitter
            if (TileHelper.TryGetTileEntity<SplitterTileEntity>(x, y, out var splitter))
            {
                valid = true;
                tileEntity = splitter;
            }

            return valid;
        }
        // on valid connection (not facing each other and one must be pointing to the other), update state
        // Also allow overriding validity because when nearby tiles are killed, its still valid for a bit, but its technically not
        public void UpdateTileState()
        {
            // transporter
            bool rightvalid = ValidConnection(Direction.Right, out var rightEntity);
            bool leftvalid = ValidConnection(Direction.Left, out var leftEntity);
            bool topValid = ValidConnection(Direction.Up, out var topEntity);
            bool bottomValid = ValidConnection(Direction.Down, out var bottomEntity);

            int styleRow = GetStyleRow(topValid, rightvalid, bottomValid, leftvalid);
            Tile tile = Main.tile[Position.X, Position.Y];
            tile.TileFrameY = (short)(18 * styleRow);

            switch (Direction)
            {
                case Direction.Up:
                    Target = topEntity;
                    tile.TileFrameX = 0;
                    break;
                case Direction.Right:
                    Target = rightEntity;
                    tile.TileFrameX = 18;
                    break;
                case Direction.Down:
                    Target = bottomEntity;
                    tile.TileFrameX = 36;
                    break;
                case Direction.Left:
                    Target = leftEntity;
                    tile.TileFrameX = 54;
                    break;
            }

            UpdateTileActiveState();
        }

        public int GetStyleRow(bool topValid, bool rightValid, bool bottomValid, bool leftValid)
        {
            // Default state
            if (!topValid && !rightValid && !bottomValid && !leftValid)
            {
                return 0;
            }

            int[] topMatrix = { 1, 5, 8, 9, 11, 12, 14, 15 };
            int[] rightMatrix = { 2, 5, 6, 10, 11, 12, 13, 15 };
            int[] bottomMatrix = { 3, 6, 7, 9, 12, 13, 14, 15 };
            int[] leftMatrix = { 4, 7, 8, 10, 11, 13, 14, 15 };

            IEnumerable<int> selection = topMatrix.Union(rightMatrix.Union(bottomMatrix.Union(leftMatrix))).Distinct();

            selection = selection.Where(x => topMatrix.Contains(x) == topValid);
            selection = selection.Where(x => rightMatrix.Contains(x) == rightValid);
            selection = selection.Where(x => bottomMatrix.Contains(x) == bottomValid);
            selection = selection.Where(x => leftMatrix.Contains(x) == leftValid);

            return selection.First();
        }

        public override void Update()
        {
            if (UpdateState)
            {
                UpdateTileState();
                UpdateState = false;
            }

            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
                TransferToTarget();
                QueueItemForNextUpdate();
                UpdateTileActiveState();
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            UpdateNearbyTilesState(true, true, true);
        }

    }
}
