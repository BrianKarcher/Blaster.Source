using BlueOrb.Base.Components;
using BlueOrb.Base.Extensions;
using BlueOrb.Controller.Camera;
using BlueOrb.Controller.Damage;
using BlueOrb.Messaging;
using BlueOrb.Physics;
using UnityEngine;

namespace BlueOrb.Controller.Player
{
    public class LockOnMovementLogic : IPlayerMovementLogic
    {
        private PlayerController _playerController;
        private ThirdPersonCameraController _camera;
        // TODO Unserialize this!
        [SerializeField]
        private float _forwardAmount;
        // TODO Unserialize this!
        //[SerializeField]
        private float _turnAmount;
        private PhysicsComponent _physicsComponent;
        private AnimationComponent _animationComponent;
        private EntityStatsComponent _entityStatsComponent;
        private PlayerParryComponent _playerParryComponent;

        public LockOnMovementLogic(PlayerController playerController)
        {
            _playerController = playerController;
            _camera = UnityEngine.Camera.main.transform.parent.GetComponent<ThirdPersonCameraController>();
            _physicsComponent = playerController.GetComponentRepository().GetComponent<PhysicsComponent>();
            _animationComponent = playerController.GetComponentRepository().GetComponent<AnimationComponent>();
            if (_playerParryComponent == null)
                _playerParryComponent = playerController.GetComponentRepository().GetComponent<PlayerParryComponent>();
            if (_entityStatsComponent == null)
                _entityStatsComponent = playerController.GetComponentRepository().GetComponent<EntityStatsComponent>();
        }

        
    }
}
