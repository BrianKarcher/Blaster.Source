using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Physics
{
    public interface IMovementController
    {
        Vector3 GetWorldPos3();
        Vector2 GetWorldPos2();
        Transform transform { get; }
        Vector3 GetVelocity3();
        IEntity GetComponentRepository();
    }
}
