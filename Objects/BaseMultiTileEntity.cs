using AutomationDefense.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AutomationDefense.Objects
{
    public abstract class BaseMultiTileEntity : ModTileEntity
    {
        public virtual int TicksPerUpdate { get; } = 60; // 1second per update
        public int Alternate;
        public abstract Point16 Origin { get; } // this must be the same as the one in MultiTile
        public override int Hook_AfterPlacement(int x, int y, int type, int style, int direction, int alternate)
        {
            int placedEntity = Place(x - Origin.X, y - Origin.Y);
            SetTilePropeties(x, y, type, style, direction, alternate);
            return placedEntity;
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            throw new NotImplementedException();
        }

        public virtual void SetTilePropeties(int x, int y, int type, int style, int direction, int alternate)
        {

        }

        public override void SaveData(TagCompound tag)
        {
            tag["Alternate"] = Alternate;
        }
        public override void LoadData(TagCompound tag)
        {
            tag.TryGet("Alternate", out Alternate);
        }


        public override void OnKill()
        {
            GetAdditionalDrops();
        }

        public virtual void GetAdditionalDrops()
        {

        }

        public void SpawnDropItem(Item item)
        {
            if (!item.ValidItem())
            {
                return;
            }
            var position = Position.ToWorldCoordinates();
            int i = Item.NewItem(Main.LocalPlayer.GetSource_FromThis(), (int)position.X, (int)position.Y, 32, 32, item.type);
            item.position = Main.item[i].position;
            Main.item[i] = item;
            var drop = Main.item[i];
            item = new Item();
            drop.velocity.Y = -2f;
            drop.velocity.X = Main.rand.NextFloat(-4f, 4f);
            drop.favorited = false;
            drop.newAndShiny = false;
        }
    }
}
