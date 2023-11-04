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

namespace AutomationDefense.GUI.UIStates
{
    public class SplitterFilterUIState : BasicItemSlotsUIState<SplitterTileEntity>
    {
        public List<UIItemSlot> TopSlots = new List<UIItemSlot>(SplitterTileEntity.NumberOfFilters);
        public List<UIItemSlot> BottomSlots = new List<UIItemSlot>(SplitterTileEntity.NumberOfFilters);
        public List<UIItemSlot> LeftSlots = new List<UIItemSlot>(SplitterTileEntity.NumberOfFilters);
        public List<UIItemSlot> RightSlots = new List<UIItemSlot>(SplitterTileEntity.NumberOfFilters);
        public override void SetupCustomUI()
        {
          
            void ItemChange()
            {
                for (int i = 0; i < SplitterTileEntity.NumberOfFilters; i++)
                {
                    ModTileEntity.TopFilters[i] = TopSlots[i].Item;
                    ModTileEntity.BottomFilters[i] = BottomSlots[i].Item;
                    ModTileEntity.LeftFilters[i] = LeftSlots[i].Item;
                    ModTileEntity.RightFilters[i] = RightSlots[i].Item;
                }
            }

            for (int i = 0; i < SplitterTileEntity.NumberOfFilters; i++)
            {
                TopSlots[i].Item = ModTileEntity.TopFilters[i];
                BottomSlots[i].Item = ModTileEntity.BottomFilters[i];
                LeftSlots[i].Item = ModTileEntity.LeftFilters[i];
                RightSlots[i].Item = ModTileEntity.RightFilters[i];

                TopSlots[i].PostItemExchange = ItemChange;

                BottomSlots[i].PostItemExchange = ItemChange;

                LeftSlots[i].PostItemExchange = ItemChange;

                RightSlots[i].PostItemExchange = ItemChange;
            }
        }

        public override void OnInitialize()
        {
            BasePanel = new BasePanel();
            SetRectangle(BasePanel, left: Main.screenWidth / 2 - 200, top: Main.screenHeight / 2, width: 250f, height: 250f);

            float horizontalSpacing = 0f;
            float verticalSpacing = 0f;

            float slotSpacing = 2f;
            float slotScaling = 0.75f;
            BasePanel.SetPadding(7);

            for (int i = 0; i < SplitterTileEntity.NumberOfFilters; i++)
            {
                // Top
                var topSlot = new UIItemSlot(slotScaling);
                topSlot.Left.Pixels = horizontalSpacing;
                topSlot.PreviewString = "Top Filter";

                TopSlots.Add(topSlot);

                // Bottom
                var bottomSlot = new UIItemSlot(slotScaling);
                bottomSlot.Left.Pixels = horizontalSpacing;
                bottomSlot.PreviewString = "Bottom Filter";

                BottomSlots.Add(bottomSlot);

                horizontalSpacing += topSlot.Width.Pixels + slotSpacing;

                // Left
                var leftSlot = new UIItemSlot(slotScaling);
                leftSlot.Top.Pixels = verticalSpacing;
                leftSlot.PreviewString = "Left Filter";

                LeftSlots.Add(leftSlot);

                // Right
                var rightSlot = new UIItemSlot(slotScaling);
                rightSlot.Top.Pixels = verticalSpacing;
                rightSlot.PreviewString = "Right Filter";

                RightSlots.Add(rightSlot);

                verticalSpacing += leftSlot.Height.Pixels + slotSpacing;
            }


            // redo spacing
            float totalHorizontalWidth = TopSlots.Last().Left.Pixels + TopSlots.Last().Width.Pixels - TopSlots.First().Left.Pixels;
            float totalVerticalHeight = LeftSlots.Last().Top.Pixels + LeftSlots.Last().Height.Pixels - LeftSlots.First().Top.Pixels;
            for (int i = 0; i < SplitterTileEntity.NumberOfFilters; i++)
            {
                TopSlots[i].Top.Pixels = BasePanel.PaddingTop;
                TopSlots[i].Left.Pixels += (BasePanel.Width.Pixels - totalHorizontalWidth) / 2 - BasePanel.PaddingLeft;

                BottomSlots[i].Top.Pixels = BasePanel.Height.Pixels - BasePanel.PaddingBottom * 3 - BottomSlots[i].Height.Pixels;
                BottomSlots[i].Left.Pixels = TopSlots[i].Left.Pixels;   

                LeftSlots[i].Left.Pixels = BasePanel.PaddingLeft;
                LeftSlots[i].Top.Pixels += (BasePanel.Height.Pixels - totalVerticalHeight) / 2 - BasePanel.PaddingTop;

                RightSlots[i].Left.Pixels = BasePanel.Width.Pixels - BasePanel.PaddingRight * 3 - RightSlots[i].Width.Pixels;
                RightSlots[i].Top.Pixels = LeftSlots[i].Top.Pixels;

                BasePanel.Append(TopSlots[i]);
                BasePanel.Append(BottomSlots[i]);
                BasePanel.Append(LeftSlots[i]);
                BasePanel.Append(RightSlots[i]);
            }

            Append(BasePanel);
        }
    }
}
