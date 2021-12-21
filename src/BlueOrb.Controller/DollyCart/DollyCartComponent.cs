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
        private float _targetSpeed;
        [SerializeField]
        private float _startSpeed;
        [SerializeField]
        private float _currentSpeed;

        private long _setSpeedTargetId, _setSpeedId;
        private bool _updatingSpeed;
        private float _velocity;
        private float _smoothTime = 2f;

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

        public void SetDollyCart(GameObject dolly)
        {
            _componentRepository.transform.parent = dolly.transform;
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
                _smoothTime = speedData.SmoothTime;
                _targetSpeed = speedData.TargetSpeed;
            });

            _setSpeedId = MessageDispatcher.Instance.StartListening("SetSpeed", _componentRepository.GetId(), (data) =>
            {
                var speed = (float)data.ExtraInfo;
                _cinemachineDollyCart.m_Speed = speed;
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
                        break;
                    case LerpType.SmoothDamp:
                        _cinemachineDollyCart.m_Speed = UnityEngine.Mathf.SmoothDamp(_cinemachineDollyCart.m_Speed, _targetSpeed, ref _velocity, _smoothTime * Time.deltaTime);
                        break;
                }
                
            }
        }
    }
}
