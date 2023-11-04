using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AutomationDefense.Helpers
{
    public static class RecipeGroupsHelper
    {

        public static HashSet<int> GetSimilarItems(int itemType)
        {
            foreach (var recipeGroup in RecipeGroup.recipeGroups)
            {
                if (recipeGroup.Value.ContainsItem(itemType))
                {
                    return recipeGroup.Value.ValidItems;
                }
            }
            return null;
        }
    }
}
