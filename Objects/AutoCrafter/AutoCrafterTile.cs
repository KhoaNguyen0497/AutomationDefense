using AutomationDefense.GUI;
using AutomationDefense.GUI.UIStates;
using AutomationDefense.Helpers;
using AutomationDefense.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationDefense.Objects.AutoCrafter
{
    public class AutoCrafterTile : BaseMultilTile<AutoCrafterTileEntity>
    {
        public override int ItemType => ModContent.ItemType<AutoCrafter>();
        public override Point16 Origin { get; } = new Point16(3, 3);

        public override void SetStaticDefaults()
        {
            SetStaticDefaults(4, 4);
        }

        public override bool RightClick(int i, int j)
        {
            if (TileHelper.TryGetTileEntity<AutoCrafterTileEntity>(i, j, out var autoCrafter))
            {
                ModContent.GetInstance<UISystem>().ToggleUI<AutoCrafterUIState>(autoCrafter);
            }

            return base.RightClick(i, j);
        }

        public override void SetAlternate()
        {
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 4)
            {
                frameCounter = 0;
                frame = ++frame % 45;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<AutoCrafterTileEntity>().Kill(i, j);
        }
    }
}
