using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Controller.Camera
{
    public interface ICameraController : IComponentBase
    {
        bool Raycast(float maxDistance, int layerMask, out RaycastHit hitInfo);
    }
}