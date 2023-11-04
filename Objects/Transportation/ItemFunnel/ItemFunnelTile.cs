using AutomationDefense.GUI;
using AutomationDefense.GUI.UIStates;
using AutomationDefense.Helpers;
using AutomationDefense.Objects.AutoCrafter;
using AutomationDefense.Objects.Transportation;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationDefense.Objects.Transportation.ItemFunnel
{
    public class ItemFunnelTile : BaseTransportationTile<ItemFunnelTileEntity>
    {
        public override int ItemType => ModContent.ItemType<ItemFunnel>();

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType;
            player.noThrow = 2;
        }

        public override bool RightClick(int i, int j)
        {
            if (Main.keyState.PressingShift())
            {
                if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(i, j, out var funnel))
                {
                    ModContent.GetInstance<UISystem>().ToggleUI<FunnelFilterUIState>(funnel);
                    return base.RightClick(i, j);
                }
            }

            if (TileHelper.TryGetTileEntity<ItemFunnelTileEntity>(i, j, out var tileEntity))
            {
                switch (tileEntity.Direction)
                {
                    case Direction.Up:
                        tileEntity.Direction = Direction.Right;
                        break;
                    case Direction.Right:
                        tileEntity.Direction = Direction.Down;
                        break;
                    case Direction.Down:
                        tileEntity.Direction = Direction.Left;
                        break;
                    case Direction.Left:
                        tileEntity.Direction = Direction.Up;
                        break;
                }
                tileEntity.UpdateState = true;
                tileEntity.UpdateNearbyTilesState(true, false, false);
            }

            return base.RightClick(i, j);
        }
    }
}

