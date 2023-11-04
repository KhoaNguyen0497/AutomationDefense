using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace AutomationDefense
{
    public class RecipeSystem : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.MusketBall, 70);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
