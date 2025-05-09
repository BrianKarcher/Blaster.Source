﻿using BlueOrb.Base.Attributes;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using UnityEngine;
using static BlueOrb.Controller.DollyCartJointComponent;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart Set Speed")]
    public class SetCartSpeedTrigger : MonoBehaviour
    {
        [SerializeField]
        [Tag]
        private string _tag;
        [SerializeField]
        private float _speed = 5;
        [SerializeField]
        private bool _immediate = false;
        [SerializeField]
        private float _smoothTime = 1;

        [SerializeField]
        private float deactivateTime = 3.0f;

        private Collider splitCollider;
        private bool hasLevelBegun = false;
        private bool isPaused = false;
        private float unPauseTime = 0;

        protected void Awake()
        {
            splitCollider = GetComponent<Collider>();
            splitCollider.enabled = false;
        }

        public void FixedUpdate()
        {
            if (hasLevelBegun)
            {
                return;
            }
            if (GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                splitCollider.enabled = true;
                hasLevelBegun = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (this.isPaused && Time.time < this.unPauseTime)
            {
                return;
            }
            if (!gameObject.activeInHierarchy)
                return;
            Debug.Log($"Trigger entered: {other.name}");
            if (!other.CompareTag(_tag))
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

            SetSpeedData data = new SetSpeedData
            {
                TargetSpeed = _speed,
                SmoothTime = _smoothTime,
                Immediate = _immediate
            };
            MessageDispatcher.Instance.DispatchMsg("SetSpeed", 0f, string.Empty, otherEntity.GetId(), data);

            this.isPaused = true;
            this.unPauseTime = Time.time + this.deactivateTime;
        }
    }
}