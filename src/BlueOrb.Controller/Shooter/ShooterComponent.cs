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

        [SerializeField] private string toggleProjectileMessage = "ToggleProjectile";

        [SerializeField] private string _hudControllerName;

        [SerializeField] private string _setAmmoMessage = "SetAmmo";

        [SerializeField]
        private string removeProjectileTypeHudMessage = "RemoveProjectileType";

        private long ammoBoxShotIndex, toggleProjectileIndex;

        public IProjectileItem CurrentSecondaryProjectile => projectileToggle.GetSelectedProjectile();

        public override void StartListening()
        {
            base.StartListening();
            this.ammoBoxShotIndex = MessageDispatcher.Instance.StartListening(_ammoBoxShotMessage, MessageId, (data) =>
            {
                var projectileConfig = data.ExtraInfo as ProjectileConfig;
                Debug.Log($"(Shooter Controller) Acquired Ammo Box {projectileConfig?.Name}, unique id {projectileConfig.UniqueId}");

                if (projectileConfig == null)
                {
                    throw new Exception("No Projectile Config");
                }

                if (!this.projectileToggle.Contains(projectileConfig.UniqueId))
                {
                    Debug.Log($"(ShooterComponent) Projectile Toggle does NOT contain {projectileConfig.Name}, adding to toggle list");
                    ProjectileItem projectileItem = new ProjectileItem()
                    {
                        CurrentAmmo = projectileConfig.Ammo,
                        ProjectileConfig = projectileConfig
                    };
                    this.projectileToggle.Add(projectileItem);
                }
                else
                {
                    Debug.Log($"(ShooterComponent) Projectile Toggle DOES contain {projectileConfig.Name}, adding to ammo");
                    IProjectileItem projectileItem = this.projectileToggle.GetSelectedProjectile();
                    Debug.Log($"(ShooterComponent) Adding {projectileConfig.Ammo} ammo to {projectileItem.CurrentAmmo} for {projectileItem.ProjectileConfig.name}");
                    projectileItem.CurrentAmmo += projectileConfig.Ammo;
                    MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, (projectileItem.ProjectileConfig.UniqueId, projectileItem.CurrentAmmo));
                }
            });

            this.toggleProjectileIndex = MessageDispatcher.Instance.StartListening(this.toggleProjectileMessage, MessageId, (data) =>
            {
                float direction = (float)data.ExtraInfo;
                Debug.Log($"(Shooter Controller) Toggle Projectile to direction: {direction}");
                projectileToggle.Toggle(direction > 0);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(_ammoBoxShotMessage, MessageId, this.ammoBoxShotIndex);
            MessageDispatcher.Instance.StopListening(this.toggleProjectileMessage, MessageId, this.toggleProjectileIndex);
        }

        public void AddAmmoToSelected(int ammo)
        {
            IProjectileItem projectileItem = this.projectileToggle.GetSelectedProjectile();
            projectileItem.CurrentAmmo += ammo;
            if (projectileItem.CurrentAmmo <= 0)
            {
                this.projectileToggle.Remove(projectileItem);
                MessageDispatcher.Instance.DispatchMsg(this.removeProjectileTypeHudMessage, 0f, null, "Hud Controller", projectileItem.ProjectileConfig.UniqueId);
            }
            else
            {
                MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, (projectileItem.ProjectileConfig.UniqueId, projectileItem.CurrentAmmo));
            }
        }
    }
}