using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using AutomationDefense.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Localization;
using Terraria.Audio;
using AutomationDefense.Objects.AutoCrafter;

namespace AutomationDefense.GUI.UIStates
{
    public class AutoCrafterUIState : BasicItemSlotsUIState<AutoCrafterTileEntity>
    {
        public UIItemSlot RecipeSlot;
        public UIItemSlot CraftingStationSlot;
        public UIItemSlot CraftingStationSlot2;
        public ItemSlotsPanel IngredientsPanel;
        public ItemSlotsPanel StationPanel;
        public ItemSlotsPanel ResultPanel;

        public UIText AlternateRecipesText;
        public override void SetupCustomUI()
        {
            RecipeSlot.Item = ModTileEntity.RecipeItem;
            CraftingStationSlot.Item = ModTileEntity.CraftingStation;
            CraftingStationSlot2.Item = ModTileEntity.CraftingStation2;

            void OnRecipeChange()
            {
                ModTileEntity.SetRecipeItem(RecipeSlot.Item);

                SetItemsToRender();
            }

            void OnCraftingStationChange()
            {
                ModTileEntity.CraftingStation = CraftingStationSlot.Item;
            }

            void OnCraftingStation2Change()
            {
                ModTileEntity.CraftingStation2 = CraftingStationSlot2.Item;
            }

            RecipeSlot.PostItemExchange = OnRecipeChange;
            CraftingStationSlot.PostItemExchange = OnCraftingStationChange;
            CraftingStationSlot2.PostItemExchange = OnCraftingStation2Change;
            SetItemsToRender();
        }

        private void SetItemsToRender()
        {
            IngredientsPanel.ItemsToRender = ModTileEntity.SelectedRecipe?.requiredItem.NullSafe().Select(x =>
            {
                if (RecipeGroup.recipeGroups.Where(r => ModTileEntity.SelectedRecipe.acceptedGroups.NullSafe().Contains(r.Key)).Any(r => r.Value.ValidItems.Contains(x.type)))
                {
                    if (!x.Name.StartsWith("Any"))
                    {
                        x.SetNameOverride("Any " + x.Name);
                    }
                }

                return x;
            }).ToList();

            StationPanel.ItemsToRender = ModTileEntity.SelectedRecipe?.requiredTile.Select(x => CraftingStationsHelper.CraftingStation(x)).Where(x => x.ValidItem()).ToList();

            ResultPanel.ItemsToRender = ModTileEntity.SelectedRecipe?.createItem.ToList();

            if (ModTileEntity.SelectedRecipe != null)
            {
                AlternateRecipesText.SetText($"Recipe {ModTileEntity.RecipeIndex + 1}/{ModTileEntity.Recipes.Count}");
            }
            else
            {
                AlternateRecipesText.SetText(string.Empty);
            }
        }

        private void CycleButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            ModTileEntity.CycleRecipe();
            SetItemsToRender();
        }

