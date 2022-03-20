using BlueOrb.Base.Interfaces;
using BlueOrb.Base.Item;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Player;
using BlueOrb.Controller.Shooter;
using BlueOrb.Messaging;
using BlueOrb.Physics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlueOrb.Controller.Component
{
    public class ProjectileItem : IProjectileItem
    {
        public int CurrentAmmo { get; set; }
        public ProjectileConfig ProjectileConfig { get; set; }
    }

    /// <summary>
    /// This component lives in the Game Controller Game Object, not on the player
    /// This makes persistence easier and less buggy if the game were to destroy the Main Character or his weapon
    /// </summary>
    [AddComponentMenu("BlueOrb/Manager/Shooter Controller")]
    public class ShooterComponent : ComponentBase<ShooterComponent>, IShooterComponent
    {
        [SerializeField]
        private string MessageId = "Shooter Controller";

        [SerializeField]
        private ProjectileToggle projectileToggle;

        [SerializeField] private ProjectileConfig _currentMainProjectileConfig;
        public ProjectileConfig CurrentMainProjectileConfig => _currentMainProjectileConfig;

        [SerializeField] private string _ammoBoxShotMessage = "AmmoBoxShot";

        //[SerializeField] private string _changeProjectileSendMessage;
        //[SerializeField] private string deleteCurrentAndChangeProjectileHudMessage = "DeleteCurrentAndChangeProjectile";

        [SerializeField] private string _hudControllerName;

        //private ProjectileConfig _currentProjectile;

        private long ammoBoxShotIndex;
        //public int _ammoCount;
        [SerializeField] private string _setAmmoMessage;

        private Dictionary<string, ProjectileItem> projectileInventory;

        protected override void Awake()
        {
            projectileInventory = new Dictionary<string, ProjectileItem>();
        }

        //        [SerializeField] private float _speed;

        //        //protected override void Awake()
        //        //{
        //        //    base.Awake();
        //        //    Debug.Log("Block Component is awake!");
        //        //}

        public override void StartListening()
        {
            base.StartListening();
            this.ammoBoxShotIndex = MessageDispatcher.Instance.StartListening(_ammoBoxShotMessage, MessageId, (data) =>
            {
                var projectileConfig = data.ExtraInfo as ProjectileConfig;
                Debug.Log($"(Shooter Controller) Setting Secondary Projectile to {projectileConfig?.Name}");

                if (projectileConfig == null)
                {
                    throw new Exception("No Projectile Config");
                }

                if (!this.projectileInventory.ContainsKey(projectileConfig.UniqueId))
                {
                    this.projectileInventory.Add(projectileConfig.UniqueId, new ProjectileInventory()
                    {
                        CurrentAmmo = 0,
                        ProjectileConfig = projectileConfig
                    });
                }

                ProjectileItem projectileItem = this.projectileInventory[projectileConfig.UniqueId];

                projectileItem.CurrentAmmo += projectileConfig.Ammo;
                

                //if (_currentSecondaryProjectileConfig == projectileConfig)
                //{
                //    Debug.Log("Player already has this projectile");
                //    return;
                //}

                var mainPlayer = EntityContainer.Instance.GetMainCharacter();
                if (currentSecondaryProjectile == null)
                {
                    currentSecondaryProjectile = projectileInventory;
                    // Inform player object the projectile has changed
                    MessageDispatcher.Instance.DispatchMsg(_changeProjectileSendMessage, 0f, MessageId, mainPlayer.GetId(), null);
                    MessageDispatcher.Instance.DispatchMsg(_changeProjectileSendMessage, 0f, MessageId, _hudControllerName, data.ExtraInfo);
                }
                else
                {
                    // Otherwise just change the ammo count
                    MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, projectileInventory);
                }
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(_ammoBoxShotMessage, MessageId, this.ammoBoxShotIndex);
        }

        public void AddAmmo(int ammo)
        {
            IProjectileItem projectileItem = this.projectileToggle.GetSelectedProjectile();
            projectileItem.CurrentAmmo += ammo;
            if (projectileItem.CurrentAmmo <= 0)
            {
                this.projectileToggle.Remove(projectileItem);
            }
            else
            {
                MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, projectileItem.CurrentAmmo);
            }            
        }
    }
}
