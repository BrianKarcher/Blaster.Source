using BlueOrb.Base.Attributes;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static BlueOrb.Controller.DollyCartComponent;

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

        void Awake()
        {
            Debug.Log("TrackSplit is awake");
        }

        private void OnTriggerEnter(Collider other)
        {
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
            MessageDispatcher.Instance.DispatchMsg("SetJoint", 0f, string.Empty, otherEntity.GetId(), _cartJoint.gameObject);

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
    }
}