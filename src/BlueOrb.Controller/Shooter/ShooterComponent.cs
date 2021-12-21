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
    /// This component lives in the Game Controller Game Object, not on the player
    /// This makes persistence easier and less buggy if the game were to destroy the Main Character or his weapon
    /// </summary>
    [AddComponentMenu("BlueOrb/Manager/Shooter Controller")]
    public class ShooterComponent : ComponentBase<ShooterComponent>, IShooterComponent
    {
        [SerializeField]
        private string MessageId = "Shooter Controller";

        [SerializeField] private ProjectileConfig _currentMainProjectileConfig;
        public ProjectileConfig CurrentMainProjectileConfig => _currentMainProjectileConfig;

        // Gets set when the player shoots an ammo box
        [SerializeField] private ProjectileConfig _currentSecondaryProjectileConfig;
        public ProjectileConfig CurrentSecondaryProjectileConfig => _currentSecondaryProjectileConfig;

        [SerializeField] private string _changeProjectileReceiveMessage;

        [SerializeField] private string _changeProjectileSendMessage;
        [SerializeField] private string _hudControllerName;

        //private ProjectileConfig _currentProjectile;

        private long _projectileId;
        public int _ammoCount;
        [SerializeField] private string _setAmmoMessage;

        //        [SerializeField] private float _speed;

        //        //protected override void Awake()
        //        //{
        //        //    base.Awake();
        //        //    Debug.Log("Block Component is awake!");
        //        //}

        public override void StartListening()
        {
            base.StartListening();
            _projectileId = MessageDispatcher.Instance.StartListening(_changeProjectileReceiveMessage, MessageId, (data) =>
            {
                var projectileConfig = data.ExtraInfo as ProjectileConfig;
                Debug.Log($"(Shooter Controller) Setting Secondary Projectile to {projectileConfig?.Name}");

                if (projectileConfig == null)
                {
                    throw new Exception("No Projectile Config");
                }

                _ammoCount = projectileConfig.Ammo;
                MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, _ammoCount);

                if (_currentSecondaryProjectileConfig == projectileConfig)
                {
                    Debug.Log("Player already has this projectile");
                    return;
                }

                _currentSecondaryProjectileConfig = projectileConfig;

                var mainPlayer = EntityContainer.Instance.GetMainCharacter();
                // Inform player object the projectile has changed
                MessageDispatcher.Instance.DispatchMsg(_changeProjectileSendMessage, 0f, MessageId, mainPlayer.GetId(), null);
                MessageDispatcher.Instance.DispatchMsg(_changeProjectileSendMessage, 0f, MessageId, _hudControllerName, data.ExtraInfo);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(_changeProjectileReceiveMessage, MessageId, _projectileId);
        }

        public void AddAmmo(int ammo)
        {
            _ammoCount += ammo;
            if (_ammoCount <= 0)
            {
                _currentSecondaryProjectileConfig = null;
                MessageDispatcher.Instance.DispatchMsg(_changeProjectileSendMessage, 0f, MessageId, _hudControllerName, null);
            }
            else
            {
                MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, _ammoCount);
            }            
        }
    }
}
