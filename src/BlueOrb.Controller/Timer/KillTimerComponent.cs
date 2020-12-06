using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Controller.Timer
{
    [AddComponentMenu("BlueOrb/Components/Kill Timer")]
    public class KillTimerComponent : ComponentBase<KillTimerComponent>
    {
        [SerializeField]
        private float _killInSeconds;

        private float _killTime;

        private void Start()
        {
            _killTime = Time.time + _killInSeconds;
        }

        private void FixedUpdate()
        {
            if (Time.time > _killTime)
            {
                Destroy(_componentRepository.gameObject);
            }
        }
    }
}
