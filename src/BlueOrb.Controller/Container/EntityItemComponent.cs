using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Block
{
    [AddComponentMenu("RQ/Components/Entity Item")]
    public class EntityItemComponent : ComponentBase<EntityItemComponent>
    {
        [SerializeField]
        private ItemConfig _item;
        [SerializeField]
        private string ammoBoxShotMessage = "AmmoBoxShot";

        public override void StartListening()
        {
            base.StartListening();

            MessageDispatcher.Instance.StartListening("AcquireItem", _componentRepository.GetId(), (data) =>
            {
                Debug.Log($"(EntityItem) Shot Ammo Box {_item?.Name}");
                MessageDispatcher.Instance.DispatchMsg(this.ammoBoxShotMessage, 0f, _componentRepository.GetId(), "Shooter Controller", _item);
            });
        }
    }
}