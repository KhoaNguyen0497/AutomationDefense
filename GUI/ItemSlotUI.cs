using AutomationDefense.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace AutomationDefense.GUI
{
	public class UIItemSlot : UIElement
	{
		public Texture2D backgroundTexture = TextureAssets.InventoryBack4.Value;
		public float scale;
		public Item Item;
		public Action PostItemExchange;
		public Func<Item, bool> ItemFilter;
		public string Preview;
		public string PreviewString;
		public bool EnableItemSwap = true;
		public bool EnableDraw = true;
		public bool EnableBackgoundTexture = true;
        public UIItemSlot(float scale = 1f)
		{
			this.scale = scale;
			Width.Set(backgroundTexture.Width * scale, 0f);
			Height.Set(backgroundTexture.Height * scale, 0f);
		}

		public override void LeftMouseDown(UIMouseEvent evt)
		{
			if (!EnableItemSwap)
			{
				return;
			}

			Item = Item.NullSafe();

			if (ItemFilter != null)
			{
				if (!ItemFilter.Invoke(Main.mouseItem))
				{
					return;
				}
			}

            // if its the same item, move on
            if (Main.mouseItem.type == Item.type)
			{
                base.LeftMouseDown(evt);
				return;
            }

            Utils.Swap(ref Item, ref Main.mouseItem);     

            if (Main.mouseItem.ValidItem() || Item.ValidItem())
            {
                SoundEngine.PlaySound(SoundID.Grab);
            }

			if (PostItemExchange != null)
			{
				PostItemExchange.Invoke();
			}

            base.LeftMouseDown(evt);
        }


		// Code courtesy of AutoReforge. Check them out, its a great mod
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (!EnableDraw)
			{
				return;
			}

            Item = Item.NullSafe();

            Vector2 position = GetInnerDimensions().Position();
			if (EnableBackgoundTexture)
			{
                spriteBatch.Draw(backgroundTexture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
            if (Item.ValidItem())
			{
                Main.instance.LoadItem(Item.type);

                Texture2D itemTexture = TextureAssets.Item[Item.type].Value;
				Rectangle textureFrame = Main.itemAnimations[Item.type]?.GetFrame(itemTexture) ?? itemTexture.Bounds;

				Color newColor = Color.White;
				float pulseScale = 1f;
				ItemSlot.GetItemLight(ref newColor, ref pulseScale, Item, false);
				int height = textureFrame.Height;
				int width = textureFrame.Width;
				float drawScale = 1f;
				float availableWidth = 32; // defaultBackgroundTexture.Width * scale;
				if (width > availableWidth || height > availableWidth)
				{
					drawScale = availableWidth / (width > height ? width : height);
				}
				drawScale *= scale;
				Vector2 itemPosition = position + backgroundTexture.Size() * scale / 2f - textureFrame.Size() * drawScale / 2f;
				Vector2 itemOrigin = textureFrame.Size() * (pulseScale / 2f - 0.5f);
				if (ItemLoader.PreDrawInInventory(Item, spriteBatch, itemPosition, textureFrame, Item.GetAlpha(newColor),
					Item.GetColor(Color.White), itemOrigin, drawScale * pulseScale))
				{
					spriteBatch.Draw(itemTexture, itemPosition, textureFrame, Item.GetAlpha(newColor), 0f, itemOrigin, drawScale * pulseScale, SpriteEffects.None, 0f);
					if (Item.color != Color.Transparent)
					{
						spriteBatch.Draw(itemTexture, itemPosition, textureFrame, Item.GetColor(Color.White), 0f, itemOrigin, drawScale * pulseScale, SpriteEffects.None, 0f);
					}
				}
				ItemLoader.PostDrawInInventory(Item, spriteBatch, itemPosition, textureFrame, Item.GetAlpha(newColor),
					Item.GetColor(Color.White), itemOrigin, drawScale * pulseScale);
				if (ItemID.Sets.TrapSigned[Item.type])
				{
					spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
				}

				if (Item.stack > 1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Item.stack.ToString(), position + new Vector2(10f, 26f) * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
				}

				if (IsMouseHovering)
				{
					Main.HoverItem = Item.Clone();
					Main.hoverItemName = Main.HoverItem.Name;
				}
			}
			else if(!string.IsNullOrEmpty(Preview))
			{
                Asset<Texture2D> texture = ModContent.Request<Texture2D>(Preview);
				if (texture != null)
				{
                    var origin = texture.Size() / 2f;
                    spriteBatch.Draw(texture.Value, GetDimensions().Center(), null, Color.White * 0.5f, 0f, origin, 1, SpriteEffects.None, 0f);
                }
            }

			if (!string.IsNullOrEmpty(PreviewString) && IsMouseHovering && Main.mouseItem.IsAir)
			{
                Main.instance.MouseText(PreviewString);
            }
        }
	}
}
