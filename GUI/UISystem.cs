using AutomationDefense.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using AutomationDefense.Objects;
using AutomationDefense.Objects.OreExtractor;
using AutomationDefense.Objects.AutoCrafter;
using AutomationDefense.Objects.Transportation.Splitter;
using AutomationDefense.GUI.UIStates;
using AutomationDefense.Objects.Transportation.ItemFunnel;

namespace AutomationDefense.GUI
{
    [Autoload(Side = ModSide.Client)] // This attribute makes this class only load on a particular side. Naturally this makes sense here since UI should only be a thing clientside. Be wary though that accessing this class serverside will error
    public class UISystem : ModSystem
    {
        private UserInterface OreExtractorUI;
        internal OreExtractorUIState OreExtractorUIState;

        private UserInterface AutoCrafterUI;
        internal AutoCrafterUIState AutoCrafterUIState;

        private UserInterface SplitterFilterUI;
        internal SplitterFilterUIState SplitterFilterUIState;

        private UserInterface FunnelFilterUI;
        private FunnelFilterUIState FunnelFilterUIState;
        public void ToggleUI<T>(ModTileEntity entity) where T : UIState
        {
            if (OreExtractorUIState is T)
            {
                OreExtractorUIState.ToggleUI(entity as OreExtractorTileEntity);
            }

            if (AutoCrafterUIState is T)
            {
                AutoCrafterUIState.ToggleUI(entity as AutoCrafterTileEntity);
            }

            if (SplitterFilterUIState is T)
            {
                SplitterFilterUIState.ToggleUI(entity as SplitterTileEntity);
            }

            if (FunnelFilterUIState is T)
            {
                FunnelFilterUIState.ToggleUI(entity as ItemFunnelTileEntity);
            }


        }

        public override void Load()
        {
            OreExtractorUI = new UserInterface();
            AutoCrafterUI = new UserInterface();
            SplitterFilterUI = new UserInterface();
            FunnelFilterUI = new UserInterface();

            OreExtractorUIState = new OreExtractorUIState();
            OreExtractorUIState.ParentUserInterface = OreExtractorUI;

            AutoCrafterUIState = new AutoCrafterUIState();
            AutoCrafterUIState.ParentUserInterface = AutoCrafterUI;

            SplitterFilterUIState = new SplitterFilterUIState();
            SplitterFilterUIState.ParentUserInterface = SplitterFilterUI;

            FunnelFilterUIState = new FunnelFilterUIState();
            FunnelFilterUIState.ParentUserInterface = FunnelFilterUI;

            OreExtractorUIState.Activate();
            AutoCrafterUIState.Activate();
            SplitterFilterUIState.Activate();
            FunnelFilterUIState.Activate();
        }


        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.ingameOptionsWindow || Main.InGameUI.IsVisible)
            {
                return;
            }

            OreExtractorUIState.CheckForceCloseUI();
            AutoCrafterUIState.CheckForceCloseUI();
            SplitterFilterUIState.CheckForceCloseUI();
            FunnelFilterUIState.CheckForceCloseUI();

            // Here we call .Update on our custom UI and propagate it to its state and underlying elements
            OreExtractorUIState.CheckUpdate(gameTime);
            AutoCrafterUIState.CheckUpdate(gameTime);
            SplitterFilterUIState.CheckUpdate(gameTime);
            FunnelFilterUIState.CheckUpdate(gameTime);
        }

        // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
        // Setting the InterfaceScaleType to UI for appropriate UI scaling
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "ExampleMod: Coins Per Minute",
                    delegate
                    {
                        OreExtractorUIState.DrawUI();
                        AutoCrafterUIState.DrawUI();
                        SplitterFilterUIState.DrawUI();
                        FunnelFilterUIState.DrawUI();

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
