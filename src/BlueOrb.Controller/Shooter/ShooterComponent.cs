using BlueOrb.Base.Interfaces;
using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
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

        [SerializeField] private ProjectileConfig _currentMainProjectileConfig;
        public ProjectileConfig CurrentMainProjectileConfig => _currentMainProjectileConfig;

        [SerializeField] private string _ammoBoxShotMessage = "AmmoBoxShot";

        [SerializeField] private string toggleProjectileMessage = "ToggleProjectile";

        [SerializeField] private string toggleProjectileHudMessage = "ToggleProjectile";

        [SerializeField] private string _hudControllerName;

        [SerializeField] private string _setAmmoMessage = "SetAmmo";

        [SerializeField]
        private string addProjectileTypeHudMessage = "AddProjectileType";

        [SerializeField]
        private string removeProjectileTypeHudMessage = "RemoveProjectileType";

        private long ammoBoxShotIndex, toggleProjectileIndex;

        private Dictionary<string, ProjectileItem> projectileItems = new Dictionary<string, ProjectileItem>();

        private string currentSecondaryProjectile;

        public void SetSecondaryProjectile(string uniqueId) => this.currentSecondaryProjectile = uniqueId;

        public IProjectileItem GetSecondaryProjectile()
        {
            if (this.currentSecondaryProjectile == null)
            {
                return null;
            }
            this.projectileItems.TryGetValue(this.currentSecondaryProjectile, out ProjectileItem item);
            if (item == null)
            {
                Debug.LogError("Could not find projectile " + this.currentSecondaryProjectile);
            }
            return item;
        }

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

                if (!this.projectileItems.TryGetValue(projectileConfig.UniqueId, out ProjectileItem projectileItem))
                {
                    Debug.Log($"(ShooterComponent) Projectile Toggle does NOT contain {projectileConfig.Name}, adding to toggle list");
                    projectileItem = new ProjectileItem()
                    {
                        CurrentAmmo = projectileConfig.Ammo,
                        ProjectileConfig = projectileConfig
                    };
                    projectileItems.Add(projectileConfig.UniqueId, projectileItem);
                    MessageDispatcher.Instance.DispatchMsg(this.addProjectileTypeHudMessage, 0f, null, "Hud Controller", projectileItem);
                }
                else
                {
                    Debug.Log($"(ShooterComponent) Projectile Toggle DOES contain {projectileConfig.Name}, adding to ammo");
                    Debug.Log($"(ShooterComponent) Adding {projectileConfig.Ammo} ammo to {projectileItem.CurrentAmmo} for {projectileItem.ProjectileConfig.name}");
                    projectileItem.CurrentAmmo += projectileConfig.Ammo;
                    MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, (projectileItem.ProjectileConfig.UniqueId, projectileItem.CurrentAmmo));
                }
            });

            this.toggleProjectileIndex = MessageDispatcher.Instance.StartListening(this.toggleProjectileMessage, MessageId, (data) =>
            {
                float direction = (float)data.ExtraInfo;
                Debug.Log($"(Shooter Controller) Toggle Projectile to direction: {direction}");
                MessageDispatcher.Instance.DispatchMsg(this.toggleProjectileHudMessage, 0f, null, "Hud Controller", direction > 0);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(_ammoBoxShotMessage, MessageId, this.ammoBoxShotIndex);
            MessageDispatcher.Instance.StopListening(this.toggleProjectileMessage, MessageId, this.toggleProjectileIndex);
        }

        public void Clear()
        {
            projectileItems.Clear();
            this.currentSecondaryProjectile = null;
        }

        public void AddAmmoToSelected(int ammo)
        {
            IProjectileItem projectileItem = this.GetSecondaryProjectile();
            projectileItem.CurrentAmmo += ammo;
            if (projectileItem.CurrentAmmo <= 0)
            {
                projectileItems.Remove(projectileItem.ProjectileConfig.UniqueId);
                MessageDispatcher.Instance.DispatchMsg(this.removeProjectileTypeHudMessage, 0f, null, "Hud Controller", projectileItem.ProjectileConfig.UniqueId);
            }
            else
            {
                MessageDispatcher.Instance.DispatchMsg(_setAmmoMessage, 0f, _componentRepository.GetId(), _hudControllerName, (projectileItem.ProjectileConfig.UniqueId, projectileItem.CurrentAmmo));
            }
        }
    }
}