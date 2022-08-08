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
    [Serializable]
    public class ProjectileToggle
    {
        // Gets set when the player shoots an ammo box
        //[SerializeField] 
        //private IProjectileItem currentSecondaryProjectile;

        [SerializeField] 
        private string addProjectileTypeHudMessage = "AddProjectileType";


        //public IProjectileItem CurrentSecondaryProjectile => currentSecondaryProjectile;

        //private ToggleDirection ToggleDirection = ToggleDirection.Right;
        //private Dictionary<string, int> projectilesSet = new Dictionary<string, int>();
        private int currentIndex;

        public void SetCurrentItem(int index)
        {
            Debug.Log($"(ProjectileToggle) Setting index to {index}");
            //GetCurrentItem()?.UnSelect();
            this.currentIndex = index;
            this.currentIndex = CheckBounds(this.currentIndex);
            //GetCurrentItem()?.Select();
            SelectItemInHud();
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
            MessageDispatcher.Instance.DispatchMsg(this.addProjectileTypeHudMessage, 0f, null, "Hud Controller", projectileItem);
        }

        public void Remove(IProjectileItem projectileItem)
        {
            
        }
    }
}