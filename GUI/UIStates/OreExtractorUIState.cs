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
using AutomationDefense.Objects.OreExtractor;

namespace AutomationDefense.GUI.UIStates
{
    public class OreExtractorUIState : BasicItemSlotsUIState<OreExtractorTileEntity>
    {
        public UIItemSlot PickaxeSlot;
        public UIItemSlot OreSlot;

        public override void SetupCustomUI()
        {
            PickaxeSlot.Item = ModTileEntity.Pickaxe;
            OreSlot.Item = ModTileEntity.OreFilter;

            void OnPickaxeExchange()
            {
                ModTileEntity.Pickaxe = PickaxeSlot.Item;
            }

            void OnOreFilterChange()
            {
                ModTileEntity.OreFilter = OreSlot.Item;
            }

            PickaxeSlot.PostItemExchange = OnPickaxeExchange;
            PickaxeSlot.ItemFilter = (i) =>
            {
                // We allow putting nothing in here
                if (!i.ValidItem())
                {
                    return true;
                }

                return i.NullSafe().pick > 0;
            };

            OreSlot.PostItemExchange = OnOreFilterChange;
            OreSlot.ItemFilter = (i) =>
            {
                // We allow putting nothing in here
                if (!i.ValidItem())
                {
                    return true;
                }

                return OreExtractorTileEntity.AllowedOres.Keys.Contains(i.NullSafe().type);
            };
        }

        public override void OnInitialize()
        {
            float paddingSide = 15;
            float paddingTop = 15;

            BasePanel = new BasePanel();
            BasePanel.SetPadding(0);

            SetRectangle(BasePanel, left: Main.screenWidth / 2 - 200, top: Main.screenHeight / 2, width: 150f, height: 100f);
            BasePanel.BackgroundColor = new Color(73, 94, 171);



            PickaxeSlot = new UIItemSlot(0.85f);
            PickaxeSlot.Top.Pixels = paddingTop;
            PickaxeSlot.Left.Pixels = paddingSide;
            PickaxeSlot.Preview = $"AutomationDefense/GUI/PreviewImages/Pickaxe";
            PickaxeSlot.PreviewString = $"Pickaxe";

            OreSlot = new UIItemSlot(0.85f);
            OreSlot.Top.Pixels = paddingTop;
            OreSlot.Left.Pixels = BasePanel.Width.Pixels - paddingSide - OreSlot.Width.Pixels;
            OreSlot.Preview = $"AutomationDefense/GUI/PreviewImages/Ore";
            OreSlot.PreviewString = $"Ore filter";


            BasePanel.Height.Set(PickaxeSlot.Height.Pixels + paddingTop * 2, 0f);
            BasePanel.Append(PickaxeSlot);
            BasePanel.Append(OreSlot);
            Append(BasePanel);
        }
    }

}