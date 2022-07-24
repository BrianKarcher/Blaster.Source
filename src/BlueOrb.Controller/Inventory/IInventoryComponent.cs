using BlueOrb.Base.Item;

namespace BlueOrb.Controller.Inventory
{
    public interface IInventoryComponent
    {
        void AddItem(ItemDesc item);
        void RemoveItem(ItemDesc item);
        void StartListening();
        void StopListening();
    }
}