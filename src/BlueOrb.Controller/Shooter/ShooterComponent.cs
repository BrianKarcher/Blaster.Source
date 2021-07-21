using BlueOrb.Base.Item;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Player;
using BlueOrb.Messaging;
using System;
using UnityEngine;

namespace BlueOrb.Controller.Component
{
    /// <summary>
    /// This component lives in the Game Controller Game Object, not on the player
    /// This makes persistence easier and less buggy if the game were to destroy the Main Character or his weapon
    /// </summary>
    [AddComponentMenu("BlueOrb/Components/Shooter")]
    public class ShooterComponent : ComponentBase<ShooterComponent>
    {
        [SerializeField]
        private GameObject _projectileSpawnPoint;
        public GameObject ProjectileSpawnPoint => _projectileSpawnPoint;
        //        //[SerializeField] private PlayerController _playerController;
        [SerializeField] private ProjectileConfig _currentProjectileConfig;
        public ProjectileConfig CurrentProjectileConfig => _currentProjectileConfig;

        [SerializeField] private string _changeProjectileReceiveMessage;

        [SerializeField] private string _changeProjectileSendMessage;

        private long _projectileId;

        //        [SerializeField] private float _speed;

        //        //protected override void Awake()
        //        //{
        //        //    base.Awake();
        //        //    Debug.Log("Block Component is awake!");
        //        //}

        public override void StartListening()
        {
            base.StartListening();
            _projectileId = MessageDispatcher.Instance.StartListening(_changeProjectileReceiveMessage, _componentRepository.GetId(), (data) =>
            {
                _currentProjectileConfig = data.ExtraInfo as ProjectileConfig;
                var mainPlayer = EntityContainer.Instance.GetMainCharacter();
                // Inform player object the projectile has changed
                MessageDispatcher.Instance.DispatchMsg(_changeProjectileSendMessage, 0f, _componentRepository.GetId(), mainPlayer.GetId(), null);
                MessageDispatcher.Instance.DispatchMsg(_changeProjectileSendMessage, 0f, _componentRepository.GetId(), "Hud Controller", data.ExtraInfo);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(_changeProjectileReceiveMessage, _componentRepository.GetId(), _projectileId);
        }

        //        public void Shoot()
        //        {
        //            //transform.TransformPoint(_projectileCreatePoint);
        //            GameObject newObject = null;

        //            var projectilePrefab = GameStateController.Instance.CurrentShard == null ? 
        //                GameStateController.Instance.CurrentMold.ReferencePrefab : 
        //                GameStateController.Instance.CurrentShard.ReferencePrefab;
        //            if (projectilePrefab == null)
        //            {
        //                Debug.LogError($"Associated item {GameStateController.Instance.CurrentShard.name} has no prefab reference.");
        //                return;
        //            }

        //            var shooterRotation = transform.rotation;

        //            CreateMuzzleFlash(_projectileCreatePoint.transform.position, shooterRotation);

        //            newObject = GameObject.Instantiate(projectilePrefab, _projectileCreatePoint.transform.position, shooterRotation);
        //            var rigidBody = newObject.GetComponent<Rigidbody>();
        //            if (rigidBody == null)
        //                throw new Exception("Could not locate PhysicsComponent or RigidBody!");
        //            var direction = transform.forward;
        //            var velocity = direction.normalized * _speed;
        //            rigidBody.velocity = velocity;
        //        }

        //        public void CreateMuzzleFlash(Vector3 pos, Quaternion rotation)
        //        {
        //            // Muzzle Flash
        //            if (GameStateController.Instance.CurrentShard.MuzzleFlash == null)
        //            {
        //                Debug.LogError("This shard has no muzzle flash. Please apply one.");
        //                return;
        //            }

        //            GameObject.Instantiate(GameStateController.Instance.CurrentShard.MuzzleFlash, pos, rotation);
        //        }
    }
}
