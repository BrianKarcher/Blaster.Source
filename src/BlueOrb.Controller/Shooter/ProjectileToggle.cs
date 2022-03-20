using BlueOrb.Base.Interfaces;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Component;
using BlueOrb.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Shooter
{
    public enum ToggleDirection
    {
        Right = 0,
        Left = 1
    }

    public class ProjectileToggle
    {
        // Gets set when the player shoots an ammo box
        //[SerializeField] 
        //private IProjectileItem currentSecondaryProjectile;

        [SerializeField] 
        private string setProjectileHudMessage = "SetProjectile";

        [SerializeField] 
        private string addProjectileTypeHudMessage = "AddProjectileType";

        [SerializeField] 
        private string removeProjectileTypeHudMessage = "RemoveProjectileType";

        //public IProjectileItem CurrentSecondaryProjectile => currentSecondaryProjectile;

        private ToggleDirection ToggleDirection = ToggleDirection.Right;
        private List<IProjectileItem> projectileInventory = new List<IProjectileItem>();
        //private Dictionary<string, int> projectilesSet = new Dictionary<string, int>();
        private int currentIndex;

        public void Toggle(bool right)
        {
            if (ToggleDirection != ToggleDirection.Right)
                right = !right;
            var newItem = right ? currentIndex - 1 : currentIndex + 1;
            SetCurrentItem(newItem);
        }

        public void SetCurrentItem(int index)
        {
            //GetCurrentItem()?.UnSelect();
            this.currentIndex = index;
            CheckBounds();
            //GetCurrentItem()?.Select();
            var mainPlayer = EntityContainer.Instance.GetMainCharacter();
            // Inform player object the projectile has changed
            MessageDispatcher.Instance.DispatchMsg(this.setProjectileHudMessage, 0f, null, mainPlayer.GetId(), null);
            MessageDispatcher.Instance.DispatchMsg(this.setProjectileHudMessage, 0f, null, "Hud Controller", this.currentIndex);
        }

        private void CheckBounds()
        {
            if (currentIndex < 0)
                currentIndex = projectileInventory.Count - 1;
            if (currentIndex > projectileInventory.Count - 1)
                currentIndex = 0;
        }

        public bool Contains(string uniqueId)
        {
            for (int i = 0; i < projectileInventory.Count; i++)
            {
                if (projectileInventory[i].ProjectileConfig.UniqueId == uniqueId)
                {
                    return true;
                }
            }
            return false;
        }

        public int Count() => projectileInventory.Count;

        public IProjectileItem GetSelectedProjectile()
        {
            if (this.projectileInventory.Count == 0)
                return null;
            return this.projectileInventory[this.currentIndex];
        }
        
        public void Add(IProjectileItem projectileItem)
        {
            this.projectileInventory.Add(projectileItem);
            MessageDispatcher.Instance.DispatchMsg(this.addProjectileTypeHudMessage, 0f, null, "Hud Controller", projectileItem);
            if (currentIndex == -1)
            {
                SetCurrentItem(0);
            }
            //this.projectilesSet.Add(projectileItem.ProjectileConfig.UniqueId, this.projectileInventory.Count - 1);
            
        }

        public void Remove(IProjectileItem projectileItem)
        {
            int index = -1;
            for (int i = 0; i < this.projectileInventory.Count; i++)
            {
                if (this.projectileInventory[i].ProjectileConfig.UniqueId == projectileItem.ProjectileConfig.UniqueId)
                {
                    index = i;
                    break;
                }
            }
            this.projectileInventory.RemoveAt(index);
            MessageDispatcher.Instance.DispatchMsg(this.removeProjectileTypeHudMessage, 0f, null, "Hud Controller", index);
            //this.projectileInventory.RemoveAt(this.projectilesSet[projectileItem.ProjectileConfig.UniqueId]);
            //this.projectilesSet.Remove(projectileItem.ProjectileConfig.UniqueId);
            //for (int i = 0; i < this.projectileInventory.Count; i++)
            //{

            //}
        }
    }
}
