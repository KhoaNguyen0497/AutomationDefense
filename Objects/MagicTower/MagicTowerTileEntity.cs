using AutomationDefense.Helpers;
using Microsoft.Xna.Framework;
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

namespace AutomationDefense.Objects.MagicTower
{
    public class MagicTowerTileEntity : BaseMultiTileEntity
    {
        public Vector2 OriginOffset = new Vector2(2,1);
        public override int TicksPerUpdate { get; } = 10; // 6 updates per second
        public Item CurrentBullet { get; set; }
        public NPC CurrentEnemy { get; set; }
        public float MaxDetectDistance { get; } = 1000f;

        public Chest LeftChest { get; set; }
        public Chest RightChest { get; set; }

        public int BaseDamage { get; } = 20;
        public override Point16 Origin { get; } = new Point16(1,6);

        public int NumOfBulletsStored { get; } = 100;

        public Vector2 PositionWordCoordinates()
        {
            return (Position.ToVector2() + OriginOffset).ToWorldCoordinates(0, 0);
        }

        public override void SetTilePropeties(int x, int y, int type, int style, int direction, int alternate)
        {
            if (TileHelper.TryGetTileEntity<MagicTowerTileEntity>(x, y, out var tileEntity))
            {
                tileEntity.Alternate = alternate;
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<MagicTowerTile>();
        }
        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            var placedEntity = base.Hook_AfterPlacement(x, y, type, style, direction, alternate);

            return placedEntity;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["CurrentBullet"] = CurrentBullet;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<Item>("CurrentBullet", out var bullet))
            {
                CurrentBullet = bullet;
            }
            base.LoadData(tag);
        }

        public void GetChests()
        {
            LeftChest = null;
            RightChest = null;
            var leftChestIndex = Chest.FindChest(Position.X - 2, Position.Y + 5);
            var rightChestIndex = Chest.FindChest(Position.X + 3, Position.Y + 5);

            if (leftChestIndex != -1)
            {
                LeftChest = Main.chest[leftChestIndex];
            }
            if (rightChestIndex != -1)
            {
                RightChest = Main.chest[rightChestIndex];
            }
        }

        public override void Update()
        {
            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
                FindClosestNPC(1000f, PositionWordCoordinates());

                if (CurrentEnemy == null)
                {
                    return;
                }

                // No bullet
                if (!CurrentBullet.ValidItem())
                {
                    GetChests();
                    if (LeftChest != null)
                    {
                        for (int i = 0; i < LeftChest.item.Length; i++)
                        {
                            var currentItem = LeftChest.item[i];
                            if (currentItem.ValidItem() && currentItem.ammo == AmmoID.Bullet)
                            {
                                CurrentBullet = currentItem.Clone();
                                CurrentBullet.stack = Math.Min(currentItem.stack, NumOfBulletsStored);
                                currentItem.stack -= CurrentBullet.stack;

                                if (currentItem.stack <= 0)
                                {
                                    LeftChest.item[i].TurnToAir();
                                }

                                break;
                            }
                        }
                    }
                    
                    if (!CurrentBullet.ValidItem() && RightChest != null)
                    {
                        if (!CurrentBullet.ValidItem())
                        {
                            for (int i = 0; i < RightChest.item.Length; i++)
                            {
                                var currentItem = RightChest.item[i];
                                if (currentItem.ValidItem() && currentItem.ammo == AmmoID.Bullet)
                                {
                                    CurrentBullet = currentItem.Clone();
                                    CurrentBullet.stack = Math.Min(currentItem.stack, NumOfBulletsStored);
                                    currentItem.stack -= CurrentBullet.stack;

                                    if (currentItem.stack <= 0)
                                    {
                                        RightChest.item[i].TurnToAir();
                                    }

                                    break;
                                }
                            }
                        }
                    }
                   
                    
                    if (!CurrentBullet.ValidItem())
                    {
                        return;
                    }
                }
              
                var targetVector = CurrentEnemy.Center - PositionWordCoordinates();
                targetVector = targetVector.RotatedByRandom(Microsoft.Xna.Framework.MathHelper.ToRadians(5));
                targetVector.Normalize();
                Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), PositionWordCoordinates(), targetVector * 10, CurrentBullet.shoot, BaseDamage + CurrentBullet.damage, 0f);
                Dust.NewDust(PositionWordCoordinates(), 1, 1, DustID.Ichor, 0f, 0f, 150, default(Color), 0.75f);
                

                CurrentBullet.stack -= 1;
                if (CurrentBullet.stack == 0)
                {
                    CurrentBullet.TurnToAir();
                }
            }
        }

        public bool ValidEnemy(NPC npc)
        {
            if (npc == null)
            {
                return false;
            }

            if (!npc.CanBeChasedBy() || !VectorHelper.WithinDistance(npc.Center, PositionWordCoordinates(), MaxDetectDistance))
            {
                return false;
            }

            return true;
        }

        public void FindClosestNPC(float maxDetectDistance, Vector2 currentPosition)
        {
            if (ValidEnemy(CurrentEnemy))
            {
                return;
            }
            else
            {
                CurrentEnemy = null;
            }

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (ValidEnemy(target))
                {
                    CurrentEnemy = target;
                }
            }
        }
        public override void GetAdditionalDrops()
        {
            SpawnDropItem(CurrentBullet);
        }
    }
}
