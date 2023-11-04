using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace AutomationDefense.Helpers
{
    public static class CraftingStationsHelper
    {
        private static Dictionary<int, Item> CachedStations { get; set; } = new Dictionary<int, Item>();
        private static Dictionary<int, int> FurnacesPriority = new Dictionary<int, int>()
        {
            {TileID.Furnaces, 1},
            {TileID.Hellforge, 2 },
            {TileID.AdamantiteForge, 3}
        };

        private static Dictionary<int, int> AnvilsPriority = new Dictionary<int, int>()
        {
            {TileID.Anvils, 1},
            {TileID.MythrilAnvil, 2 }
        };
        public static Item CraftingStation(int tileId)
        {
            if (!CachedStations.ContainsKey(tileId))
            {
                CachedStations.Add(tileId, ContentSamples.ItemsByType.Values.FirstOrDefault(x => x.createTile == tileId).NullSafe());
            }

            return CachedStations[tileId];
        }

        // For example, Hellforge is acceptable as a station if the required station is Furnace
        public static bool EligibleStation(int selectedStation, int requiredStation)
        {
            if (selectedStation == requiredStation)
            {
                return true;
            }

            if (FurnacesPriority.ContainsKey(selectedStation) && FurnacesPriority.ContainsKey(requiredStation))
            {
                return FurnacesPriority[selectedStation] > FurnacesPriority[requiredStation];
            }

            if (AnvilsPriority.ContainsKey(selectedStation) && AnvilsPriority.ContainsKey(requiredStation))
            {
                return AnvilsPriority[selectedStation] > AnvilsPriority[requiredStation];
            }

            return false;
        }
    }
}
