using BlueOrb.Base.Item;
using System;
using System.Collections.Generic;

namespace BlueOrb.Controller.Inventory
{
    /// <summary>
    /// This gets "owned" by the InventoryComponent, as well as getting saved and loaded with the game
    /// </summary>
    [Serializable]
    public class InventoryData
    {
        public Dictionary<string, ItemDesc> Items = new Dictionary<string, ItemDesc>();

        public void Add(ItemDesc item)
        {
            // Prevent duplication of Items, just adjust the Qty if the item is already in the inventory
            if (!Items.TryGetValue(item.ItemConfig.UniqueId, out ItemDesc itemConfigAndCount))
            {
                itemConfigAndCount = item.Clone();
                Items.Add(item.ItemConfig.UniqueId, itemConfigAndCount);
            }
            else
            {
                itemConfigAndCount.Qty += item.Qty;
            }
            if (itemConfigAndCount.Qty <= 0)
            {
                Remove(itemConfigAndCount.ItemConfig.UniqueId);
            }
        }

        public void Remove(string uniqueId)
        {
            if (Items.ContainsKey(uniqueId))
            {
                Items.Remove(uniqueId);
            }
        }

        public bool Contains(string uniqueId) => Items.ContainsKey(uniqueId);
    }
}