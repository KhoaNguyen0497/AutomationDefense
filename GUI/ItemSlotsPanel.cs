using AutomationDefense.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace AutomationDefense.GUI
{
    public class ItemSlotsPanel : UIPanel
    { 
        public List<UIItemSlot> ItemSlots { get; set; }
        public UIText Title { get; set; }
        public float SlotsSideSpacing { get; set; } = 2f;
        public float SlotsTopSpacing { get; set; } = 2f;

        public float ItemSlotScale { get; set; } = 0.5f;
        public int MaxSlotsPerRow { get; set; } = 5;

        public float SpaceBetweenTitleAndSlots { get; set; } = 10f;
        public List<Item> ItemsToRender = new List<Item>();

        public ItemSlotsPanel(string title, float titleScale) 
        { 
            ItemSlots = new List<UIItemSlot>();
            
            if (!string.IsNullOrEmpty(title))
            {
                Title = new UIText(title, titleScale);
                Append(Title);
            }    
        }

        private float CalculateSlotLeft(float width, int column)
        {
            return width * column + SlotsSideSpacing * (column + 1);
        }
        private float CalculateSlotTop(float height, int row)
        {
            var yOffSet = 0f;
            if (Title != null)
            {
                yOffSet = Title.Height.Pixels + SpaceBetweenTitleAndSlots + PaddingTop;
            }

            return height * row + SlotsTopSpacing * (row + 1) + yOffSet;
        }


        public void SetItemSlots(int slots)
        {
            int row = 0;
            int column = 0;

            for (int i = 0; i < slots; i++)
            {
                var slot = new UIItemSlot(ItemSlotScale);
                slot.EnableItemSwap = false;
                slot.EnableBackgoundTexture = true;
                if (column > MaxSlotsPerRow - 1 || CalculateSlotLeft(slot.Width.Pixels, column) + slot.Width.Pixels > Width.Pixels - PaddingRight)
                {
                    row++;
                    column = 0;
                }

                slot.Top.Set(CalculateSlotTop(slot.Height.Pixels, row), 0f);
                slot.Left.Set(CalculateSlotLeft(slot.Width.Pixels, column), 0f);
                Append(slot);
                ItemSlots.Add(slot);
                column++;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int i = 0; i < ItemSlots.Count; i++)
            {
                ItemSlots[i].Item.NullSafe().TurnToAir();
                ItemSlots[i].EnableDraw = false;

                if (ItemsToRender?.Count() > i)
                {
                    ItemSlots[i].Item = ItemsToRender[i].Clone();
                    ItemSlots[i].EnableDraw = true;

                }
            }
        }
    }
}
