using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Manager
{
    /// <summary>
    /// Gets instantiated and destroyed by the SceneController
    /// Controls the level state
    /// Scope: Single level, gets instantiated on level entry and destroyed on level exit
    /// </summary>
    public class LevelStateController : ComponentBase<LevelStateController>
    {
        public int _currentScore;
        public int _highScore;

        private ProjectileConfig _currentProjectile;

        private long _setProjectileId;

        public void PrepareStartStageData()
        {
            _currentScore = 0;
            SendScoreToUI();
        }

        public void AddPoints(int points)
        {
            _currentScore += points;
            SendScoreToUI();
        }

        private void SendScoreToUI()
        {
            MessageDispatcher.Instance.DispatchMsg("SetCurrentScore", 0f, string.Empty, "UI Controller", _currentScore);
        }

        public override void StartListening()
        {
            base.StartListening();

            _setProjectileId = MessageDispatcher.Instance.StartListening("SetProjectile", "Level State", (data) =>
            {
                var projectileConfig = data.ExtraInfo as ProjectileConfig;
                if (projectileConfig == null)
                {
                    throw new Exception("No Projectile Config");
                }

                if (_currentProjectile == projectileConfig)
                {
                    Debug.Log("Player already has this projectile");
                    return;
                }


            });
        }
    }
}
