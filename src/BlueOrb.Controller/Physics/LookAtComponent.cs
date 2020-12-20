using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Controller.Block
{
    [AddComponentMenu("RQ/Components/Look At")]
    public class LookAtComponent : ComponentBase<LookAtComponent>
    {
        [SerializeField]
        private Transform _target;

        private void Update()
        {
            if (_target != null)
            {
                var lookRotation = Quaternion.LookRotation(_target.position - _componentRepository.GetPosition(), Vector3.up);
                lookRotation.z = 0;
                lookRotation.x = 0;
                //this.transform.LookAt(_target, Vector3.up);
                this.transform.rotation = lookRotation;
            }
        }
    }
}
