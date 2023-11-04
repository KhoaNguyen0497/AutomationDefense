using AutomationDefense.Helpers;

using System.Collections.Generic;
using System.Linq;

using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutomationDefense.Objects.OreExtractor
{
    public class OreExtractorTileEntity : BaseMultiTileEntity
    {
        public Item Pickaxe;
        public Item OreFilter;
        public int BaseOreChance = 1;
        public int StonesPerUpdate = 100;
        public override Point16 Origin { get; } = new Point16(3, 4);

        public static Dictionary<int, int> AllowedOres = new Dictionary<int, int>()
        {
            { ItemID.CopperOre, 0 },
            { ItemID.TinOre, 0 },
            { ItemID.IronOre, 0 },
            { ItemID.LeadOre, 0 },
            { ItemID.SilverOre, 0 },
            { ItemID.TungstenOre, 0 },
            { ItemID.GoldOre, 35 },
            { ItemID.PlatinumOre, 35 },
            { ItemID.Meteorite, 50 },
            { ItemID.DemoniteOre, 55 },
            { ItemID.CrimtaneOre, 55 },
            { ItemID.Obsidian, 55 },
            { ItemID.Hellstone, 65 },
            { ItemID.CobaltOre, 100 },
            { ItemID.PalladiumOre, 100 },
            { ItemID.MythrilOre, 110 },
            { ItemID.OrichalcumOre, 110 },
            { ItemID.AdamantiteOre, 150 },
            { ItemID.TitaniumOre, 150 },
            { ItemID.ChlorophyteOre, 200 },

        };

        public override void SetTilePropeties(int x, int y, int type, int style, int direction, int alternate)
        {
            if (TileHelper.TryGetTileEntity<OreExtractorTileEntity>(x, y, out var tileEntity))
            {
                tileEntity.Alternate = alternate;
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<OreExtractorTile>();
        }
        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            var placedEntity = base.Hook_AfterPlacement(x, y, type, style, direction, alternate);

            return placedEntity;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Pickaxe"] = Pickaxe;
            tag["OreFilter"] = OreFilter;

            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("Pickaxe", out Pickaxe))
            {
                Pickaxe = Pickaxe.NullSafe();
            }

            if (tag.TryGet("OreFilter", out OreFilter))
            {
                OreFilter = OreFilter.NullSafe();
            }

            base.LoadData(tag);
        }

        public override void Update()
        {
            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
                if (Pickaxe != null && Pickaxe.pick > 0)
                {
                    var inputChestIndex = Chest.FindChest(Alternate == 0 ? Position.X + 7 : Position.X - 2, Position.Y + 3);

                    if (inputChestIndex == -1)
                    {
                        return;
                    }
                    var outputChestIndex = Chest.FindChest(Alternate == 0 ? Position.X - 2 : Position.X + 7, Position.Y + 3);

                    if (outputChestIndex == -1)
                    {
                        return;
                    }

                    var inputChest = Main.chest[inputChestIndex];
                    var outputChest = Main.chest[outputChestIndex];

                    var stones = inputChest.GetFromChest(ItemID.StoneBlock, StonesPerUpdate, new List<int>());
                    if (!stones.ValidItem())
                    {
                        return;
                    }

                    IEnumerable<int> validOres = AllowedOres.Where(x => x.Value <= Pickaxe.pick).Select(x => x.Key);
                    if (OreFilter.ValidItem())
                    {
                        validOres = validOres.Where(x => x == OreFilter.type);
                    }
                    for (int i = 0; i < stones.stack; i++)
                    {
                        if (MathHelper.Chance(BaseOreChance))
                        {
                            var randomOre = new Item(validOres.RandomElement());
                            randomOre.stack = 1;
                            outputChest.DepositIntoChest(randomOre);
                        }
                    }
                }
            }
        }


        public override void GetAdditionalDrops()
        {
            SpawnDropItem(Pickaxe);
            SpawnDropItem(OreFilter);
        }
    }
}
