using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Inventory
{
    [AddComponentMenu("BlueOrb/Components/Inventory")]
    public class InventoryComponent : ComponentBase<InventoryComponent>, IInventoryComponent
    {
        public const string AddItemMessage = "AddItem";
        public const string RemoveItemMessage = "RemoveItem";
        private long addItemId;

        [SerializeField]
        private InventoryData inventoryData;

        public override void StartListening()
        {
            base.StartListening();
            addItemId = MessageDispatcher.Instance.StartListening(AddItemMessage, _componentRepository.GetId(), (data) =>
            {
                var item = (ItemDesc)data.ExtraInfo;
                Debug.Log($"Adding item {item.ItemConfig.name}, qty {item.Qty}");
                AddItem(item);
            });
            MessageDispatcher.Instance.StartListening(RemoveItemMessage, _componentRepository.GetId(), (data) =>
            {
                if (data.ExtraInfo == null)
                {
                    Debug.LogError($"(InventoryComponent) No item sent to remove.");
                    return;
                }
                var item = (string)data.ExtraInfo;
                RemoveItem(item);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(AddItemMessage, _componentRepository.GetId(), addItemId);
        }

        public void AddItem(ItemDesc item)
        {
            this.inventoryData.Add(item);
            if (item.ItemConfig.InstantiatePrefab && item.ItemConfig.ReferencePrefab != null)
            {
                GameObject.Instantiate(item.ItemConfig.ReferencePrefab, this.transform);
            }
        }

        public void RemoveItem(string uniqueId)
        {
            Debug.Log($"Removing item {uniqueId}");
            this.inventoryData.Remove(uniqueId);
        }

        public bool ContainsItem(string uniqueId) => this.inventoryData.Contains(uniqueId);
    }
}