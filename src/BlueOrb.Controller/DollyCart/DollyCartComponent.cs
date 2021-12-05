using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Inventory
{
    [AddComponentMenu("RQ/Components")]
    public class DollyCartComponent : ComponentBase<DollyCartComponent>
    {
        private float _targetTime;
        private bool _running;
        [SerializeField]
        private CinemachineDollyCart _cinemachineDollyCart;
        [SerializeField]
        public LerpType _speedChangeType;

        private float _targetSpeed;
        private float _startSpeed;
        private float _currentSpeed;

        private long _setSpeedMessageId;
        private bool _updatingSpeed;
        private float _velocity;
        private float _smoothTime = 2f;

        public enum LerpType
        {
            Lerp = 0,
            Slerp = 1,
            SmoothDamp = 2
        }

        public class SetSpeedData
        {
            public float SmoothTime = 2f;
            public float TargetSpeed;
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
            _setSpeedMessageId = MessageDispatcher.Instance.StartListening("SetSpeedTarget", _componentRepository.GetId(), (data) =>
            {
                _updatingSpeed = true;
                var speedData = data.ExtraInfo as SetSpeedData;
                _smoothTime = speedData.SmoothTime;
                _targetSpeed = speedData.TargetSpeed;
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
            MessageDispatcher.Instance.StopListening("SetSpeedTarget", _componentRepository.GetId(), _setSpeedMessageId);
        }

        protected void FixedUpdate()
        {
            if (_updatingSpeed)
            {
                switch (_speedChangeType)
                {
                    case LerpType.Lerp:
                        _cinemachineDollyCart.m_Speed = Mathf.Lerp(_startSpeed, _targetSpeed, 0.5f);
                        break;
                    case LerpType.SmoothDamp:
                        _cinemachineDollyCart.m_Speed = UnityEngine.Mathf.SmoothDamp(_cinemachineDollyCart.m_Speed, _targetSpeed, ref _velocity, 2f);
                        break;
                }
                
            }
        }

        //public void AddItem(ItemDesc item)
        //{
        //    var itemConfigAndCount = _inventoryData.Items.FirstOrDefault(i => i.ItemConfig == item.ItemConfig);
        //    // Prevent duplication of Items, just adjust the Qty if the item is already in the inventory
        //    //ItemDesc itemToUpdate = null;
        //    if (itemConfigAndCount == null)
        //    {
        //        itemConfigAndCount = item.Clone();
        //        _inventoryData.Add(itemConfigAndCount);
        //        //Items.Add(itemConfigAndCount);
        //    }
        //    else
        //    {
        //        //itemConfigAndCount = itemConfigAndCount;
        //        itemConfigAndCount.Qty += item.Qty;
        //    }
            
        //    MessageDispatcher.Instance.DispatchMsg("ItemAdded", 0, null, _componentRepository.GetId(), itemConfigAndCount);
        //}

        //public void ToggleMold(bool isAsc)
        //{

        //}
    }
}
