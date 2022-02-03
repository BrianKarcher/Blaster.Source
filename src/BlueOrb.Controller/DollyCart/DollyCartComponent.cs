using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void SetDollyCartParent(GameObject dolly)
        {
            _dollyJoint.transform.parent = dolly.transform;
            //_componentRepository.transform.parent = dolly.transform;
            _cinemachineDollyCart = dolly.GetComponent<CinemachineDollyCart>();
        }

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
                var cartGO = (GameObject)data.ExtraInfo;
                var cart = cartGO.GetComponent<Cinemachine.CinemachineDollyCart>();
                _cinemachineDollyCart = cart;
            });
            _setTrack = MessageDispatcher.Instance.StartListening("SetTrack", _componentRepository.GetId(), (data) =>
            {
                var trackGO = (GameObject)data.ExtraInfo;
                SetDollyCartParent(trackGO);
            });
            //_addItemId = MessageDispatcher.Instance.StartListening("AddItem", _componentRepository.GetId(), (data) =>
            //{
            //    var item = (ItemDesc)data.ExtraInfo;
            //    Debug.Log($"Adding item {item.ItemConfig.name}, qty {item.Qty}");
            //    //AddItem(item);
            //});
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("SetSpeedTarget", _componentRepository.GetId(), _setSpeedTargetId);
            MessageDispatcher.Instance.StopListening("SetSpeed", _componentRepository.GetId(), _setSpeedId);
            MessageDispatcher.Instance.StopListening("SetCineCart", _componentRepository.GetId(), _setCineCart);
            MessageDispatcher.Instance.StopListening("SetTrack", _componentRepository.GetId(), _setTrack);
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
