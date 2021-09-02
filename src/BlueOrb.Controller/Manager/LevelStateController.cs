using BlueOrb.Base.Interfaces;
using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Component;
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
    [AddComponentMenu("BlueOrb/Manager/Level State")]
    public class LevelStateController : ComponentBase<LevelStateController>, ILevelStateController
    {
        public int _currentScore;
        public int _highScore;

        [SerializeField]
        private IShooterComponent _shooterComponent;
        public IShooterComponent ShooterComponent => _shooterComponent;

        //private long _setProjectileId;
        protected override void Awake()
        {
            EntityContainer.Instance.LevelStateController = this;
        }

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

            //_setProjectileId = MessageDispatcher.Instance.StartListening("SetProjectile", "Level State", (data) =>
            //{
            //    var projectileConfig = data.ExtraInfo as ProjectileConfig;


            //    _currentProjectile = projectileConfig;

            //    var mainPlayer = EntityContainer.Instance.GetMainCharacter();
            //    MessageDispatcher.Instance.DispatchMsg("SetProjectile", 0f, "Level State", mainPlayer.GetId(), projectileConfig);
            //});
        }
    }
}
