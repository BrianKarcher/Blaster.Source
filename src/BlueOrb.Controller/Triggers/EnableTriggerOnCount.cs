using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Enable Trigger on Count")]
    public class EnableTriggerOnCount : ComponentBase<EnableTriggerOnCount>
    {
        [SerializeField]
        private int maxCount;
        [SerializeField]
        private string countMessage = "Count";
        [SerializeField]
        private string resetMessage = "Reset";

        private int currentCount;
        private long countMessageId, resetMessageId;
        private Collider splitCollider;
        private bool firstUpdate = true;

        protected override void Awake()
        {
            base.Awake();
            var colliders = GetComponents<Collider>();
            splitCollider = GetComponent<Collider>();
        }

        public void Start()
        {
            Reset();
        }

        public void Update()
        {
            if (this.firstUpdate)
            {
                this.firstUpdate = false;
                splitCollider.enabled = false;
            }
        }

        public override void StartListening()
        {
            base.StartListening();
            countMessageId = MessageDispatcher.Instance.StartListening(countMessage, _componentRepository.GetId(), (data) =>
            {
                this.currentCount++;
                if (this.currentCount >= maxCount)
                {
                    EnableCollider(true);
                }
            });
            resetMessageId = MessageDispatcher.Instance.StartListening(resetMessage, _componentRepository.GetId(), (data) =>
            {
                Reset();
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(countMessage, _componentRepository.GetId(), countMessageId);
            MessageDispatcher.Instance.StopListening(resetMessage, _componentRepository.GetId(), resetMessageId);
        }

        private void Reset()
        {
            this.currentCount = 0;
            EnableCollider(false);
        }

        public void EnableCollider(bool enabled)
        {
            splitCollider.enabled = enabled;
        }
    }
}