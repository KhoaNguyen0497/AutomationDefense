using AutomationDefense.Helpers;
using AutomationDefense.Objects.Transportation.ItemFunnel;
using AutomationDefense.Objects.Transportation.ItemTransporter;
using AutomationDefense.Objects.Transportation.Splitter;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutomationDefense.Objects.Transportation
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class BaseTransportationTileEntity : ModTileEntity
    {
        public Item InItem { get; set; }
        public Item OutItem { get; set; }
        public Tile ParentTile { get; set; }
        public BaseTransportationTileEntity Target { get; set; }

        public int TicksPerUpdate = 10;

        public Direction Direction { get; set; } = Direction.Up;

        public bool UpdateState { get; set; }

        public bool InActiveState()
        {
            return OutItem.ValidItem();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            throw new NotImplementedException();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["InItem"] = InItem;
            tag["OutItem"] = OutItem;
            tag["Direction"] = Direction.ToString();
        }
        public override void LoadData(TagCompound tag)
        {
            Enum.TryParse<Direction>(tag.Get<string>("Direction"), out var direction);
            Direction = direction;
            ParentTile = Main.tile[Position.X, Position.Y];
            try
            {
                InItem = tag.Get<Item>("InItem");
                OutItem = tag.Get<Item>("OutItem");
            }
            catch
            {
                InItem = null;
                OutItem = null;
            }

            UpdateState = true;
        }

        public virtual bool AcceptItem()
        {
            return !InItem.ValidItem();
        }

        public void UpdateTileActiveState()
        {
            Tile tile = Main.tile[Position.X, Position.Y];
            if (InActiveState())
            {
                tile.TileFrameX = (short)(tile.TileFrameX % 72 + 72);
            }
            else
            {
                tile.TileFrameX = (short)(tile.TileFrameX % 72);
            }
        }

        /// <summary>
        /// Get Position(<paramref name="x"/>, <paramref name="y"/>) in <paramref name="direction"/> 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetSidePosition(Direction direction, out int x, out int y)
        {
            x = 0;
            y = 0;
            switch (direction)
            {
                case Direction.Up:
                    x = Position.X;
                    y = Position.Y - 1;
                    break;
                case Direction.Down:
                    x = Position.X;
                    y = Position.Y + 1;
                    break;
                case Direction.Right:
                    x = Position.X + 1;
                    y = Position.Y;
                    break;
                case Direction.Left:
                    x = Position.X - 1;
                    y = Position.Y;
                    break;
            }

        }
        public void SetInitialProperties<T>(int x, int y)
            where T : BaseTransportationTileEntity
        {
            ParentTile = Main.tile[x, y];


            if (TileHelper.TryGetTileEntity<T>(x, y, out var tileEntity))
            {
                tileEntity.ParentTile = ParentTile;
            }
        }

        public bool TransferToTarget()
        {
            if (Target != null && Target.AcceptItem() && OutItem.ValidItem())
            {
                Target.InItem = OutItem.Clone();
                OutItem.TurnToAir();
                return true;
            }
            return false;
        }


        public void QueueItemForNextUpdate()
        {
            // Put queued item in output slot if output slot is empty
            if (InItem.ValidItem() && !OutItem.ValidItem())
            {
                OutItem = InItem.Clone();
                InItem.TurnToAir();
            }
        }

        public void UpdateNearbyTilesState(bool updateTransporters, bool updateFunnels, bool updateSplitter)
        {
            if (updateTransporters)
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
            }

            if (updateFunnels)
            {
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

            if (updateSplitter)
            {
                // Splitter
                if (TileHelper.TryGetTileEntity<SplitterTileEntity>(Position.X - 1, Position.Y, out var leftSplitter))
                {
                    leftSplitter.UpdateState = true;
                }

                if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(Position.X + 1, Position.Y, out var rightSplitter))
                {
                    rightSplitter.UpdateState = true;
                }

                if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(Position.X, Position.Y - 1, out var topSplitter))
                {
                    topSplitter.UpdateState = true;
                }

                if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(Position.X, Position.Y + 1, out var bottomSplitter))
                {
                    bottomSplitter.UpdateState = true;
                }
            }
        }

        public override void OnKill()
        {
            GetAdditionalDrops();
        }

        public virtual void GetAdditionalDrops()
        {
            SpawnDropItem(InItem);
            SpawnDropItem(OutItem);
        }

        public void SpawnDropItem(Item item)
        {
            if (!item.ValidItem())
            {
                return;
            }
            var position = Position.ToWorldCoordinates();
            int i = Item.NewItem(Main.LocalPlayer.GetSource_FromThis(), (int)position.X, (int)position.Y, 32, 32, item.type);
            item.position = Main.item[i].position;
            Main.item[i] = item;
            var drop = Main.item[i];
            item = new Item();
            drop.velocity.Y = -2f;
            drop.velocity.X = Main.rand.NextFloat(-4f, 4f);
            drop.favorited = false;
            drop.newAndShiny = false;
        }
    }

}