        public override void OnInitialize()
        {
            float paddingSide = 15f;
            float paddingTop = 15f;
            float paddingBottom = 15f;

            BasePanel = new BasePanel();
            BasePanel.SetPadding(0);

            SetRectangle(BasePanel, left: Main.screenWidth / 2 - 200, top: Main.screenHeight / 2, width: 250f, height: 275f);
            BasePanel.BackgroundColor = new Color(73, 94, 171);

            RecipeSlot = new UIItemSlot(0.85f);
            RecipeSlot.Top.Pixels = paddingTop;
            RecipeSlot.Left.Pixels = paddingSide;
            RecipeSlot.Preview = $"AutomationDefense/GUI/PreviewImages/Recipe";
            RecipeSlot.PreviewString = $"Recipe";



            Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("AutomationDefense/GUI/Buttons/RefreshButton");
            UIHoverImageButton cycleButton = new UIHoverImageButton(buttonRefreshTexture, "Cycle alternative recipes");
            SetRectangle(cycleButton, left: RecipeSlot.Left.Pixels + RecipeSlot.Width.Pixels + 5f, top: RecipeSlot.Top.Pixels, width: 25f, height: 25f);
            cycleButton.OnLeftClick += new MouseEvent(CycleButtonClicked);

            AlternateRecipesText = new UIText("", 0.85f);
            AlternateRecipesText.Top.Pixels = cycleButton.Top.Pixels + cycleButton.Height.Pixels + 5f;
            AlternateRecipesText.Left.Pixels = cycleButton.Left.Pixels;

            CraftingStationSlot = new UIItemSlot(0.85f);
            CraftingStationSlot.Top.Pixels = paddingTop;
            CraftingStationSlot.Left.Pixels = AlternateRecipesText.Left.Pixels + 70f + 5f;
            CraftingStationSlot.Preview = $"AutomationDefense/GUI/PreviewImages/Station";
            CraftingStationSlot.PreviewString = $"Crafting Station";

            CraftingStationSlot2 = new UIItemSlot(0.85f);
            CraftingStationSlot2.Top.Pixels = CraftingStationSlot.Top.Pixels;
            CraftingStationSlot2.Left.Pixels = CraftingStationSlot.Left.Pixels + CraftingStationSlot.Width.Pixels + 5f;
            CraftingStationSlot2.Preview = $"AutomationDefense/GUI/PreviewImages/Station";
            CraftingStationSlot2.PreviewString = $"Crafting Station";





            float recipeInfoPaddingSide = 10f; // spacing between the edges of BasePanel
            float recipeInfoPaddingTop = paddingTop + RecipeSlot.Height.Pixels + paddingTop; // spacing between top edge of BasePanel and this panel

            IngredientsPanel = new ItemSlotsPanel("Ingredients:", 0.85f);
            IngredientsPanel.SetPadding(12);
            IngredientsPanel.PaddingLeft = 0f;
            SetRectangle(IngredientsPanel, left: recipeInfoPaddingSide, top: recipeInfoPaddingTop, width: BasePanel.Width.Pixels - recipeInfoPaddingSide - 80f, height: 100f);
            IngredientsPanel.BackgroundColor = Color.Transparent;
            IngredientsPanel.BorderColor = Color.Transparent;
            IngredientsPanel.SetItemSlots(10);

            StationPanel = new ItemSlotsPanel("Stations:", 0.85f);
            StationPanel.SetPadding(12);
            StationPanel.PaddingLeft = 0f;
            StationPanel.PaddingRight = 0f;
            StationPanel.ItemSlotScale = 0.8f;
            SetRectangle(StationPanel, left: recipeInfoPaddingSide + IngredientsPanel.Width.Pixels, top: recipeInfoPaddingTop, width: BasePanel.Width.Pixels - recipeInfoPaddingSide * 2 - IngredientsPanel.Width.Pixels, height: IngredientsPanel.Height.Pixels);
            StationPanel.BackgroundColor = Color.Transparent;
            StationPanel.BorderColor = Color.Transparent;
            StationPanel.SetItemSlots(2);


            ResultPanel = new ItemSlotsPanel("Result:", 0.85f);
            ResultPanel.SetPadding(0);
            ResultPanel.SetPadding(0);
            ResultPanel.PaddingTop = 12f;
            ResultPanel.ItemSlotScale = 0.8f;
            SetRectangle(ResultPanel, left: IngredientsPanel.Left.Pixels, top: IngredientsPanel.Top.Pixels + IngredientsPanel.Height.Pixels, width: IngredientsPanel.Width.Pixels, height: 0f);
            ResultPanel.Height.Set(BasePanel.Height.Pixels - ResultPanel.Top.Pixels - paddingBottom, 0f);
            ResultPanel.BackgroundColor = Color.Transparent;
            ResultPanel.BorderColor = Color.Transparent;
            ResultPanel.SetItemSlots(2);

            BasePanel.Append(RecipeSlot);
            BasePanel.Append(CraftingStationSlot);
            BasePanel.Append(CraftingStationSlot2);
            BasePanel.Append(cycleButton);
            BasePanel.Append(AlternateRecipesText);

            BasePanel.Append(IngredientsPanel);
            BasePanel.Append(StationPanel);
            BasePanel.Append(ResultPanel);
            Append(BasePanel);
        }
    }
}
