using BlueOrb.Base.Attributes;
using BlueOrb.Base.Extensions;
using BlueOrb.Common.Components;
using BlueOrb.Controller.DollyCart;
using BlueOrb.Messaging;
using BlueOrb.Physics;
using System.Collections.Generic;
using UnityEngine;

namespace BlueOrb.Controller
{
    [AddComponentMenu("BlueOrb/Components/Dolly Cart Joint")]
    public class DollyCartJointComponent : ComponentBase<DollyCartJointComponent>
    {
        private const string StopTimeMessage = "StopForTimer";

        private float _stopTime;
        public float StopTime => this._stopTime;
        //private bool _running;

        [SerializeField]
        public LerpType _speedChangeType = LerpType.SmoothDamp;
        [SerializeField]
        private IDollyCart dollyCart;
        [SerializeField]
        private bool autoRotate = false;
        [SerializeField]
        private bool zeroY = true;

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

        [SerializeField]
        private float enemyPushRadius = 1.0f;

        [SerializeField]
        private LayerMask enemyLayer;

        private Vector3 oldDollyCartPosition;
        //private Vector3 oldDollyCartRotation;
        public float yaw;
        public float pitch;
        public float roll;
        //public float oldyaw;
        //public float oldpitch;
        //public float oldroll;

        public float SmoothTime => this.dollyCart?.SmoothTime ?? 0f;
        public float TargetSpeed => this.dollyCart?.TargetSpeed ?? 0f;
        public void SetTargetSpeed(float speed) => this.dollyCart?.SetTargetSpeed(speed);

        [SerializeField]
        private IPhysicsComponent physicsComponent;

        private CharacterController characterController;

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

        public class SetJointData
        {
            public GameObject Joint { get; set; }
            public CartStartPosition CartStartPosition { get; set; }
        }

        public enum CartStartPosition
        {
            Reset = 0,
            ClosestToPlayer = 1
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
            if (physicsComponent == null)
            {
                physicsComponent = GetComponent<IPhysicsComponent>();
            }
            this.characterController = GetComponent<CharacterController>();
            //_running = false;
            pitch = _dollyJoint.transform.eulerAngles.x;
            yaw = _dollyJoint.transform.eulerAngles.y;
            roll = _dollyJoint.transform.eulerAngles.z;
            //ItemsByType = new Dictionary<ItemTypeEnum, ItemDesc>();
        }

        // Move other CharacterControllers the player collides with. This should get rid of the jitter that we get
        // when we run into an enemy and suddenly stop and then have to start back up again when the enemy dies.
        //void OnControllerColliderHit(ControllerColliderHit hit)
        //{

        //}

