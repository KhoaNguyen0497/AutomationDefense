using AutomationDefense.Helpers;
using AutomationDefense.Objects;
using AutomationDefense.Objects.MagicTower;
using AutomationDefense.Objects.WoodHarvesterBase;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace AutomationDefense.Objects.WoodHarvester
{
    public class WoodHarvesterTileEntity : BaseMultiTileEntity
    {
        public override Point16 Origin { get; } = new Point16(0, 0);
        public int Order { get; set; } = 0;
        
        public Point16 HarvesterBasePosition { get; set; } = Point16.Zero;
       
        public override int TicksPerUpdate => 600;

        public int WoodGenerated { get; } = 1;


        public static Dictionary<TreeTypes, int> TreeTypesToWood = new Dictionary<TreeTypes, int>()
        {
            {TreeTypes.Forest, ItemID.Wood },
            {TreeTypes.Corrupt, ItemID.Ebonwood },
            {TreeTypes.Mushroom, ItemID.GlowingMushroom },
            {TreeTypes.Crimson, ItemID.Shadewood },
            {TreeTypes.Jungle, ItemID.RichMahogany },
            {TreeTypes.Snow, ItemID.BorealWood },
            {TreeTypes.Hallowed, ItemID.Pearlwood },
            {TreeTypes.Palm, ItemID.PalmWood },
            {TreeTypes.PalmCorrupt, ItemID.Ebonwood },
            {TreeTypes.PalmCrimson, ItemID.Shadewood },
            {TreeTypes.PalmHallowed, ItemID.Pearlwood },
            {TreeTypes.Ash, ItemID.AshWood },
        };
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<WoodHarvesterTile>();
        }
        public static void DropTreeWood(int type, ref int wood)
        {
            ModTree modTree = PlantLoader.Get<ModTree>(5, type);
            if (modTree != null)
            {
                wood = modTree.DropWood();
            }
        }

        public int FindTreeWood()
        {
            
            int xOffset = Alternate == 0 ? - 1 : + 2;
            var treeTile = Main.tile[Position.X + xOffset, Position.Y + 1];
            var grassTile = Main.tile[HarvesterBasePosition.X + xOffset, HarvesterBasePosition.Y + 3];
            
            if ((WorldGen.IsTreeType(treeTile.TileType) && !TileID.Sets.CountsAsGemTree[treeTile.TileType]) || treeTile.TileType == TileID.PalmTree)
            {
                TreeTypes treeType = WorldGen.GetTreeType(grassTile.TileType);

                if (TreeTypesToWood.TryGetValue(treeType, out int woodId)) 
                {
                    return woodId;
                }
            }

            return 0;
        }

        public override void Update()
        {
            if (Main.GameUpdateCount % TicksPerUpdate == 0 && HarvesterBasePosition != Point16.Zero)
            {
                var woodId = FindTreeWood();
                if (woodId > 0)
                {
                    if (TileHelper.TryGetTileEntity<WoodHarvesterBaseTileEntity>(HarvesterBasePosition.X, HarvesterBasePosition.Y, out var harvesterBaseTileEntity))
                    {
                        if (!harvesterBaseTileEntity.WoodStored.ValidItem() || harvesterBaseTileEntity.WoodStored.type != woodId)
                        {
                            harvesterBaseTileEntity.WoodStored = new Item(woodId, 0);
                        }

                        if (harvesterBaseTileEntity.WoodStored.stack < harvesterBaseTileEntity.WoodStored.maxStack)
                        {
                            harvesterBaseTileEntity.WoodStored.stack += WoodGenerated;
                        }
                    }
                }                         
            }
        }

        // Styling stuff. Set a cap on if the tile is the top tile
        public void UpdateTileState(bool capOn)
        {
            var sideLength = 36;
            var currentTileFrameX = Main.tile[Position.X, Position.Y].TileFrameX;
            short xOffset = 0;
            if (capOn)
            {
                // Already cap
                if (currentTileFrameX < 54)
                {
                    return;
                }
                xOffset = (short)(-sideLength * 2);

            }
            else
            {
                // Already no cap
                if (currentTileFrameX >= 54)
                {
                    return;
                }
                xOffset = (short)(sideLength * 2);
            }

            Main.tile[Position.X, Position.Y].TileFrameX += xOffset;
            Main.tile[Position.X + 1, Position.Y].TileFrameX += xOffset;
            Main.tile[Position.X, Position.Y + 1].TileFrameX += xOffset;
            Main.tile[Position.X + 1, Position.Y + 1].TileFrameX += xOffset;
        }

        public override void SetTilePropeties(int x, int y, int type, int style, int direction, int alternate)
        {
            var sideLength = 36;
            if (TileHelper.TryGetTileEntity<WoodHarvesterTileEntity>(x, y, out var currentHarvester))
            {
                currentHarvester.Alternate = alternate;
                currentHarvester.UpdateTileState(true);

                if (TileHelper.TryGetTileEntity<WoodHarvesterTileEntity>(x, y + 2, out var bottomHarvester))
                {
                    currentHarvester.Order = bottomHarvester.Order + 1;
                    currentHarvester.HarvesterBasePosition = bottomHarvester.HarvesterBasePosition;

                    // Must align the tiles
                    if (bottomHarvester.Alternate !=  alternate)
                    {
                        currentHarvester.Alternate = bottomHarvester.Alternate;

                        // 0 means bottom faces left, current faces right, so -36, else + 36
                        short newXOffset = (short) (bottomHarvester.Alternate == 0 ? -sideLength : sideLength); 
                        Main.tile[x, y].TileFrameX += newXOffset;
                        Main.tile[x + 1, y].TileFrameX += newXOffset;
                        Main.tile[x, y + 1].TileFrameX += newXOffset;
                        Main.tile[x + 1, y + 1].TileFrameX += newXOffset;
                    }

                    // Bottom harvester is not a cap anymore
                    bottomHarvester.UpdateTileState(false);
                }


                // Randomise animation frame
                short newYOffset = (short)((MathHelper.RandomNumber(0, 42) * sideLength) % (sideLength * 43));
                Main.tile[x, y].TileFrameY += newYOffset;
                Main.tile[x + 1, y].TileFrameY += newYOffset;
                Main.tile[x, y + 1].TileFrameY += newYOffset;
                Main.tile[x + 1, y + 1].TileFrameY += newYOffset;

                if (TileHelper.TryGetTileEntity<WoodHarvesterBaseTileEntity>(x, y + 2, out var harvesterBase))
                {
                    harvesterBase.UpdateTileState(false);
                    currentHarvester.HarvesterBasePosition = harvesterBase.Position;
                }
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Order"] = Order;
            tag["HarvestBasePosition"] = HarvesterBasePosition;
            base.SaveData(tag);
        }
        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<int>("Order", out var order))
            {
                Order = order;
            }

            if (tag.TryGet<Point16>("HarvestBasePosition", out var basePos))
            {
                HarvesterBasePosition = basePos;
            }
            base.LoadData(tag);
        }

        public void UpdateNearbyTilesState()
        {
            // It will destroy harvesters above it so we dont have to worry about that

            // Check for harvesters and harvester base below

            if (TileHelper.TryGetTileEntity<WoodHarvesterBaseTileEntity>(Position.X, Position.Y + 2, out var harvesterBase))
            {
                harvesterBase.UpdateTileState(true);
            }

            if (TileHelper.TryGetTileEntity<WoodHarvesterTileEntity>(Position.X, Position.Y + 2, out var harvester))
            {
                harvester.UpdateTileState(true);
            }
        }

        public override void OnKill()
        {
            base.OnKill();
            UpdateNearbyTilesState();
        }
    }
}
