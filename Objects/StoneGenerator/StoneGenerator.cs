using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AutomationDefense.Objects.StoneGenerator
{
    public class StoneGenerator : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Orange;
            Item.createTile = ModContent.TileType<StoneGeneratorTile>();
            Item.value = 50000;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LavaBucket, 10);
            recipe.AddIngredient(ItemID.WaterBucket, 10);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 50);
            recipe.AddIngredient(ItemID.Obsidian, 50);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
