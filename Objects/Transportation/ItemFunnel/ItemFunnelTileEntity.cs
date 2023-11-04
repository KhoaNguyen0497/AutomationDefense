using AutomationDefense.Helpers;
using AutomationDefense.Objects.Transportation;
using AutomationDefense.Objects.Transportation.ItemTransporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Server;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace AutomationDefense.Objects.Transportation.ItemFunnel
{

    public class ItemFunnelTileEntity : BaseTransportationTileEntity
    {
        public static int NumberOfFilters { get; } = 5;
        public List<Item> Filters { get; set; } = new List<Item>(new Item[NumberOfFilters]);
        public bool OutputMode { get; set; } // Funnel will transfer to the connected Target (transporter) instead of into a chest

        public bool ConnectedToValidTile { get; set; }
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ItemFunnelTile>();
        }

        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            int placedEntity = Place(x, y);
            SetInitialProperties<ItemFunnelTileEntity>(x, y);
            if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(x, y, out var tileEntity))
            {
                tileEntity.UpdateNearbyTilesState(true, false, false);
                tileEntity.UpdateState = true;
            }
            return placedEntity;
        }

        // direction can be any side
        public bool ValidConnection(Direction direction, out BaseTransportationTileEntity tileEntity)
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


        public int GetStyleRow()
        {
            if (ConnectedToValidTile)
            {
                if (OutputMode)
                {
                    return 2;
                }

                return 1;
            }

            return 0;
        }
        public void UpdateTileState()
        {
            // transporter
            BaseTransportationTileEntity entity = null;

            Tile tile = Main.tile[Position.X, Position.Y];

            switch (Direction)
            {
                case Direction.Up:
                    ConnectedToValidTile = ValidConnection(Direction.Down, out entity);
                    tile.TileFrameX = 0;
                    break;
                case Direction.Right:
                    ConnectedToValidTile = ValidConnection(Direction.Left, out entity);
                    tile.TileFrameX = 18;
                    break;
                case Direction.Down:
                    ConnectedToValidTile = ValidConnection(Direction.Up, out entity);
                    tile.TileFrameX = 36;
                    break;
                case Direction.Left:
                    ConnectedToValidTile = ValidConnection(Direction.Right, out entity);
                    tile.TileFrameX = 54;
                    break;
            }

            Target = ConnectedToValidTile ? entity : null;

            if (Target != null)
            {
                OutputMode = Target.Direction != Direction;
            }

            tile.TileFrameY = (short)(GetStyleRow() * 18);
            UpdateTileActiveState();
        }

        public override void Update()
        {
            if (UpdateState)
            {
                UpdateTileState();
                UpdateState = false;
            }

            // Target is NOT null regardless of output mode as long as it is connected
            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
                // Get the chest connected to the main side of the funnel (not the side with transporter)
                var chest = GetAdjcentChest();

                if (ConnectedToValidTile)
                {
                    // From chest to transporter
                    if (OutputMode)
                    {
                        // First push the item to the transporter
                        if (OutItem.ValidItem() && Target.AcceptItem())
                        {
                            Target.InItem = OutItem.Clone();
                            OutItem.TurnToAir();
                        }

                        // Second, pull the next item from chest
                        if (!InItem.ValidItem() && chest != null)
                        {
                            for (int i = 0; i < chest.item.Length; i++)
                            {
                                if (chest.item[i].ValidItem())
                                {
                                    if (Filters.All(x => !x.ValidItem()) || Filters.Any(x => x.type == chest.item[i].type))
                                    {
                                        InItem = chest.item[i].Clone();
                                        chest.item[i].TurnToAir();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else //Input mode. Dont have to pull because Transporter pushes into it.
                    {
                        // Push to chest
                        if (OutItem.ValidItem() && chest != null)
                        {
                            chest.DepositIntoChest(OutItem);
                        }
                    }
                }

                QueueItemForNextUpdate();
                UpdateTileActiveState();
            }

        }

        public Chest GetAdjcentChest()
        {
            int chestIndex = -1;

            switch (Direction)
            {
                case Direction.Down:
                    chestIndex = Chest.FindChest(Position.X, Position.Y + 1);

                    if (chestIndex == -1)
                    {
                        chestIndex = Chest.FindChest(Position.X - 1, Position.Y + 1);
                    }
                    break;
                case Direction.Up:
                    chestIndex = Chest.FindChest(Position.X, Position.Y - 2);
                    if (chestIndex == -1)
                    {
                        chestIndex = Chest.FindChest(Position.X - 1, Position.Y - 2);
                    }
                    break;
                case Direction.Right:
                    chestIndex = Chest.FindChest(Position.X + 1, Position.Y);
                    if (chestIndex == -1)
                    {
                        chestIndex = Chest.FindChest(Position.X + 1, Position.Y - 1);
                    }
                    break;
                case Direction.Left:
                    chestIndex = Chest.FindChest(Position.X - 2, Position.Y);
                    if (chestIndex == -1)
                    {
                        chestIndex = Chest.FindChest(Position.X - 2, Position.Y - 1);
                    }
                    break;
            }

            if (chestIndex != -1)
            {
                return Main.chest[chestIndex];
            }

            return null;
        }

        public override void OnKill()
        {
            base.OnKill();
            UpdateNearbyTilesState(true, false, false);
        }

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < NumberOfFilters; i++)
            {
                tag[$"Filter{i}"] = Filters[i];
            }

            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < NumberOfFilters; i++)
            {
                if (tag.TryGet<Item>($"Filter{i}", out var item))
                {
                    Filters[i] = item;
                }
                else
                {
                    Filters[i] = null;
                }               
            }

            base.LoadData(tag);
        }
        public override void GetAdditionalDrops()
        {
            foreach (var item in Filters)
            {
                SpawnDropItem(item);
            }

            base.GetAdditionalDrops();
        }
    }
}
