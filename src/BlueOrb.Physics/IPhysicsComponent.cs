using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Physics
{
    public interface IPhysicsComponent : IComponentBase
    {
        PhysicsLogic Controller { get; }
        bool AutoApplyToAnimator { get; set; }

        void AddForce(Vector3 force);
        void AddForce2(Vector2 force);
        void Explode(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier);
        bool GetEnabled();
        PhysicsData GetPhysicsData();
        PhysicsData GetOriginalPhysicsData();
        Vector2 GetVelocity2();
        Vector3 GetVelocity3();
        //void AddVelocity3(Vector3 velocity);
        //Vector2D GetFeetPosition();
        Vector2 GetFeetWorldPosition2();
        Vector3 GetFeetWorldPosition3();
        Vector3 GetLocalPos3();
        Vector3 GetWorldPos3();
        Vector2 GetWorldPos2();

        float GetSpeed();

        bool GetIsGrounded();
        void Jump();
        void SetEnabled(bool enabled);
        //void SetWorldPos(Vector2 new_pos);
        void SetVelocity3(Vector3 velocity);
        void SetWorldPos3(Vector3 new_pos);
        void Stop();
        //Transform transform { get; }
        ISteeringBehaviorManager GetSteering();
        void Move(Vector3 motion);
        void RevertAutoApplyToAnimator();
        //IPhysicsAffector GetPhysicsAffector(string name);
        //void SetPhysicsAffector(string name, IPhysicsAffector physicsAffector);
        float MaxSpeed { get; }
    }
}
