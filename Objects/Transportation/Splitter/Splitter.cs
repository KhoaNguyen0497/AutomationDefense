using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using AutomationDefense.Objects.Transportation;

namespace AutomationDefense.Objects.Transportation.Splitter
{
    public class Splitter : BaseTransportationItem<SplitterTile, SplitterTileEntity>
    {
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 2);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
