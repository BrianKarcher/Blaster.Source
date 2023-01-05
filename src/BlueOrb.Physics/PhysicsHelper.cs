using BlueOrb.Common.Container;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Physics
{
    public static class PhysicsHelper
    {
        // Cache the function delegate. Must cache all delegates!
        //public Func<RaycastHit, RaycastHit, int> RaycastDistanceCompareDel;
        public static System.Comparison<RaycastHit> RaycastDistanceCompareDel;

        static PhysicsHelper()
        {
            RaycastDistanceCompareDel = RaycastDistanceCompare;
        }

        private static int RaycastDistanceCompare(RaycastHit lhs, RaycastHit rhs)
        {
            if (lhs.distance > rhs.distance)
                return 1;
            if (rhs.distance > lhs.distance)
                return -1;
            return 0;
        }

        public static bool PosInFOV2(Vector2 entityPos, Vector2 forward, Vector2 targetPos, float fov)
        {
            var vectorToTarget = targetPos - entityPos;
            var angle = Vector2.Angle(forward, vectorToTarget);
            return angle <= fov;
        }

        public static bool PosInFOV3(Vector3 entityPos, Vector3 forward, Vector3 targetPos, float fov)
        {
            var vectorToTarget = targetPos - entityPos;
            var angle = Vector3.Angle(forward, vectorToTarget);
            return angle <= fov;
        }

        public static bool CheckLOSDistance(GameObject entity, Vector3 targetPos, float losDistanceSquared)
        {
            // Using distance squared space because a square root is slow
            var distanceSq = (targetPos - entity.transform.position).sqrMagnitude;
            //Debug.Log($"(CheckLOS Distance: {distanceSq}");
            return distanceSq <= losDistanceSquared;
        }

        public static bool HasLineOfSight(Vector3 entity, Vector3 target, int obstacleLayerMask)
        {
            var currentPos = entity;
            //var layerMask = 1 << _obstacleLayerMask;

            bool hasLineOfSight = !UnityEngine.Physics.Raycast(currentPos, target - currentPos, (target - currentPos).magnitude, obstacleLayerMask);
            if (hasLineOfSight)
                Debug.DrawLine(currentPos, target, Color.blue);
            else
                Debug.DrawLine(currentPos, target, Color.red);
            return hasLineOfSight;
        }

        public static bool HasLineOfSight(GameObject entity, GameObject target, int obstacleLayerMask)
        {
            var targetEntity = target.GetComponent<IEntity>();
            if (targetEntity == null)
                return HasLineOfSight(entity.transform.position, target.transform.position, obstacleLayerMask);
            //if (targetEntity == null)
            //    return HasLineOfSight(targetEntity.GetHeadPosition(), obstacleLayerMask);
            //var otherPhysicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            //if (otherPhysicsComponent == null)
            //    return HasLineOfSight(target.transform.position, obstacleLayerMask);
            //var otherPos = otherPhysicsComponent.GetWorldPos3();
            var otherPos = targetEntity.GetHeadPosition();

            return HasLineOfSight(entity.transform.position, otherPos, obstacleLayerMask);
        }
    }
}