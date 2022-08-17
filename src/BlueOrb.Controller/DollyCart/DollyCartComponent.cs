using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using Cinemachine;
using UnityEngine;

namespace BlueOrb.Controller
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart")]
    public class DollyCartComponent : ComponentBase<DollyCartComponent>
    {
        private float _targetTime;
        private bool _running;
        [SerializeField]
        private CinemachineDollyCart _cinemachineDollyCart;
        [SerializeField]
        public LerpType _speedChangeType;
        [SerializeField]
        private GameObject _dollyJoint;

        [SerializeField]
        private float _targetSpeed;
        [SerializeField]
        private float _startSpeed;
        [SerializeField]
        private float _currentSpeed;

        private long _setSpeedTargetId, _setSpeedId, _setCineCart, _setTrack;
        [SerializeField]
        private bool _updatingSpeed;
        private float _velocity;
        [SerializeField]
        private float _smoothTime = 2f;
        [SerializeField]
        private bool _speedDecreasing = false;

        private GameObject cinemachineDollyCartGameObject;
        private Vector3 oldDollyCartPosition;
        //private Vector3 oldDollyCartRotation;
        public float yaw;
        public float pitch;
        public float roll;
        public float oldyaw;
        public float oldpitch;
        public float oldroll;

        public enum LerpType
        {
            Lerp = 0,
            Slerp = 1,
            SmoothDamp = 2,
            InverseLerp = 3
        }

        public class SetSpeedData
        {
            public float SmoothTime = 2f;
            public float TargetSpeed;
        }

        //public void SetDollyCartParent(GameObject dolly)
        //{
        //    _dollyJoint.transform.parent = dolly.transform;
        //    //_cinemachineDollyCart.
        //    //_componentRepository.transform.parent = dolly.transform;
        //    _cinemachineDollyCart = dolly.GetComponent<CinemachineDollyCart>();
        //}

        public float GetSpeed()
        {
            return _cinemachineDollyCart.m_Speed;
        }

        //[SerializeField]
        //private InventoryData _inventoryData;

        protected override void Awake()
        {
            base.Awake();
            _running = false;

            //ItemsByType = new Dictionary<ItemTypeEnum, ItemDesc>();
        }

        private void LateUpdate()
        {
            // Directly translate the position and rotation based on the delta for the Dolly Cart.
            Vector3 distanceDelta = this._dollyJoint.transform.position - this.oldDollyCartPosition;
            //Quaternion rotationDelta = this.oldDollyCartRotation * Quaternion.Inverse(this.oldDollyCartRotation);
            //Quaternion rotationDelta = this._dollyJoint.transform.rotation.to - this.oldDollyCartRotation;
            //Vector3 forwardDelta = this._dollyJoint.transform.forward - this.oldDollyCartRotation;
            //Quaternion newQ = rotationDelta + this._dollyJoint.transform.rotation;
            //_dollyJoint.transform.Translate()
            //Mathf.MoveTowardsAngle()

            //Short explanation: targetAngle - myAngle + 540 calculates targetAngle -myAngle + 180 and adds 360 to ensure it's a positive number,
            //since compilers can be finicky about % modulus with negative numbers. Then % 360 normalizes the difference to [0, 360).
            //And finally the - 180 subtracts the 180 added at the first step, and shifts the range to [-180, 180).
            float deltaYaw = (this.cinemachineDollyCartGameObject.transform.eulerAngles.y - this.oldyaw + 540) % 360 - 180;
            yaw += deltaYaw;

            SetOldPositionAndRotation(this.cinemachineDollyCartGameObject.gameObject);
        }

        public override void StartListening()
        {
            base.StartListening();
            _setSpeedTargetId = MessageDispatcher.Instance.StartListening("SetSpeedTarget", _componentRepository.GetId(), (data) =>
            {
                _updatingSpeed = true;
                var speedData = data.ExtraInfo as SetSpeedData;
                _startSpeed = _cinemachineDollyCart.m_Speed;
                _currentSpeed = _startSpeed;
                _smoothTime = speedData.SmoothTime;
                _targetSpeed = speedData.TargetSpeed;
                if (_startSpeed < _targetSpeed)
                {
                    _speedDecreasing = false;
                }
                else
                {
                    _speedDecreasing = true;
                }
            });

            _setSpeedId = MessageDispatcher.Instance.StartListening("SetSpeed", _componentRepository.GetId(), (data) =>
            {
                var speed = (float)data.ExtraInfo;
                _startSpeed = speed;
                _currentSpeed = speed;
                _cinemachineDollyCart.m_Speed = speed;
            });

            _setCineCart = MessageDispatcher.Instance.StartListening("SetCineCart", _componentRepository.GetId(), (data) =>
            {
                this.cinemachineDollyCartGameObject = (GameObject)data.ExtraInfo;
                var cart = this.cinemachineDollyCartGameObject.GetComponent<CinemachineDollyCart>();
                _cinemachineDollyCart.m_Position = 0;
                _cinemachineDollyCart = cart;
                // The old cart is disabled, new deltas will come from the new cart
                SetOldPositionAndRotation(this.cinemachineDollyCartGameObject);
            });
            _setTrack = MessageDispatcher.Instance.StartListening("SetTrack", _componentRepository.GetId(), (data) =>
            {
                //var trackGO = (GameObject)data.ExtraInfo;
                //SetDollyCartParent(trackGO);
            });
            //_addItemId = MessageDispatcher.Instance.StartListening("AddItem", _componentRepository.GetId(), (data) =>
            //{
            //    var item = (ItemDesc)data.ExtraInfo;
            //    Debug.Log($"Adding item {item.ItemConfig.name}, qty {item.Qty}");
            //    //AddItem(item);
            //});
        }

        private void SetOldPositionAndRotation(GameObject go)
        {
            this.oldDollyCartPosition = go.transform.position;
            this.oldpitch = go.transform.localRotation.eulerAngles.x;
            this.oldyaw = go.transform.localRotation.eulerAngles.y;
            this.oldroll = go.transform.localRotation.eulerAngles.z;
            //this.oldDollyCartRotation = go.transform.forward;
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("SetSpeedTarget", _componentRepository.GetId(), _setSpeedTargetId);
            MessageDispatcher.Instance.StopListening("SetSpeed", _componentRepository.GetId(), _setSpeedId);
            MessageDispatcher.Instance.StopListening("SetCineCart", _componentRepository.GetId(), _setCineCart);
            MessageDispatcher.Instance.StopListening("SetTrack", _componentRepository.GetId(), _setTrack);
        }

        public void RotateTo(Vector3 dir, float time)
        {
            Debug.Log($"Rotating to {dir}");
            this._cinemachineDollyCart.m_Speed = 0f;
            gameObject.transform.rotation = Quaternion.Euler(dir.x, dir.y, dir.z);
            this._cinemachineDollyCart.m_Speed = 5f;
            //iTween.RotateTo(gameObject, dir, time);
        }

        protected void FixedUpdate()
        {
            if (_updatingSpeed)
            {
                switch (_speedChangeType)
                {
                    case LerpType.Lerp:
                        _cinemachineDollyCart.m_Speed = Mathf.Lerp(_cinemachineDollyCart.m_Speed, _targetSpeed, 1f / _smoothTime * Time.deltaTime);
                        //_cinemachineDollyCart.m_Speed = Mathf.Lerp(_startSpeed, _targetSpeed, 1f / _smoothTime * Time.deltaTime);
                        //_cinemachineDollyCart.m_Speed = Mathf.Lerp(_cinemachineDollyCart.m_Speed, _targetSpeed, _smoothTime * Time.deltaTime);

                        _currentSpeed = _cinemachineDollyCart.m_Speed;
                        break;
                    case LerpType.SmoothDamp:
                        _cinemachineDollyCart.m_Speed = UnityEngine.Mathf.SmoothDamp(_currentSpeed, _targetSpeed, ref _velocity, _smoothTime);
                        _currentSpeed = _cinemachineDollyCart.m_Speed;
                        break;
                }
                if (Mathf.Approximately(_currentSpeed, _targetSpeed) || (_speedDecreasing && _currentSpeed <= _targetSpeed + 0.01f))
                {
                    _currentSpeed = _targetSpeed;
                    _updatingSpeed = false;
                }
                else if (Mathf.Approximately(_currentSpeed, _targetSpeed) || (!_speedDecreasing && _currentSpeed >= _targetSpeed - 0.01f))
                {
                    _currentSpeed = _targetSpeed;
                    _updatingSpeed = false;
                }
            }
        }
    }
}