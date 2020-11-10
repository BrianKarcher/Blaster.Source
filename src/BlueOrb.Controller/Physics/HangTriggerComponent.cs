using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Controller.Physics
{
    [AddComponentMenu("RQ/Components/Hang Trigger")]
    public class HangTriggerComponent : ComponentBase<HangTriggerComponent>
    {
        [SerializeField] private GameObject _playerPlacePosition;
        public GameObject PlayerPlacePosition => _playerPlacePosition;
    }
}
