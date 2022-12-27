using BlueOrb.Base.Attributes;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Enable Track Split Trigger")]
    public class EnableTrackSplitOnShotTrigger : ComponentBase<EnableTrackSplitOnShotTrigger>
    {
        [SerializeField]
        private string message = "ProjectileHit";

        [SerializeField]
        private TrackSplit[] trackSplits;

        [SerializeField]
        private bool SetDisableTimer = false;

        //[SerializeField]
        //private float time;

        //private Collider thisCollider;
        //private bool hasBegun = false;

        //protected void Awake()
        //{
        //    thisCollider = GetComponent<Collider>();
        //    thisCollider.enabled = false;
        //}

        public override void StartListening()
        {
            base.StartListening();
            MessageDispatcher.Instance.StartListening(message, _componentRepository.GetId(), (data) =>
            {
                for (int i = 0; i < trackSplits.Length; i++)
                {
                    TrackSplit trackSplit = trackSplits[i];
                    trackSplit.EnableCollider(true);
                    if (this.SetDisableTimer)
                    {
                        trackSplit.SetDisabledTimer();
                    }
                }
            });
        }

        //public void FixedUpdate()
        //{
        //    if (hasBegun)
        //    {
        //        return;
        //    }
        //    if (GameStateController.Instance.LevelStateController.HasLevelBegun)
        //    {
        //        thisCollider.enabled = true;
        //        hasBegun = true;
        //    }
        //}

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (!gameObject.activeInHierarchy)
        //        return;
        //    Debug.Log($"Trigger entered: {other.name}");
        //    if (!other.CompareTag(tag))
        //    {
        //        return;
        //    }
        //    Debug.Log($"Trigger: {other.name}");

        //    var otherEntity = other.GetComponent<IEntity>();
        //    otherEntity = otherEntity ?? other.attachedRigidbody.GetComponent<IEntity>();
        //    if (otherEntity == null)
        //    {
        //        Debug.LogError($"Could not find entity for {other.name}");
        //        return;
        //    }

        //    GameStateController.Instance.UIController.HudController.CreateAlert(this.message);
        //}
    }
}