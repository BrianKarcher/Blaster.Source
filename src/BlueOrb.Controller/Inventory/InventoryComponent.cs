using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Inventory
{
    [AddComponentMenu("BlueOrb/Components/Inventory")]
    public class InventoryComponent : ComponentBase<InventoryComponent>, IInventoryComponent
    {
        public const string RemoveItemMessage = "RemoveItem";
        private long addItemId;

        [SerializeField]
        private InventoryData inventoryData;

        public override void StartListening()
        {
            base.StartListening();
            addItemId = MessageDispatcher.Instance.StartListening("AddItem", _componentRepository.GetId(), (data) =>
            {
                var item = (ItemDesc)data.ExtraInfo;
                Debug.Log($"Adding item {item.ItemConfig.name}, qty {item.Qty}");
                AddItem(item);
            });
            MessageDispatcher.Instance.StartListening(RemoveItemMessage, _componentRepository.GetId(), (data) =>
            {
                var item = (ItemDesc)data.ExtraInfo;
                RemoveItem(item);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("AddItem", _componentRepository.GetId(), addItemId);
        }

        public void AddItem(ItemDesc item)
        {
            this.inventoryData.Add(item);
            if (item.ItemConfig.InstantiatePrefab && item.ItemConfig.ReferencePrefab != null)
            {
                GameObject.Instantiate(item.ItemConfig.ReferencePrefab, this.transform);
            }
        }

        public void RemoveItem(ItemDesc item)
        {
            Debug.Log($"Removing item {item.ItemConfig.name}");
            this.inventoryData.Remove(item.ItemConfig.UniqueId);
        }
    }
}