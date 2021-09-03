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

        public override void StartListening()
        {
            base.StartListening();

            MessageDispatcher.Instance.StartListening("SetSecondaryProjectile", _componentRepository.GetId(), (data) =>
            {
                Debug.Log($"(EntityItem) Setting Secondary Projectile to {_item?.Name}");
                MessageDispatcher.Instance.DispatchMsg("SetSecondaryProjectile", 0f, _componentRepository.GetId(), "Shooter Controller", _item);
            });
            
        }
    }
}
