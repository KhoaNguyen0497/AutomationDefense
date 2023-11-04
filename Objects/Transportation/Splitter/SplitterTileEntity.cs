using AutomationDefense.Helpers;
using AutomationDefense.Objects.Transportation;
using AutomationDefense.Objects.Transportation.ItemFunnel;
using AutomationDefense.Objects.Transportation.ItemTransporter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutomationDefense.Objects.Transportation.Splitter
{
    public class SplitterTileEntity : BaseTransportationTileEntity
    {
        public bool HasOutputTile;

        public static int NumberOfFilters { get; set; } = 3;
        public List<Item> TopFilters { get; set; } = new List<Item>(new Item[NumberOfFilters]);
        public List<Item> BottomFilters { get; set; } = new List<Item>(new Item[NumberOfFilters]);
        public List<Item> LeftFilters { get; set; } = new List<Item>(new Item[NumberOfFilters]);
        public List<Item> RightFilters { get; set; } = new List<Item>(new Item[NumberOfFilters]);

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<SplitterTile>();
        }

        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            int placedEntity = Place(x, y);
            if (TileHelper.TryGetTileEntity<SplitterTileEntity>(x, y, out var tileEntity))
            {
                tileEntity.UpdateNearbyTilesState();
                tileEntity.UpdateState = true;
            }
            return placedEntity;
        }

        public void UpdateNearbyTilesState()
        {
            // Transporter
            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(Position.X - 1, Position.Y, out var leftTransporter))
            {
                leftTransporter.UpdateState = true;
            }

            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(Position.X + 1, Position.Y, out var rightTransporter))
            {
                rightTransporter.UpdateState = true;
            }

            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(Position.X, Position.Y - 1, out var topTransporter))
            {
                topTransporter.UpdateState = true;
            }

            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(Position.X, Position.Y + 1, out var bottomTransporter))
            {
                bottomTransporter.UpdateState = true;
            }

            // Funnel
            if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(Position.X - 1, Position.Y, out var leftFunnel))
            {
                leftFunnel.UpdateState = true;
            }

            if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(Position.X + 1, Position.Y, out var rightFunnel))
            {
                rightFunnel.UpdateState = true;
            }

            if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(Position.X, Position.Y - 1, out var topFunnel))
            {
                topFunnel.UpdateState = true;
            }

            if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(Position.X, Position.Y + 1, out var bottomFunnel))
            {
                bottomFunnel.UpdateState = true;
            }
        }

        public bool ValidConnection(Direction direction, out ItemTransporterTileEntity tileEntity)
        {
            int x = 0;
            int y = 0;
            tileEntity = null;

            GetSidePosition(direction, out x, out y);

            bool valid = false;

            if (TileHelper.TryGetTileEntity<ItemTransporterTileEntity>(x, y, out var transporter))
            {
                tileEntity = transporter;
                valid = true;
            }

            return valid;
        }

        public void UpdateTileState()
        {
            HasOutputTile = GetOutputTiles().Count > 0;
        }

        public override void Update()
        {
            if (UpdateState)
            {
                UpdateTileState();
                UpdateState = false;
            }

            // This one should update faster than the rest to allow splitting/merging seamlessly
            if (Main.GameUpdateCount % (TicksPerUpdate / 2) == 0)
            {
                // If InItem is the same type as OutItem, merge it first to make this faster
                if (InItem.ValidItem() && OutItem.ValidItem() && InItem.type == OutItem.type)
                {
                    if (InItem.stack <= OutItem.RemainingStack())
                    {
                        OutItem.stack += InItem.stack;
                        InItem.TurnToAir();
                    }
                    else
                    {
                        var remainingStack = OutItem.RemainingStack();
                        InItem.stack -= remainingStack;
                        OutItem.stack += remainingStack;
                    }
                }

                if (OutItem.ValidItem() && HasOutputTile)
                {
                    // Transfer and split evenly (only for tiles that currently accept items)
                    // As of right now it means all the outputTiles are empty and therefore stack can be split evenly
                    var outputTiles = GetOutputTiles().Where(x => x.AcceptItem()).ToList();
                    if (outputTiles.Any())
                    {
                        // At this point, OutItem.stack > 0 because OutItem.Valid() and outputTiles.Count() > 0 because outputTiles.Any()
                        var splits = MathHelper.DivideEvenly(OutItem.stack, outputTiles.Count()).ToList();
                        for (int i = 0; i < splits.Count(); i++)
                        {
                            var stackToTransfer = splits.ElementAt(i);
                            if (stackToTransfer == 0)
                            {
                                continue;
                            }

                            outputTiles.ElementAt(i).InItem = new Item(OutItem.type, stackToTransfer);
                        }

                        OutItem.TurnToAir();
                    }
                }

                // Requeue items
                QueueItemForNextUpdate();
            }
        }

        // Assuming item is valid
        public bool MatchFilter(Direction direction, Item item)
        {
            if (!item.ValidItem())
            {
                return true;
            }

            // If there are no filters set for any direction, or none that matches, return true
            if (TopFilters.Union(BottomFilters).Union(LeftFilters).Union(RightFilters).All(x => x.NullSafe().type != item.type))
            {
                return true;
            }

            // If there are matching filters set in any direction, only return true for that direction
            switch (direction)
            {
                case Direction.Up: return TopFilters.Any(x=>x.NullSafe().type == item.type);
                case Direction.Down: return BottomFilters.Any(x=>x.NullSafe().type == item.type);
                case Direction.Left: return LeftFilters.Any(x=>x.NullSafe().type == item.type);
                case Direction.Right: return RightFilters.Any(x=>x.NullSafe().type == item.type);
            }

            return false;
        }

        public List<ItemTransporterTileEntity> GetOutputTiles()
        {
            List<ItemTransporterTileEntity> outputTiles = new List<ItemTransporterTileEntity>();


            if (MatchFilter(Direction.Up, OutItem))
            {
                if (ValidConnection(Direction.Up, out var tileUp) && tileUp.Direction != Direction.Down)
                {
                    outputTiles.Add(tileUp);
                }
            }

            if (MatchFilter(Direction.Down, OutItem))
            {
                if (ValidConnection(Direction.Down, out var tileDown) && tileDown.Direction != Direction.Up)
                {
                    outputTiles.Add(tileDown);
                }
            }

            if (MatchFilter(Direction.Left, OutItem))
            {
                if (ValidConnection(Direction.Left, out var tileLeft) && tileLeft.Direction != Direction.Right)
                {
                    outputTiles.Add(tileLeft);
                }
            }

            if (MatchFilter(Direction.Right, OutItem))
            {
                if (ValidConnection(Direction.Right, out var tileRight) && tileRight.Direction != Direction.Left)
                {
                    outputTiles.Add(tileRight);
                }
            }
        
            return outputTiles;
        }

        public override bool AcceptItem()
        {
            return HasOutputTile && base.AcceptItem();
        }


        public override void OnKill()
        {
            base.OnKill();
            UpdateNearbyTilesState();
        }

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < NumberOfFilters; i++)
            {
                tag[$"TopFilter{i}"] = TopFilters[i];
                tag[$"BottomFilter{i}"] = BottomFilters[i];
                tag[$"LeftFilter{i}"] = LeftFilters[i];
                tag[$"RightFilter{i}"] = RightFilters[i];
            }

            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < NumberOfFilters; i++)
            {
                if (tag.TryGet<Item>($"TopFilter{i}", out var topItem))
                {
                    TopFilters[i] = topItem;
                }


                if (tag.TryGet<Item>($"BottomFilter{i}", out var bottomItem))
                {
                    BottomFilters[i] = bottomItem;
                }

                if (tag.TryGet<Item>($"LeftFilter{i}", out var leftItem))
                {
                    LeftFilters[i] = leftItem;
                }

                if (tag.TryGet<Item>($"RightFilter{i}", out var rightItem))
                {
                    RightFilters[i] = rightItem;
                }
            }
            base.LoadData(tag);
        }
        public override void GetAdditionalDrops()
        {
            foreach (var item in TopFilters.Union(BottomFilters).Union(LeftFilters).Union(RightFilters))
            {
                SpawnDropItem(item);
            }

            base.GetAdditionalDrops();
        }
    }
}
