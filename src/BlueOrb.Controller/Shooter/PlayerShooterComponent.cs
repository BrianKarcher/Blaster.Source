using BlueOrb.Base.Interfaces;
using BlueOrb.Base.Item;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Player;
using BlueOrb.Messaging;
using BlueOrb.Physics;
using System;
using UnityEngine;

namespace BlueOrb.Controller.Component
{
    /// <summary>
    /// This component lives on the player, NOT the Game Controller
    /// </summary>
    [AddComponentMenu("BlueOrb/Components/Player Shooter")]
    public class PlayerShooterComponent : ComponentBase<PlayerShooterComponent>
    {
        //[SerializeField] private ProjectileConfig _mainProjectile;

        [SerializeField]
        private GameObject _projectileSpawnPoint;
        public GameObject ProjectileSpawnPoint => _projectileSpawnPoint;
        //        //[SerializeField] private PlayerController _playerController;
        //[SerializeField] private ProjectileConfig _currentSecondaryProjectileConfig;
        //public ProjectileConfig CurrentSecondaryProjectileConfig => _currentSecondaryProjectileConfig;

        [SerializeField] private GameObject _spawnPointPosition;

        [SerializeField] float maxRaycastDistance = 1000f;

        [SerializeField] private LayerMask _layerMask;

        [SerializeField] private string _changeProjectileReceiveMessage;

        [SerializeField] private string _changeProjectileSendMessage;

        [SerializeField] private GameObject _shooterGameObject;

        //private ProjectileConfig _currentProjectile;

        private long _projectileId;
        private UnityEngine.Camera _camera;

        //        [SerializeField] private float _speed;

        //        //protected override void Awake()
        //        //{
        //        //    base.Awake();
        //        //    Debug.Log("Block Component is awake!");
        //        //}

        protected override void Awake()
        {
            base.Awake();
            if (_camera == null)
            {
                var cameraGo = UnityEngine.Camera.main;
                _camera = cameraGo;
                //_camera = _entity.Components.GetComponent<ICameraController>();
            }
        }

        //public override void StartListening()
        //{
        //    base.StartListening();
        //    _projectileId = MessageDispatcher.Instance.StartListening(_changeProjectileReceiveMessage, _componentRepository.GetId(), (data) =>
        //    {
        //        var projectileConfig = data.ExtraInfo as ProjectileConfig;

        //        if (projectileConfig == null)
        //        {
        //            throw new Exception("No Projectile Config");
        //        }

        //        if (_currentSecondaryProjectileConfig == projectileConfig)
        //        {
        //            Debug.Log("Player already has this projectile");
        //            return;
        //        }

        //        _currentSecondaryProjectileConfig = projectileConfig;

        //    });
        //}

        //public override void StopListening()
        //{
        //    base.StopListening();
        //    MessageDispatcher.Instance.StopListening(_changeProjectileReceiveMessage, _componentRepository.GetId(), _projectileId);
        //}

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

        public void ShootMainProjectile()
        {
            //Debug.Log("Shooting Main Projectile");
            var projectileConfig = EntityContainer.Instance.LevelStateController.ShooterComponent.CurrentMainProjectileConfig;
            ProcessShoot(projectileConfig);
        }

        public void ShootSecondaryProjectile()
        {
            //Debug.Log("Shooting Secondary Projectile");
            var projectileInventory = EntityContainer.Instance.LevelStateController.ShooterComponent.CurrentSecondaryProjectile;
            ProcessShoot(projectileInventory.ProjectileConfig);
            DecreaseAmmo();
        }

        private void DecreaseAmmo()
        {
            EntityContainer.Instance.LevelStateController.ShooterComponent.AddAmmoToSelected(-1);
        }

