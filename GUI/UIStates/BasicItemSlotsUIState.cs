using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using AutomationDefense.Helpers;
using AutomationDefense.Objects;
using Terraria.ModLoader;

namespace AutomationDefense.GUI.UIStates
{
    public abstract class BasicItemSlotsUIState<T> : UIState where T : ModTileEntity
    {
        public UserInterface ParentUserInterface { get; set; }

        public BasePanel BasePanel;
        public virtual T ModTileEntity { get; set; }
        public Vector2 PositionWhenOpen;

        public void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }
        public void ToggleUI(T entity)
        {
            // close
            if (ParentUserInterface.CurrentState != null)
            {
                // a different UI is opened
                if (entity != ModTileEntity)
                {
                    CloseUI();
                    OpenUI(entity);
                }
                else
                {
                    CloseUI();
                }
            }
            // open
            else
            {
                OpenUI(entity);
            }
        }

        public void CloseUI()
        {
            ParentUserInterface?.SetState(null);
            SoundEngine.PlaySound(SoundID.MenuClose);
        }

        public void OpenUI(T entity)
        {
            SetNeccessaryVariables();
            ParentUserInterface?.SetState(this);
            ModTileEntity = entity;
            SetupCustomUI();
            SoundEngine.PlaySound(SoundID.MenuOpen);
        }

        public void SetNeccessaryVariables()
        {
            Main.playerInventory = true;
            Main.LocalPlayer.chest = -1;
            Main.LocalPlayer.sign = -1;
            Main.LocalPlayer.SetTalkNPC(-1);
            PositionWhenOpen = Main.LocalPlayer.position;
        }

        public abstract void SetupCustomUI();

        public void CheckForceCloseUI()
        {
            if (ShouldCloseAllUI())
            {
                ParentUserInterface.SetState(null);
            }
        }

        public bool ShouldCloseAllUI()
        {
            float maxDistance = 300;
            return Main.LocalPlayer.chest != -1 ||
                !Main.playerInventory ||
                Main.LocalPlayer.sign > -1 ||
                Main.LocalPlayer.talkNPC > -1 ||
                !VectorHelper.WithinDistance(PositionWhenOpen, Main.LocalPlayer.position, maxDistance);
        }

        public void CheckUpdate(GameTime gameTime)
        {
            if (ParentUserInterface?.CurrentState != null)
                ParentUserInterface?.Update(gameTime);
        }

        public void DrawUI()
        {
            if (ParentUserInterface?.CurrentState != null)
                ParentUserInterface.Draw(Main.spriteBatch, new GameTime());
        }
    }
}
