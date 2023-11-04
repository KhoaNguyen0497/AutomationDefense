using AutomationDefense.Objects.Transportation;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationDefense.Objects.Transportation.ItemFunnel
{
    public class ItemFunnel : BaseTransportationItem<ItemFunnelTile, ItemFunnelTileEntity>
    {
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}