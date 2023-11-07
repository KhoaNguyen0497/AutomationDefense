using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AutomationDefense
{
    public class ExtendedItem : GlobalItem
    {
        public bool MarkedForAutoPickup { get; set; }
        public override bool InstancePerEntity => true;

        public override bool CanPickup(Item item, Player player)
        {
            if (MarkedForAutoPickup)
            {
                return false;
            }
            return base.CanPickup(item, player);
        }

    }
}