        public override void StartListening()
        {
            base.StartListening();
            _setSpeedTargetId = MessageDispatcher.Instance.StartListening("SetJoint", _componentRepository.GetId(), (data) =>
            {
                SetDollyCart((SetJointData)data.ExtraInfo);
            });

            _setSpeedId = MessageDispatcher.Instance.StartListening("SetSpeed", _componentRepository.GetId(), (data) =>
            {
                var speedData = data.ExtraInfo as SetSpeedData;
                Debug.Log($"Setting dolly target speed to {speedData.TargetSpeed}");
                if (this.dollyCart == null)
                {
                    Debug.LogError($"(DollyCartJointComponent SetSpeed, no Dolly Cart located");
                }
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

        //public void ProcessDollyCartSpeedChange() => this.dollyCart?.ProcessDollyCartSpeedChange();

        public void SetBrake(bool brake) => this.dollyCart?.SetBrake(brake);

        public void StopViaTime(float time)
        {
            Debug.Log($"StopViaTime called ({time})");
            this._stopTime = time;
            //Brake();
            MessageDispatcher.Instance.DispatchMsg(StopTimeMessage, 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
        }

        public void StartAcceleration(float speed, float time) => this.dollyCart.StartAcceleration(speed, time);

        private float correctiveTimer;
        [SerializeField]
        private bool isCorrecting = false;
        private Quaternion correctiveOriginalRotation;

        public void SetDollyCart(SetJointData setJointData)
        {
            Debug.Log("SetDollyCart called");
            if (this.dollyCart != null)
            {
                MessageDispatcher.Instance.DispatchMsg("DetachJoint", 0f, _componentRepository.GetId(), this.dollyCart.GetId(), null);
            }
            this.dollyCart = setJointData.Joint.GetComponent<IDollyCart>();
            if (this.dollyCart == null)
            {
                throw new System.Exception($"No dolly cart sent to set the joint to in {setJointData.Joint.name}.");
            }
            MessageDispatcher.Instance.DispatchMsg("AttachJoint", 0f, _componentRepository.GetId(), this.dollyCart.GetId(), null);
            if (setJointData.CartStartPosition == CartStartPosition.Reset)
            {
                this.dollyCart.ResetPosition();
            }
            else if (setJointData.CartStartPosition == CartStartPosition.ClosestToPlayer)
            {
                float pos = this.dollyCart.FindPositionClosestToPoint(this._dollyJoint.transform.position);
                Debug.Log($"Player position: {this._dollyJoint.transform.position}, placing dolly cart at pos {pos}");
                this.dollyCart.SetPosition(pos);
            }
            correctiveTimer = 0f;
            isCorrecting = true;
            correctiveOriginalRotation = this._dollyJoint.transform.rotation;
            // The old cart is disabled, new deltas will come from the new cart
            SetOldPositionAndRotation(this.dollyCart.GetWorldPosition());
        }

        private void SetOldPositionAndRotation(Vector3 pos)
        {
            this.oldDollyCartPosition = pos;
            //this.oldpitch = go.transform.localEulerAngles.x;
            //this.oldyaw = go.transform.localEulerAngles.y;
            //this.oldroll = go.transform.localEulerAngles.z;
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

        Collider[] pushColliderStore = new Collider[50];
        HashSet<int> enemiesChecked = new HashSet<int>();

        protected void FixedUpdate()
        {
            //if (_updatingSpeed)
            //{
            //    ProcessDollyCartSpeed();
            //}
            int count = UnityEngine.Physics.OverlapCapsuleNonAlloc(transform.position, transform.position + new Vector3(0f, this.characterController.height, 0f),
                this.enemyPushRadius, this.pushColliderStore, enemyLayer.value);
            this.enemiesChecked.Clear();
            for (int i = 0; i < count; i++)
            {
                Collider collider = this.pushColliderStore[i];
                CharacterController con = collider.attachedRigidbody?.GetComponent<CharacterController>();
                con = con ?? collider.GetComponent<CharacterController>();

                if (con == null) { continue; }
                //if (hit.moveDirection.y < -0.3F) { return; }
                if (this.enemiesChecked.Contains(con.GetInstanceID()))
                {
                    continue;
                }
                Vector3 pushDir = collider.transform.position - this.transform.position; //new Vector3(hit.point.x - this.transform.position.x, 0, hit.point.z - this.transform.position.z);
                                                                                         //con.Move(pushDir * Time.deltaTime * pushSpeed);
                //Debug.LogError($"Moving {con.name}");
                con.Move(pushDir.normalized * enemyPushRadius * Time.deltaTime);
                this.enemiesChecked.Add(con.GetInstanceID());
            }

            UpdatePositionAndRotation();
        }

        private void UpdatePositionAndRotation()
        {
            if (this.dollyCart == null)
            {
                return;
            }
            // Directly translate the position and rotation based on the delta for the Dolly Cart.
            Vector3 distanceDelta = this.dollyCart.GetWorldPosition() - this.oldDollyCartPosition;
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
            //_dollyJoint.transform.localPosition += distanceDelta;
            if (zeroY)
            {
                distanceDelta.y = 0;
            }
            this.physicsComponent.Move(distanceDelta / Time.deltaTime);
            // Corrective position
            //Vector3.SmoothDamp()

            //Vector3.MoveTowards()
            //_dollyJoint.transform.position.
            
            // Player got off track? Time to correct
            if (!isCorrecting && (PosOffTrack
                || RotOffTrack))
            {
                Debug.Log("Correcting the Joint!");
                this.isCorrecting = true;
                this.correctiveTimer = 0f;
                correctiveOriginalRotation = this._dollyJoint.transform.rotation;
            }

            //Short explanation: targetAngle - myAngle + 540 calculates targetAngle -myAngle + 180 and adds 360 to ensure it's a positive number,
            //since compilers can be finicky about % modulus with negative numbers. Then % 360 normalizes the difference to [0, 360).
            //And finally the - 180 subtracts the 180 added at the first step, and shifts the range to [-180, 180).
            if (this.isCorrecting)
            {
                this.correctiveTimer += ((float)1f / (float)5f) * Time.deltaTime;
                float smoothTimer = Mathf.SmoothStep(0f, 1f, this.correctiveTimer);
                Vector3 targetPos = Vector3.Lerp(_dollyJoint.transform.position, this.dollyCart.GetWorldPosition(), smoothTimer);
                Vector3 dir = targetPos - _dollyJoint.transform.position;
                dir.y = 0;
                this.physicsComponent.Move(dir / Time.deltaTime);
                //_dollyJoint.transform.position = 

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

                if (this.autoRotate)
                {
                    _dollyJoint.transform.rotation = Quaternion.Slerp(correctiveOriginalRotation, this.dollyCart.GetWorldRotation(), smoothTimer);
                }

                if (PosOnTrack && RotOnTrack)
                //if (_dollyJoint.transform.rotation - )
                {
                    this.isCorrecting = false;
                }
            }
            else
            {
                //Vector3 dir = this.dollyCart.GetWorldPosition() - _dollyJoint.transform.position;
                //this.physicsComponent.Move(dir / Time.deltaTime);
                //_dollyJoint.transform.position = this.dollyCart.GetWorldPosition();
                if (this.autoRotate)
                {
                    this._dollyJoint.transform.rotation = this.dollyCart.GetWorldRotation();
                }
            }

            //Quaternion.RotateTowards
            //_dollyJoint.transform.rotation.
            SetOldPositionAndRotation(this.dollyCart.GetWorldPosition());
        }

        private bool PosOffTrack => (_dollyJoint.transform.position.xz() - this.dollyCart.GetWorldPosition().xz()).magnitude > 0.01f;
        private bool PosOnTrack => (_dollyJoint.transform.position.xz() - this.dollyCart.GetWorldPosition().xz()).magnitude < 0.005f;

        private bool RotOffTrack => this.autoRotate && Mathf.Abs(Quaternion.Angle(_dollyJoint.transform.rotation, this.dollyCart.GetWorldRotation())) > 0.05f;
        private bool RotOnTrack => !this.autoRotate || Mathf.Abs(Quaternion.Angle(_dollyJoint.transform.rotation, this.dollyCart.GetWorldRotation())) < 0.005f;
    }
}