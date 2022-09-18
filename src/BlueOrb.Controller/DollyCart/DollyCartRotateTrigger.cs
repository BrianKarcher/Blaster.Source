using BlueOrb.Base.Attributes;
using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Controller.DollyCart
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart Rotate Trigger")]
    public class DollyCartRotateTrigger : MonoBehaviour
    {
        [Tag]
        [SerializeField]
        private string tag;

        [SerializeField]
        private Vector3 rotateTo;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(this.tag))
            {
                IEntity otherEntity = other.attachedRigidbody?.GetComponent<IEntity>();
                if (otherEntity == null)
                {
                    return;
                }
                var dollyCartComponent = otherEntity.Components.GetComponent<DollyCartJointComponent>();
                if (dollyCartComponent == null)
                {
                    return;
                }
                //dollyCartComponent.RotateTo(this.rotateTo, 2f);
            }
        }
    }
}
