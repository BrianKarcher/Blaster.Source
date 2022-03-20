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

        [SerializeField] private string _hudControllerName;

        [SerializeField] private string _setAmmoMessage;

        private long ammoBoxShotIndex;

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

                if (!this.projectileToggle.Contains(projectileConfig.UniqueId))
                {
                    ProjectileItem projectileItem = new ProjectileItem()
                    {
                        CurrentAmmo = projectileConfig.Ammo,
                        ProjectileConfig = projectileConfig
                    };
                    this.projectileToggle.Add(projectileItem);
                }
                else
                {
                    IProjectileItem projectileItem = this.projectileToggle.GetSelectedProjectile();
                    projectileItem.CurrentAmmo += projectileConfig.Ammo;
                    MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, projectileItem.CurrentAmmo);
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
