using BlueOrb.Base.Attributes;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using UnityEngine;
using static BlueOrb.Controller.DollyCartJointComponent;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart Track Split Trigger")]
    public class TrackSplit : MonoBehaviour
    {
        [SerializeField]
        [Tag]
        private string _tag;
        [SerializeField]
        private Transform _cartJoint;
        [SerializeField]
        private float _speed = 5;
        [SerializeField]
        private bool _immediate = false;
        [SerializeField]
        private float _smoothTime = 1;

        [SerializeField]
        private float deactivateTime = 3.0f;

        [SerializeField]
        private CartStartPosition cartStartPosition = CartStartPosition.Reset;

        private Collider splitCollider;
        private bool hasBegun = false;
        private bool isPaused = false;
        private float unPauseTime = 0;

        protected void Awake()
        {
            splitCollider = GetComponent<Collider>();
            splitCollider.enabled = false;
        }

        public void FixedUpdate()
        {
            if (hasBegun)
            {
                return;
            }
            if (GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                splitCollider.enabled = true;
                hasBegun = true;
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
            this.isPaused = true;
            this.unPauseTime = Time.time + this.deactivateTime;
        }
    }
}