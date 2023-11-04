using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace AutomationDefense.Helpers
{
    public static class ItemHelper
    {
        public static bool ValidItem(this Item item)
        {
            return (item != null && !item.IsAir);
        }

        public static Item NullSafe(this Item item)
        {
            if (item == null)
            {
                item = new Item();
                item.SetDefaults();
            }

            return item;
        }

        public static int RemainingStack(this Item item)
        {
            return item.maxStack - item.stack;
        }

        // Check if both items belong in the same group
        public static bool AreSimilarItems(int item1, int item2, List<int> acceptedGroups)
        {
            if (item1 == item2)
            {
                return true;
            }

            foreach (var recipeGroup in RecipeGroup.recipeGroups.Where(x => acceptedGroups.Contains(x.Key)))
            {
                if (recipeGroup.Value.ContainsItem(item1) && recipeGroup.Value.ContainsItem(item2))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
