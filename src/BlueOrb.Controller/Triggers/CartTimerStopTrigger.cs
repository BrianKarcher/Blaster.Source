using BlueOrb.Base.Attributes;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart Timer Stop Trigger")]
    public class CartTimerStopTrigger : MonoBehaviour
    {
        [SerializeField]
        [Tag]
        private string _tag;

        [SerializeField]
        private float time;

        private Collider thisCollider;
        private bool hasBegun = false;

        protected void Awake()
        {
            thisCollider = GetComponent<Collider>();
            thisCollider.enabled = false;
        }

        public void FixedUpdate()
        {
            if (hasBegun)
            {
                return;
            }
            if (GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                thisCollider.enabled = true;
                hasBegun = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeInHierarchy)
                return;
            Debug.Log($"Trigger entered: {other.name}");
            if (!other.CompareTag(_tag))
            {
                return;
            }
            Debug.Log($"Reparenting to {other.name}");

            var otherEntity = other.GetComponent<IEntity>();
            otherEntity = otherEntity ?? other.attachedRigidbody.GetComponent<IEntity>();
            if (otherEntity == null)
            {
                Debug.LogError($"TrackSplit could not find entity for {other.name}");
                return;
            }

            DollyCartJointComponent dollyCartJointComponent = otherEntity.Components.GetComponent<DollyCartJointComponent>();
            if (dollyCartJointComponent == null)
            {
                return;
            }

            dollyCartJointComponent.StopViaTime(this.time);
        }
    }
}