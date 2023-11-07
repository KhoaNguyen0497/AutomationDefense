using AutomationDefense.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutomationDefense.Objects.ItemVacuum
{
    public class ItemVacuumTileEntity : BaseMultiTileEntity
    {
        public override Point16 Origin { get; } = new Point16(2,2);
        public List<Item> MarkedItems { get; set; } = new List<Item>();

        public float ItemVacuumSpeed = 10f;

        public float VacuumRadius = 250f;

        public Tuple<Func<Item, bool>, int, float, float> UpgradeItem { get; } = new Tuple<Func<Item, bool>, int, float, float>((Item i) => i.type == ItemID.MeteoriteBar, 5, 25f, 1000f); // ItemID, Stack required, Upgrade, Max Upgrade
        public Tuple<Func<Item, bool>, int, float, float> DowngradeItem { get; } = new Tuple<Func<Item, bool>, int, float, float>((Item i) => i.hammer > 0, 1, 25f, 100f); // ItemID, Stack required, Upgrade, Max Upgrade
        public Vector2 PositionWordCoordinates()
        {
            return (Position + Origin).ToVector2().ToWorldCoordinates(0, 0);
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<ItemVacuumTile>();
        }
        public void GetChests(out Chest leftChest, out Chest rightChest)
        {
            leftChest = null;
            rightChest = null;
            var leftChestIndex = Chest.FindChest(Position.X - 2, Position.Y + 2);
            var rightChestIndex = Chest.FindChest(Position.X + 5, Position.Y + 2);

            if (leftChestIndex != -1)
            {
                leftChest = Main.chest[leftChestIndex];
            }
            if (rightChestIndex != -1)
            {
                rightChest = Main.chest[rightChestIndex];
            }
        }

        public void UpdateRadius()
        {
            if (!Main.LocalPlayer.HeldItem.ValidItem())
            {
                return;
            }

            // Has the right item
            if (UpgradeItem.Item1(Main.LocalPlayer.HeldItem))
            {
                // Has the right stack
                if (Main.LocalPlayer.HeldItem.stack >= UpgradeItem.Item2)
                {
                    // Not reached max radius yet
                    if (VacuumRadius < UpgradeItem.Item4)
                    {
                        Main.LocalPlayer.HeldItem.stack -= UpgradeItem.Item2;
                        VacuumRadius += UpgradeItem.Item3;
                        VacuumRadius = Math.Min(UpgradeItem.Item4, VacuumRadius);
                        SoundEngine.PlaySound(SoundID.Tink);
                        if (Main.LocalPlayer.HeldItem.stack == 0)
                        {
                            Main.LocalPlayer.HeldItem.TurnToAir();
                        }
                    }
                }
            }

            // Has the right item
            if (DowngradeItem.Item1(Main.LocalPlayer.HeldItem))
            {
                // Dont have to check for stack for now. 
                // Not reached min radius yet
                if (VacuumRadius > DowngradeItem.Item3)
                {
                    SoundEngine.PlaySound(SoundID.Tink);

                    VacuumRadius -= DowngradeItem.Item3;
                    VacuumRadius = Math.Max(DowngradeItem.Item4, VacuumRadius);
                }
            }
        }
        public void DrawGrabRadius()
        {
            if (!Main.LocalPlayer.HeldItem.ValidItem())
            {
                return;
            }

            if (UpgradeItem.Item1(Main.LocalPlayer.HeldItem) || DowngradeItem.Item1(Main.LocalPlayer.HeldItem) || Main.LocalPlayer.HeldItem.type == ModContent.ItemType<ItemVacuum>())
            {
                Vector2 startingPoint = PositionWordCoordinates() - Vector2.UnitX * VacuumRadius;
                int dustCount = 10;
                for (int i = 0; i < dustCount; i++)
                {
                    var rotatedPosition = startingPoint.RotatedBy(Microsoft.Xna.Framework.MathHelper.ToRadians(Main.GameUpdateCount % 360 + (360 / dustCount) * i), PositionWordCoordinates());
                    Dust.NewDust(rotatedPosition, 1, 1, DustID.ShadowbeamStaff, 0f, 0f, 255, default(Color), 1f);
                }
            }
        }

        public void VacuumItems()
        {
            MarkedItems = MarkedItems.Where(x => x.ValidItem() && x.GetGlobalItem<ExtendedItem>().MarkedForAutoPickup).ToList();

            foreach (var item in MarkedItems)
            {
                item.position = item.position.MoveTowards(PositionWordCoordinates(), ItemVacuumSpeed);
                // If close enough to store items
                if (VectorHelper.WithinDistance(item.position, PositionWordCoordinates(), 16))
                {
                    GetChests(out var leftChest, out var rightChest);

                    // Regardless of whether we can deliver into chest or not, set autopick up to false
                    item.GetGlobalItem<ExtendedItem>().MarkedForAutoPickup = false;
                    if (leftChest != null)
                    {
                        leftChest.DepositIntoChest(item);
                    }
                    if (rightChest != null && item.ValidItem())
                    {
                        rightChest.DepositIntoChest(item);
                    }

                    Dust.NewDust(PositionWordCoordinates(), 1, 1, DustID.TreasureSparkle, 0f, 0f, 255, default(Color), 2.5f);
                    Dust.NewDust(PositionWordCoordinates(), 1, 1, DustID.TreasureSparkle, 0f, 0f, 255, default(Color), 1.5f);
                    Dust.NewDust(PositionWordCoordinates(), 1, 1, DustID.TreasureSparkle, 0f, 0f, 255, default(Color), 1f);
                    Dust.NewDust(PositionWordCoordinates(), 1, 1, DustID.TreasureSparkle, 0f, 0f, 255, default(Color), 1.5f);
                    Dust.NewDust(PositionWordCoordinates(), 1, 1, DustID.TreasureSparkle, 0f, 0f, 255, default(Color), 2.5f);
                }
            }
        }

    

        public override void Update()
        {
            VacuumItems();

            DrawGrabRadius();

            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
                GetChests(out var leftChest, out var rightChest);
                int availableSlots = leftChest.AvailableSlots() + rightChest.AvailableSlots();
                foreach (var item in Main.item)
                {
                    if (availableSlots == 0)
                    {
                        break;
                    }

                    if (ValidForAutoloot(item))
                    {
                        item.GetGlobalItem<ExtendedItem>().MarkedForAutoPickup = true;
                        MarkedItems.Add(item);
                        availableSlots -= 1;
                    }
                }
            }
        }

        public bool ValidForAutoloot(Item item)
        {
            return item.ValidItem()
                && !item.beingGrabbed
                && !ItemID.Sets.ItemsThatShouldNotBeInInventory[item.type]
                && !ItemID.Sets.CommonCoin[item.type]
                && !item.GetGlobalItem<ExtendedItem>().MarkedForAutoPickup
                && VectorHelper.WithinDistance(item.position, PositionWordCoordinates(), VacuumRadius);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["VacuumRadius"] = VacuumRadius;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<float>("VacuumRadius", out var r))
            {
                VacuumRadius = r;
            }
            base.LoadData(tag);
        }

        public override void OnKill()
        {
            foreach (var item in MarkedItems)
            {
                item.GetGlobalItem<ExtendedItem>().MarkedForAutoPickup = false;
            }
            base.OnKill();
        }
    }
}
