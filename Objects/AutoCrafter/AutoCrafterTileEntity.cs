using AutomationDefense.GUI;
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
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutomationDefense.Objects.AutoCrafter
{
    public class AutoCrafterTileEntity : BaseMultiTileEntity
    {
        public Item RecipeItem { get; private set; }
        public Item CraftingStation;
        public Item CraftingStation2;
        public List<Recipe> Recipes = new List<Recipe>();
        public override Point16 Origin { get; } = new Point16(3, 3);

        public int RecipeIndex { get; set; }

        public Recipe SelectedRecipe
        {
            get
            {
                if (RecipeIndex > -1 && Recipes.Count > RecipeIndex)
                {
                    return Recipes[RecipeIndex];
                }

                return null;
            }
        }


        public void CycleRecipe()
        {
            if (RecipeIndex == -1)
            {
                return;
            }

            RecipeIndex++;

            if (RecipeIndex >= Recipes.Count)
            {
                RecipeIndex = 0;
            }
        }

        public void SetRecipeItem(Item item, int index = -1)
        {
            Recipes = new List<Recipe>();
            RecipeIndex = -1;
            RecipeItem = item.NullSafe();

            if (RecipeItem.ValidItem())
            {
                foreach (Recipe recipe in Main.recipe)
                {
                    if (recipe.createItem.type == RecipeItem.type && !recipe.Disabled)
                    {
                        Recipes.Add(recipe);
                    }
                }

                if (Recipes.Count > 0)
                {
                    if (index > -1)
                    {
                        RecipeIndex = index;
                    }
                    else
                    {
                        RecipeIndex = 0;
                    }
                }
                else
                {
                    RecipeIndex = -1;
                }
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {

            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<AutoCrafterTile>();
        }

        public override void Update()
        {
            if (Main.GameUpdateCount % TicksPerUpdate == 0)
            {
                if (SelectedRecipe != null)
                {
                    var inputChestIndex = Chest.FindChest(Alternate == 0 ? Position.X + 4 : Position.X - 2, Position.Y + 2);

                    if (inputChestIndex == -1)
                    {
                        return;
                    }
                    var outputChestIndex = Chest.FindChest(Alternate == 0 ? Position.X - 2 : Position.X + 4, Position.Y + 2);

                    if (outputChestIndex == -1)
                    {
                        return;
                    }

                    Chest inputChest = Main.chest[inputChestIndex];
                    Chest outputChest = Main.chest[outputChestIndex];

                    var requiredItems = new Dictionary<int, int>();
                    foreach (var item in SelectedRecipe.requiredItem)
                    {
                        if (requiredItems.ContainsKey(item.type))
                        {
                            requiredItems[item.type] += item.stack;
                        }
                        else
                        {
                            requiredItems[item.type] = item.stack;
                        }
                    }

                    if (inputChest.CheckIfChestHasItems(requiredItems, SelectedRecipe.acceptedGroups.NullSafe()))
                    {
                        // The Where makes it so that DemonAltar items are free to craft
                        var requiredStations = SelectedRecipe.requiredTile.Select(x => CraftingStationsHelper.CraftingStation(x)).Where(x => x.ValidItem());
                        var hasStations = false;

                        // This means anything that doesnt require a tile nearby can be free to craft
                        // Including things with conditions like near water, near honey, etc. because these are not tiles
                        if (requiredStations.Count() == 0)
                        {
                            hasStations = true;
                        }
                        else
                        {
                            if (requiredStations.All(x => CraftingStationsHelper.EligibleStation(CraftingStation.NullSafe().createTile, x.createTile) || CraftingStationsHelper.EligibleStation(CraftingStation2.NullSafe().createTile, x.createTile)))
                            {
                                hasStations = true;
                            }
                        }

                        if (hasStations)
                        {
                            // CRAFT THE ITEM
                            if (outputChest.DepositIntoChest(SelectedRecipe.createItem.Clone()))
                            {
                                foreach (var item in requiredItems)
                                {
                                    inputChest.GetFromChest(item.Key, item.Value, SelectedRecipe.acceptedGroups.NullSafe());
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void SetTilePropeties(int x, int y, int type, int style, int direction, int alternate)
        {
            if (TileHelper.TryGetTileEntity<AutoCrafterTileEntity>(x, y, out var tileEntity))
            {
                tileEntity.Alternate = alternate;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["RecipeItem"] = RecipeItem;
            tag["RecipeIndex"] = RecipeIndex;
            tag["CraftingStation"] = CraftingStation;
            tag["CraftingStation2"] = CraftingStation2;
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<int>("RecipeIndex", out var recipeIndex))
            {
                RecipeIndex = recipeIndex;
            }
            else
            {
                RecipeIndex = -1;
            }

            if (tag.TryGet<Item>("RecipeItem", out var item))
            {
                SetRecipeItem(item, RecipeIndex);
            }

            if (tag.TryGet("CraftingStation", out CraftingStation))
            {
                CraftingStation = CraftingStation.NullSafe();
            }

            if (tag.TryGet("CraftingStation2", out CraftingStation2))
            {
                CraftingStation2 = CraftingStation2.NullSafe();
            }


            base.LoadData(tag);
        }

        public override void GetAdditionalDrops()
        {
            SpawnDropItem(RecipeItem);
            SpawnDropItem(CraftingStation);
            SpawnDropItem(CraftingStation2);
        }
    }
}
