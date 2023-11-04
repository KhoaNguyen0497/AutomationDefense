using AutomationDefense.Objects.Transportation.Splitter;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria;
using System.Security.Cryptography;
using AutomationDefense.Objects.Transportation.ItemFunnel;

namespace AutomationDefense.GUI.UIStates
{
    public class FunnelFilterUIState : BasicItemSlotsUIState<ItemFunnelTileEntity>
    {
        public List<UIItemSlot> Filters = new List<UIItemSlot>(ItemFunnelTileEntity.NumberOfFilters);

        public override void SetupCustomUI()
        {
          
            void ItemChange()
            {
                for (int i = 0; i < ItemFunnelTileEntity.NumberOfFilters; i++)
                {
                    ModTileEntity.Filters[i] = Filters[i].Item;
                }
            }

            for (int i = 0; i < ItemFunnelTileEntity.NumberOfFilters; i++)
            {
                Filters[i].Item = ModTileEntity.Filters[i];

                Filters[i].PostItemExchange = ItemChange;

            }
        }

        public override void OnInitialize()
        {
            BasePanel = new BasePanel();
            SetRectangle(BasePanel, left: Main.screenWidth / 2 - 200, top: Main.screenHeight / 2, width: 275f, height: 80f);

            float horizontalSpacing = 0f;

            float slotSpacing = 2f;
            float slotScaling = 0.75f;
            BasePanel.SetPadding(7);

            for (int i = 0; i < ItemFunnelTileEntity.NumberOfFilters; i++)
            {
                var slot = new UIItemSlot(slotScaling);
                slot.Left.Pixels = horizontalSpacing;
                slot.PreviewString = "Filter";

                Filters.Add(slot);
                horizontalSpacing += slot.Width.Pixels + slotSpacing;              
            }


            // redo spacing
            float totalHorizontalWidth = Filters.Last().Left.Pixels + Filters.Last().Width.Pixels - Filters.First().Left.Pixels;
            for (int i = 0; i < ItemFunnelTileEntity.NumberOfFilters; i++)
            {
                Filters[i].Top.Pixels = BasePanel.PaddingTop;
                Filters[i].Left.Pixels += (BasePanel.Width.Pixels - totalHorizontalWidth) / 2 - BasePanel.PaddingLeft;

                BasePanel.Append(Filters[i]);
            }

            Append(BasePanel);
        }
    }
}