        private void ProcessShoot(ProjectileConfig projectileConfig)
        {
            if (projectileConfig == null)
                return;

            var velocity = CalculateVelocity(projectileConfig.MaxSpeed);

            //var angleToRotate = _animationComponent.GetFacingDirection().GetDirectionAngle();

            //var position = _physicsComponent.GetWorldPos() + RotateAroundAxis(new Vector3(_offset.x, _offset.y, 0f), angleToRotate, new Vector3(0, 0, 1));

            Vector3 position = _spawnPointPosition.transform.position;

            //var newObject = (GameObject.Instantiate(_objectToShoot, position, entity.transform.rotation) as Transform).GetComponent<IComponentRepository>();
            //IComponentRepository newObject = null;
            //if (string.IsNullOrEmpty(ObjectPoolName))
            //{
            //    newObject = (GameObject.Instantiate(_objectToShoot, position, entity.transform.rotation) as Transform).GetComponent<IComponentRepository>();
            //}
            //else
            //{
            //    var newGO = ObjectPool.Instance.PullGameObjectFromPool(ObjectPoolName, position, Quaternion.identity);
            //    newObject = newGO.GetComponent<IComponentRepository>();
            //    newObject.Reset();
            //}
            //GameObject newObject = null;
            Quaternion rotation;
            //if (LookToVelocity)
            //{
            //    rotation = Quaternion.LookRotation(velocity);
            //}
            //else
            //{
            rotation = _shooterGameObject.transform.rotation; // * Quaternion.Euler(_rotation);
            //}
            var newObject = GameObject.Instantiate(projectileConfig.ReferencePrefab, position, rotation) as GameObject;
            //if (_createAsChild)
            //{
            //    newObject.transform.parent = _entity.transform;
            //}

            IEntity newEntity = null;
            if (newObject != null)
                newEntity = newObject.GetComponent<IEntity>();
            PhysicsComponent newPhysicsComponent;
            //if (newEntity == null)
            //{
            //    // Not an Entity Common Component
            //    newPhysicsComponent = newObject.GetComponent<PhysicsComponent>();
            //}
            //else
            //{
            // Get the Physics Component through the Entity Common Component
            newPhysicsComponent = newEntity.Components.GetComponent<PhysicsComponent>();
            //}

            if (newPhysicsComponent == null)
            {
                var rigidBody = newObject.GetComponent<Rigidbody>();
                if (rigidBody == null)
                    Debug.LogError($"Could not locate PhysicsComponent or RigidBody on {newObject.name}!");
                else
                {
                    //throw new Exception("Could not locate PhysicsComponent or RigidBody!");
                    rigidBody.velocity = velocity;
                }
            }
            else
            {
                newPhysicsComponent?.SetVelocity3(velocity);
            }
            //var thisLevel = GetLevel();

            //newPhysicsComponent.GetPhysicsAffector("Steering").Velocity = velocity.ToVector3(0);
            //newPhysicsComponent.SetVelocity(velocity.ToVector3(0));
            // Set the new object to the same level as this one
            //newObject.SendMessageToComponents<CollisionComponent>(0f, this._uniqueId, Telegrams.SetLevelHeight,
            //    thisLevel);
            //var otherFloorComponent = repo.Components.GetComponent<FloorComponent>();
            //otherFloorComponent?.SetFloor((int)thisLevel);
        }

        private Vector3 CalculateVelocity(float maxSpeed)
        {
            //var speed = UnityEngine.Random.Range(_minSpeed, _maxSpeed);

            Vector3 direction = GetShootDirection();

            var velocity = direction.normalized * maxSpeed;

            return velocity;
        }

        private Vector3 GetShootDirection()
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            // Create a vector at the center of our camera's viewport
            // Vector3 rayOrigin = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // var rtn = UnityEngine.Physics.Raycast(rayOrigin, _camera.transform.forward, out hitInfo, maxDistance, layerMask);

            var rtn = UnityEngine.Physics.Raycast(ray, out var hitInfo, maxRaycastDistance, _layerMask);
            //if (!_camera.Raycast(1000f, _layerMask, out var hitInfo))
            if (!rtn)
            {
                //Debug.Log("Shooter Raycast No Hit");
                // If raycast no-hit, just point down the camera forward direction very far
                return _camera.transform.TransformPoint(new Vector3(0f, 0f, 1000f)) - _spawnPointPosition.transform.position;
            }
            else
            {
                Debug.Log("Shooter Raycast Hit");
                return hitInfo.point - _spawnPointPosition.transform.position;
            }
        }
    }
}
