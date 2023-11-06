using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AutomationDefense.Objects.WoodHarvester
{
    public class WoodHarvester : ModItem
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
            Item.rare = ItemRarityID.Green;
            Item.createTile = ModContent.TileType<WoodHarvesterTile>();
            Item.value = 50000;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupID.Wood, 500);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 20);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
