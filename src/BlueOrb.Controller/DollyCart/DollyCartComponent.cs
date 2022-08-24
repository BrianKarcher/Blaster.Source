using BlueOrb.Base.Attributes;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using Cinemachine;
using UnityEngine;

namespace BlueOrb.Controller
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart Joint")]
    public class DollyCartComponent : ComponentBase<DollyCartComponent>
    {
        private float _targetTime;
        private bool _running;

        [SerializeField]
        public LerpType _speedChangeType = LerpType.SmoothDamp;
        [SerializeField]
        private DollyCart.DollyCart dollyCart;

        public bool HasCart => this.dollyCart != null;

        [SerializeField]
        private GameObject _dollyJoint;

        private long _setSpeedTargetId, _setSpeedId, _setCineCart, _setTrack;

        [SerializeField]
        [Tag]
        private string enemyTag;
        public string EnemyTag => this.enemyTag;

        public float Speed => this.dollyCart?.Speed ?? 0f;

        public void Stop() => this.dollyCart?.Stop();

        //[SerializeField]
        ////[Layer]
        //private int[] EnemyLayer;

        [SerializeField]
        private Vector3 enemyCheckOffset = new Vector3(0f, 0f, 2f);
        public Vector3 EnemyCheckOffset => this.enemyCheckOffset;

        [SerializeField]
        private Vector3 enemyCheckHalfExtents = new Vector3(0.5f, 1f, 1f);
        public Vector3 EnemyCheckHalfExtents => this.enemyCheckHalfExtents;

        private Vector3 oldDollyCartPosition;
        //private Vector3 oldDollyCartRotation;
        public float yaw;
        public float pitch;
        public float roll;
        public float oldyaw;
        public float oldpitch;
        public float oldroll;

        public float SmoothTime => this.dollyCart?.SmoothTime ?? 0f;
        public float TargetSpeed => this.dollyCart?.TargetSpeed ?? 0f;
        public void SetTargetSpeed(float speed) => this.dollyCart?.SetTargetSpeed(speed);

        public enum LerpType
        {
            Lerp = 0,
            Slerp = 1,
            SmoothDamp = 2,
            InverseLerp = 3,
            SmoothStep = 4
        }

        public class SetSpeedData
        {
            public float SmoothTime = 2f;
            public float TargetSpeed;
            public bool Immediate;
        }

        //public void SetDollyCartParent(GameObject dolly)
        //{
        //    _dollyJoint.transform.parent = dolly.transform;
        //    //_cinemachineDollyCart.
        //    //_componentRepository.transform.parent = dolly.transform;
        //    _cinemachineDollyCart = dolly.GetComponent<CinemachineDollyCart>();
        //}

        //[SerializeField]
        //private InventoryData _inventoryData;

        protected override void Awake()
        {
            base.Awake();
            _running = false;
            pitch = _dollyJoint.transform.eulerAngles.x;
            yaw = _dollyJoint.transform.eulerAngles.y;
            roll = _dollyJoint.transform.eulerAngles.z;
            //ItemsByType = new Dictionary<ItemTypeEnum, ItemDesc>();
        }

        public override void StartListening()
        {
            base.StartListening();
            _setSpeedTargetId = MessageDispatcher.Instance.StartListening("SetJoint", _componentRepository.GetId(), (data) =>
            {
                SetDollyCart((GameObject)data.ExtraInfo);
            });

            _setSpeedId = MessageDispatcher.Instance.StartListening("SetSpeed", _componentRepository.GetId(), (data) =>
            {
                var speedData = data.ExtraInfo as SetSpeedData;
                this.dollyCart?.StartAcceleration(speedData.TargetSpeed, speedData.SmoothTime, speedData.Immediate);
            });

            //_setCineCart = MessageDispatcher.Instance.StartListening("SetCineCart", _componentRepository.GetId(), (data) =>
            //{
            //    SetDollyCart((GameObject)data.ExtraInfo);
            //});
            //_setTrack = MessageDispatcher.Instance.StartListening("SetTrack", _componentRepository.GetId(), (data) =>
            //{
            //    //var trackGO = (GameObject)data.ExtraInfo;
            //    //SetDollyCartParent(trackGO);
            //});
            //_addItemId = MessageDispatcher.Instance.StartListening("AddItem", _componentRepository.GetId(), (data) =>
            //{
            //    var item = (ItemDesc)data.ExtraInfo;
            //    Debug.Log($"Adding item {item.ItemConfig.name}, qty {item.Qty}");
            //    //AddItem(item);
            //});
        }

        public void ProcessDollyCartSpeedChange() => this.dollyCart?.ProcessDollyCartSpeedChange();

        public void Brake() => this.dollyCart?.Brake();

        public void StartAcceleration(float speed, float time) => this.dollyCart.StartAcceleration(speed, time);

        private float correctiveTimer;
        private bool isCorrecting = false;
        private Quaternion correctiveOriginalRotation;
        public void SetDollyCart(GameObject dollyCart, bool resetCartPosition = true)
        {
            this.dollyCart = dollyCart.GetComponent<DollyCart.DollyCart>();
            if (resetCartPosition)
            {
                this.dollyCart.Reset();
            }
            correctiveTimer = 0f;
            isCorrecting = true;
            correctiveOriginalRotation = this._dollyJoint.transform.rotation;
            // The old cart is disabled, new deltas will come from the new cart
            SetOldPositionAndRotation(this.dollyCart.gameObject);
        }

        private void SetOldPositionAndRotation(GameObject go)
        {
            this.oldDollyCartPosition = go.transform.position;
            this.oldpitch = go.transform.localEulerAngles.x;
            this.oldyaw = go.transform.localEulerAngles.y;
            this.oldroll = go.transform.localEulerAngles.z;
            //this.oldDollyCartRotation = go.transform.forward;
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("SetJoint", _componentRepository.GetId(), _setSpeedTargetId);
            MessageDispatcher.Instance.StopListening("SetSpeed", _componentRepository.GetId(), _setSpeedId);
            //MessageDispatcher.Instance.StopListening("SetCineCart", _componentRepository.GetId(), _setCineCart);
            //MessageDispatcher.Instance.StopListening("SetTrack", _componentRepository.GetId(), _setTrack);
        }

        //public void RotateTo(Vector3 dir, float time)
        //{
        //    Debug.Log($"Rotating to {dir}");
        //    SetSpeed(0f);
        //    gameObject.transform.rotation = Quaternion.Euler(dir.x, dir.y, dir.z);
        //    SetSpeed(5f);
        //    //iTween.RotateTo(gameObject, dir, time);
        //}

        protected void FixedUpdate()
        {
            //if (_updatingSpeed)
            //{
            //    ProcessDollyCartSpeed();
            //}

            UpdatePositionAndRotation();
        }

        private void UpdatePositionAndRotation()
        {
            if (this.dollyCart == null)
            {
                return;
            }
            // Directly translate the position and rotation based on the delta for the Dolly Cart.
            Vector3 distanceDelta = this.dollyCart.transform.position - this.oldDollyCartPosition;
            //Quaternion rotationDelta = this.oldDollyCartRotation * Quaternion.Inverse(this.oldDollyCartRotation);
            //Quaternion rotationDelta = this._dollyJoint.transform.rotation.to - this.oldDollyCartRotation;
            //Vector3 forwardDelta = this._dollyJoint.transform.forward - this.oldDollyCartRotation;
            //Quaternion newQ = rotationDelta + this._dollyJoint.transform.rotation;
            //_dollyJoint.transform.Translate()
            //Mathf.MoveTowardsAngle()

            //if (distanceDelta.sqrMagnitude < Mathf.)
            //{
            //    return;
            //}
            _dollyJoint.transform.localPosition += distanceDelta;
            // Corrective position
            //Vector3.SmoothDamp()

            //Vector3.MoveTowards()
            //_dollyJoint.transform.position.

            //Short explanation: targetAngle - myAngle + 540 calculates targetAngle -myAngle + 180 and adds 360 to ensure it's a positive number,
            //since compilers can be finicky about % modulus with negative numbers. Then % 360 normalizes the difference to [0, 360).
            //And finally the - 180 subtracts the 180 added at the first step, and shifts the range to [-180, 180).
            if (this.isCorrecting)
            {
                this.correctiveTimer += ((float)1f / (float)5f) * Time.deltaTime;
                float smoothTimer = Mathf.SmoothStep(0f, 1f, this.correctiveTimer);
                _dollyJoint.transform.position = Vector3.Lerp(_dollyJoint.transform.position, this.dollyCart.transform.position, smoothTimer);

                //float deltaYaw = (this.dollyCart.transform.localEulerAngles.y - this.oldyaw + 540) % 360 - 180;
                //yaw += deltaYaw;
                //float deltaPitch = (this.dollyCart.transform.localEulerAngles.x - this.oldpitch + 540) % 360 - 180;
                //pitch += deltaPitch;
                //// Corrective rotation (errors occur when a cart switches from one joint to another, this corrects that)
                //yaw = Mathf.LerpAngle(yaw, this.dollyCart.transform.eulerAngles.y, 2f);
                ////Mathf.Smooth
                //pitch = Mathf.LerpAngle(pitch, this.dollyCart.transform.eulerAngles.x, 2f);
                //_dollyJoint.transform.eulerAngles = new Vector3(pitch, yaw, 0);
                //_dollyJoint.transform.rotation = Quaternion.Slerp(correctiveOriginalRotation, this.dollyCart.transform.rotation, Time.deltaTime);
                _dollyJoint.transform.rotation = Quaternion.Slerp(correctiveOriginalRotation, this.dollyCart.transform.rotation, smoothTimer);
                if (this.correctiveTimer >= 1f)
                //if (_dollyJoint.transform.rotation - )
                {
                    this.isCorrecting = false;
                }
            }
            else
            {
                _dollyJoint.transform.position = this.dollyCart.transform.position;
                this._dollyJoint.transform.rotation = this.dollyCart.transform.rotation;
            }

            //Quaternion.RotateTowards
            //_dollyJoint.transform.rotation.
            SetOldPositionAndRotation(this.dollyCart.gameObject);
        }
    }
}