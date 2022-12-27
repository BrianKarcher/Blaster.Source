using BlueOrb.Base.Attributes;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using BlueOrb.Physics;
using UnityEngine;
using static BlueOrb.Controller.DollyCartJointComponent;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart Track Split Trigger")]
    public class TrackSplit : MonoBehaviour
    {
        public enum State
        {
            Enabled = 1,
            Disabled = 0
        }

        [SerializeField]
        private State StartState = State.Enabled;

        [SerializeField]
        [Tag]
        private string _tag;
        [SerializeField]
        private Transform _cartJoint;
        [SerializeField]
        private bool setSpeed = true;
        [SerializeField]
        private float _speed = 5;
        [SerializeField]
        private bool _immediate = false;
        [SerializeField]
        private float _smoothTime = 1;

        [SerializeField]
        private float deactivateTime = 3.0f;

        [SerializeField]
        private bool disableAfterTrigger = false;

        [SerializeField]
        private CartStartPosition cartStartPosition = CartStartPosition.Reset;

        private Collider splitCollider;
        private bool hasBegun = false;
        private bool isPaused = false;
        private float unPauseTime = 0;
        private bool firstRun = true;

        private bool disableTimerEnabled = false;
        /// <summary>
        /// When the time is up, the collider will become disabled.
        /// </summary>
        private float disableTime;

        protected void Awake()
        {
            splitCollider = GetComponent<Collider>();
            splitCollider.enabled = false;
        }

        public void FixedUpdate()
        {
            if (this.disableTimerEnabled && Time.time > this.disableTime)
            {
                EnableCollider(false);
                this.disableTimerEnabled = false;
            }
            if (hasBegun)
            {
                return;
            }
            if (!GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                return;
            }
            if (firstRun)
            {
                if (StartState == State.Enabled)
                {
                    splitCollider.enabled = true;
                }
                else
                {
                    splitCollider.enabled = false;
                }
                hasBegun = true;
                firstRun = false;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (this.isPaused && Time.time < this.unPauseTime)
            {
                return;
            }
            if (!gameObject.activeInHierarchy)
            {
                Debug.Log($"TrackSplit Trigger Enter aborting because this game object is not active. Other collider {other.name}");
                return;
            }
            Debug.Log($"Trigger entered: {other.name}");
            if (!other.CompareTag(_tag))
            {
                return;
            }
            Debug.Log($"Reparenting to {other.name}");
            //var worldRotation = other.transform.rotation;
            //other.transform.parent = _cartJoint;
            var otherEntity = other.GetComponent<IEntity>();
            otherEntity = otherEntity ?? other.attachedRigidbody.GetComponent<IEntity>();
            if (otherEntity == null)
            {
                Debug.LogError($"TrackSplit could not find entity for {other.name}");
                return;
            }

            // Get the speed of the other entity
            DollyCartJointComponent dollyComponent = otherEntity.Components.GetComponent<DollyCartJointComponent>();
            float currentTargetSpeed = dollyComponent.TargetSpeed;

            //MessageDispatcher.Instance.DispatchMsg("SetTrack", 0f, string.Empty, otherEntity.GetId(), _cartJoint.gameObject);
            //var cart = otherEntity.Components.GetComponent<DollyCartComponent>();
            //cart.

            //var cart = _cartJoint.GetComponent<Cinemachine.CinemachineDollyCart>();
            SetJointData setJointData = new SetJointData()
            {
                CartStartPosition = cartStartPosition,
                Joint = _cartJoint.gameObject
            };

            MessageDispatcher.Instance.DispatchMsg("SetJoint", 0f, string.Empty, otherEntity.GetId(), setJointData);

            if (this.setSpeed)
            {
                SetSpeedData data = new SetSpeedData
                {
                    TargetSpeed = _speed,
                    SmoothTime = _smoothTime,
                    Immediate = _immediate
                };
                MessageDispatcher.Instance.DispatchMsg("SetSpeed", 0f, string.Empty, otherEntity.GetId(), data);
                //_cartJoint.gameObject.SetActive(true);
                //var dollyCart = _cartJoint.GetComponent<Cinemachine.CinemachineDollyCart>();
                //dollyCart.m_Speed = _speed;
                //other.transform.rotation = worldRotation;
            }
            else
            {
                Debug.Log($"Setting cart target speed to {currentTargetSpeed}");
                SetSpeedData data = new SetSpeedData
                {
                    TargetSpeed = currentTargetSpeed,
                    SmoothTime = _smoothTime,
                    Immediate = true
                };
                MessageDispatcher.Instance.DispatchMsg("SetSpeed", 0f, string.Empty, otherEntity.GetId(), data);
            }

            this.isPaused = true;
            this.unPauseTime = Time.time + this.deactivateTime;
            if (disableAfterTrigger)
            {
                splitCollider.enabled = false;
                //this.gameObject.SetActive(false);
            }
        }

        public void EnableCollider(bool enabled)
        {
            splitCollider.enabled = enabled;
        }

        public void SetDisabledTimer()
        {
            this.disableTimerEnabled = true;
            this.disableTime = Time.time + this.deactivateTime;
        }
    }
}