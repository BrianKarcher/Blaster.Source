using BlueOrb.Base.Interfaces;
using BlueOrb.Base.Item;
using BlueOrb.Base.Manager;
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
        //public float _currentHp;
        //public float _maxHp;

        [SerializeField]
        private ShooterComponent _shooterComponent;
        public IShooterComponent ShooterComponent => _shooterComponent;

        private long _addPointsIndex;

        //private static LevelStateController _instance;
        //[HideInInspector]
        //public static LevelStateController Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            _instance = GameObject.FindObjectOfType<LevelStateController>();
        //            _instance.Init();
        //        }
        //        return _instance;
        //    }
        //}

        //private long _setProjectileId;
        protected override void Awake()
        {
            EntityContainer.Instance.LevelStateController = this;
            //StartLevel();
        }

        public void StartLevel()
        {
            //var stats = GameStateController.Instance.EntityStats;
            //_currentHp = _maxHp;
            //SetMaxHp(_maxHp);

            UpdateUI();
        }

        //public void SetCurrentHp(float hp)
        //{
        //    _currentHp = hp;
        //    UpdateUI();
        //}

        //public void AddHp(float hp)
        //{
        //    _currentHp += hp;
        //    UpdateUI();
        //}

        //public void SetMaxHp(float maxHp)
        //{
        //    _maxHp = maxHp;
        //    UpdateUI();
        //}

        public void PrepareStartStageData()
        {
            _currentScore = 0;
            UpdateUI();
        }

        public void AddPoints(int points)
        {
            _currentScore += points;
            UpdateUI();
        }

        private void UpdateUI()
        {
            MessageDispatcher.Instance.DispatchMsg("SetCurrentScore", 0f, string.Empty, "UI Controller", _currentScore);
            //MessageDispatcher.Instance.DispatchMsg("SetHp", 0f, this.GetId(), "Hud Controller", (_currentHp, _maxHp));
        }

        public override void StartListening()
        {
            base.StartListening();

            _addPointsIndex = MessageDispatcher.Instance.StartListening("AddPoints", "Level Controller", (data) =>
            {
                var points = (PointsData)data.ExtraInfo;
                _currentScore += points.Points;
                MessageDispatcher.Instance.DispatchMsg("SetCurrentScore", 0f, _componentRepository.GetId(), "UI Controller", _currentScore);
                MessageDispatcher.Instance.DispatchMsg("CreatePointsLabel", 0f, _componentRepository.GetId(), "UI Controller", points);
            });
            //_setProjectileId = MessageDispatcher.Instance.StartListening("SetProjectile", "Level State", (data) =>
            //{
            //    var projectileConfig = data.ExtraInfo as ProjectileConfig;


            //    _currentProjectile = projectileConfig;

            //    var mainPlayer = EntityContainer.Instance.GetMainCharacter();
            //    MessageDispatcher.Instance.DispatchMsg("SetProjectile", 0f, "Level State", mainPlayer.GetId(), projectileConfig);
            //});
        }
    }

    public class PointsData
    {
        public int Score { get; set; }
        public int Points { get; set; }
        public Color Color { get; set; } = Color.white;
        public Vector3 Position { get; set; }
    }
}
