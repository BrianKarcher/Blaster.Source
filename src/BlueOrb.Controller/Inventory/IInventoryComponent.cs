using BlueOrb.Base.Item;

namespace BlueOrb.Controller.Inventory
{
    public interface IInventoryComponent
    {
        void AddItem(ItemDesc item);
        void RemoveItem(string uniqueId);
        void StartListening();
        void StopListening();
    }
}