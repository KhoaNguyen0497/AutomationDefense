using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AutomationDefense.Objects.OreExtractor
{
    public class OreExtractor : ModItem
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
            Item.createTile = ModContent.TileType<OreExtractorTile>();
            Item.value = 50000;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 200);
            recipe.AddIngredient(ItemID.GoldBar, 100);
            recipe.AddIngredient(ItemID.StoneBlock, 1000);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddRecipeGroup(RecipeGroupID.IronBar, 200);
            recipe2.AddIngredient(ItemID.PlatinumBar, 100);
            recipe2.AddIngredient(ItemID.StoneBlock, 1000);
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.Register();
        }
    }
}
