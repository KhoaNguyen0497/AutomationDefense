using ReLogic.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace AutomationDefense.Helpers
{
    public static class ChestHelper 
    {
        public static bool DepositIntoChest(this Chest chest, Item itemToDeposit)
        {
            if (!itemToDeposit.ValidItem())
            {
                return false;
            }

            // if at least 1 stack is delivered, return true
            int originalAmount = itemToDeposit.stack;

            for (int i = 0; i < chest.item.Length; i++)
            {
                if (itemToDeposit.stack == 0)
                {
                    break;
                }

                var item = chest.item[i];

                // If theres an existing item, merge
                if (item != null && item.type == itemToDeposit.type)
                {
                    int remaining = item.maxStack - item.stack;

                    var deposit = Math.Min(remaining, itemToDeposit.stack);

                    if (deposit == 0)
                    {
                        continue;
                    }

                    item.stack += deposit;
                    itemToDeposit.stack -= deposit;
                }
                else if (item == null || item.IsAir)
                {
                    chest.item[i] = itemToDeposit.Clone();
                    chest.item[i].stack = 0;
                    int remaining = chest.item[i].maxStack;

                    var deposit = Math.Min(remaining, itemToDeposit.stack);

                    chest.item[i].stack = deposit;
                    itemToDeposit.stack -= deposit;
                }
            }

            if (itemToDeposit.stack == 0)
            {
                itemToDeposit.TurnToAir();
                return true;
            }

            return itemToDeposit.stack < originalAmount;         
        }

        public static Item GetFromChest(this Chest chest, int itemId, int stacks, List<int> acceptedGroups)
        {
            var item = new Item(itemId, 0);

            stacks = Math.Min(stacks, item.maxStack);
            if (stacks == 0)
            {
                return null;
            }

            for (int i = 0; i < chest.item.Length; i++)
            {
                if (stacks == 0)
                {
                    break;
                }

                var currentItem = chest.item[i];
                if (ItemHelper.AreSimilarItems(currentItem.type, itemId, acceptedGroups))
                {
                    var stackToGet = Math.Min(currentItem.stack, stacks);

                    currentItem.stack -= stackToGet;
                    item.stack += stackToGet;
                    stacks -= stackToGet;

                    if (currentItem.stack == 0)
                    {
                        chest.item[i].TurnToAir();
                    }
                }
            }

            if (item.ValidItem())
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        public static bool CheckIfChestHasItems(this Chest chest, Dictionary<int, int> items, List<int> acceptedGroups)
        {
            var clonedDict = items.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            foreach (var chestItem in chest.item)
            {
                if (chestItem.ValidItem())
                {
                    foreach (var item in clonedDict)
                    {
                        if (item.Value > 0 && (ItemHelper.AreSimilarItems(item.Key, chestItem.type, acceptedGroups)))
                        {
                            clonedDict[item.Key] = Math.Max(item.Value - chestItem.stack, 0);
                            break;
                        }
                    }
                }
            }

            return clonedDict.All(x => x.Value == 0);
        }

        public static int AvailableSlots(this Chest chest) 
        {
            if (chest == null)
            {
                return 0;
            }

            return chest.item.Count(x => !x.ValidItem());
        }
    }
}
