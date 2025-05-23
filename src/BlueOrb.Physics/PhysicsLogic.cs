﻿using System;
using System.Collections.Generic;
using BlueOrb.Base.Extensions;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Physics
{
    /// <summary>
    /// Testable version of the Physics logic
    /// </summary>
    [Serializable]
    public class PhysicsLogic
    {
        // TODO: Remove this
        [SerializeField] private bool _isGrounded;
        [SerializeField]
        public Vector3 GroundNormal = new Vector3(0f, -1f, 0f);

        [SerializeField] private PhysicsData _physicsData;
        public PhysicsData OriginalPhysicsData { get; private set; }

        private IMovementController _movementController;
        public Vector3 BounceFromLocation { get; set; }
        public GameObject BounceFromObject { get; set; }

        public void Construct(PhysicsData physicsData)
        {
            _physicsData = physicsData;
        }

        public void Awake()
        {
            OriginalPhysicsData = new PhysicsData();
            OriginalPhysicsData.CopyFrom(_physicsData);
            // Cache distance squared for LOS
            _physicsData.LOSSquared = _physicsData.LineOfSight * _physicsData.LineOfSight;
        }

        public void SetMovementController(IMovementController movementController)
        {
            _movementController = movementController;
        }

        public PhysicsData GetPhysicsData()
        {
            return _physicsData;
        }

        public PhysicsData GetOriginalPhysicsData()
        {
            return OriginalPhysicsData;
        }

        public int GetClosestObject(IList<int> instanceIds)
        {
            int closestUniqueId = 0;
            float closestDistance = 10000000f;

            var thisPos = _movementController.GetWorldPos3();

            //foreach (var usableObject in uniqueIds)
            for (int i = 0; i < instanceIds.Count; i++)
            {
                var usableObject = instanceIds[i];
                //Vector2D usableObjectLocation = Vector2D.Zero();
                var entity = EntityContainer.Instance.GetEntity(usableObject.ToString());
                var physicsComponent = entity.Components.GetComponent<IPhysicsComponent>();
                var usableObjectLocation = physicsComponent == null ? entity.transform.position : physicsComponent.GetWorldPos3();
                //MessageDispatcher.Instance.DispatchMsg(0f,
                //    string.Empty, usableObject, RQ.Enums.Telegrams.GetPos, null,
                //    (location) => usableObjectLocation = (Vector2D)location);

                var distanceSq = Vector3.SqrMagnitude(thisPos - usableObjectLocation);
                if (distanceSq < closestDistance)
                {
                    // Found a closer object
                    closestDistance = distanceSq;
                    closestUniqueId = usableObject;
                }
            }

            return closestUniqueId;
        }

        public bool GetIsGrounded()
        {
            return _isGrounded;
        }

        public void SetIsGrounded(bool isGrounded)
        {
            _isGrounded = isGrounded;
        }

        public void CheckGroundStatus()
        {
            //#if UNITY_EDITOR
            //			// helper to visualise the ground check ray in the scene view
            Debug.DrawLine(_movementController.transform.position + new Vector3(0f, 0.2f, 0f), _movementController.transform.position + new Vector3(0f, 0.2f, 0f) + (Vector3.down * _physicsData.GroundCheckDistance), Color.blue);
            //Debug.
            //#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            //bool groundRaycast = UnityEngine.Physics.Raycast(_movementController.transform.position + new Vector3(0f, 0.2f, 0f), Vector3.down, out hitInfo, _physicsData.GroundCheckDistance, _physicsData.GroundLayer);
            //bool groundRaycast = UnityEngine.Physics.SphereCast(_movementController.transform.position + new Vector3(0f, 0.2f, 0f), .5f, Vector3.down, out hitInfo, _physicsData.GroundCheckDistance);
            Collider[] colliders = UnityEngine.Physics.OverlapSphere(_movementController.transform.position + new Vector3(0f, 0.2f, 0f), _physicsData.GroundCheckDistance, _physicsData.GroundLayer);
            bool isGrounded = colliders.Length != 0;
            if (_isGrounded != isGrounded)
            {
                Debug.Log($"{_movementController.GetComponentRepository().name} changed isGrounded to {isGrounded}");
            }
            _isGrounded = isGrounded;
            // The raycast and the Unity rigidbody sometimes disagree, so if either show ground or not moving vertically, assume entity is on the ground.
            //if (!groundRaycast && Mathf.Abs(_movementController.GetVelocity3().y) > 0.05f)
            //{
            //    _isGrounded = false;
            //    GroundNormal = Vector3.up;
            //    //m_Animator.applyRootMotion = false;
            //}
            //else
            //{
            //    GroundNormal = hitInfo.normal;
            //    _isGrounded = true;
            //    //m_Animator.applyRootMotion = true;
            //}
        }

        public bool IsFacingBigDrop(float distance)
        {
            var componentBase = (IComponentBase)_movementController;
            var pos = componentBase.GetComponentRepository().GetPosition() + (componentBase.transform.forward * distance);
            var dir = Vector3.down;
            bool raycast = UnityEngine.Physics.Raycast(pos, dir, 2.0f, _physicsData.TerrainLayer);
            if (_physicsData.ShowDebug)
            {
                Debug.DrawLine(pos, pos + (dir * 2.0f), raycast ? Color.cyan : Color.green, 1.0f);
            }
            return !raycast;
        }
    }
}
