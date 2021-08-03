using BlueOrb.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
            other.transform.parent = _cartJoint;
            //_cartJoint.gameObject.SetActive(true);
            var dollyCart = _cartJoint.GetComponent<Cinemachine.CinemachineDollyCart>();
            dollyCart.m_Speed = _speed;
            //other.transform.rotation = worldRotation;
        }
    }
}
