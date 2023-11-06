using AutomationDefense.Objects.Transportation;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationDefense.Objects.Transportation.ItemTransporter
{
    public class ItemTransporter : BaseTransportationItem<ItemTransporterTile, ItemTransporterTileEntity>
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.AutomationDefense.hjson file.

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

    }
}