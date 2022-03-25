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
    //public enum ToggleDirection
    //{
    //    Right = 0,
    //    Left = 1
    //}

    [Serializable]
    public class ProjectileToggle
    {
        // Gets set when the player shoots an ammo box
        //[SerializeField] 
        //private IProjectileItem currentSecondaryProjectile;

        [SerializeField] 
        private string selectProjectileHudMessage = "SelectProjectile";

        [SerializeField] 
        private string addProjectileTypeHudMessage = "AddProjectileType";

        [SerializeField] 
        private string removeProjectileTypeHudMessage = "RemoveProjectileType";

        //public IProjectileItem CurrentSecondaryProjectile => currentSecondaryProjectile;

        //private ToggleDirection ToggleDirection = ToggleDirection.Right;
        private List<IProjectileItem> projectileInventory = new List<IProjectileItem>();
        //private Dictionary<string, int> projectilesSet = new Dictionary<string, int>();
        private int currentIndex;

        public void Toggle(bool isRight)
        {
            if (projectileInventory.Count == 0)
            {
                currentIndex = 0;
                return;
            }
            var newItem = isRight ? currentIndex + 1 : currentIndex - 1;
            SetCurrentItem(newItem);
        }

        public void SetCurrentItem(int index)
        {
            Debug.Log($"(ProjectileToggle) Setting index to {index}");
            //GetCurrentItem()?.UnSelect();
            this.currentIndex = index;
            this.currentIndex = CheckBounds(this.currentIndex);
            //GetCurrentItem()?.Select();
            var mainPlayer = EntityContainer.Instance.GetMainCharacter();
            // Inform player object the projectile has changed
            MessageDispatcher.Instance.DispatchMsg(this.selectProjectileHudMessage, 0f, null, mainPlayer.GetId(), this.currentIndex);
            SelectItemInHud();
        }

        private int CheckBounds(int index)
        {
            if (projectileInventory.Count == 0)
            {
                return -1;
            }
            if (index < 0)
            {
                return projectileInventory.Count - 1;
            }                
            if (index > projectileInventory.Count - 1)
            {
                return 0;
            }
            return index;
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
            this.currentIndex = CheckBounds(this.currentIndex);
            MessageDispatcher.Instance.DispatchMsg(this.removeProjectileTypeHudMessage, 0f, null, "Hud Controller", index);
            SelectItemInHud();
            //this.projectileInventory.RemoveAt(this.projectilesSet[projectileItem.ProjectileConfig.UniqueId]);
            //this.projectilesSet.Remove(projectileItem.ProjectileConfig.UniqueId);
            //for (int i = 0; i < this.projectileInventory.Count; i++)
            //{

            //}
        }

        private void SelectItemInHud()
        {
            MessageDispatcher.Instance.DispatchMsg(this.selectProjectileHudMessage, 0f, null, "Hud Controller", this.currentIndex);
        }
    }
}
