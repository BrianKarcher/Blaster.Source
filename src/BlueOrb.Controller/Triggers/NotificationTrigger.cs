using BlueOrb.Base.Attributes;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Notification Trigger")]
    public class NotificationTrigger : MonoBehaviour
    {
        [SerializeField]
        private string message;

        [SerializeField]
        [Tag]
        private string tag;

        [SerializeField]
        private float time;

        [SerializeField]
        private AudioClip audioClip;

        [SerializeField]
        private AudioSource audioSource;

        private Collider thisCollider;
        private bool hasBegun = false;

        protected void Awake()
        {
            thisCollider = GetComponent<Collider>();
            thisCollider.enabled = false;
            if (this.audioSource == null)
            {
                this.audioSource = GetComponent<AudioSource>();
            }
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
            if (!other.CompareTag(tag))
            {
                return;
            }
            Debug.Log($"Trigger: {other.name}");

            var otherEntity = other.GetComponent<IEntity>();
            otherEntity = otherEntity ?? other.attachedRigidbody.GetComponent<IEntity>();
            if (otherEntity == null)
            {
                Debug.LogError($"Could not find entity for {other.name}");
                return;
            }
            if (this.audioClip != null)
            {
                this.audioSource.PlayOneShot(this.audioClip);
            }
            GameStateController.Instance.UIController.HudController.CreateNotification(this.message);
        }
    }
}