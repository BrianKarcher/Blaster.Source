using BlueOrb.Base.Extensions;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using System;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    [AddComponentMenu("Blue Orb/Components/Projectile Controller")]
    public class ProjectileController : ComponentBase<EnemyController>
    {
        [SerializeField]
        private float lifetimeSeconds = 60f;

        private float lifeEndTime;
        private bool lifeCountdownStarted = false;

        private void OnUpdate()
        {
            if (!GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                return;
            }
            if (!lifeCountdownStarted)
            {
                this.lifeEndTime = Time.time + lifetimeSeconds;
            }
            if (Time.time > this.lifeEndTime)
            {
                this.GetComponentRepository().Destroy();
            }
        }
    }
}