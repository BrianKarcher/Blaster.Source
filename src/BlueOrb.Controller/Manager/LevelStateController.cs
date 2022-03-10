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
        private const string Id = "Level Controller";
        //private bool _isLevelBegun = false;

        public override string GetId()
        {
            return Id;
        }

        private void Start()
        {
            GameStateController.Instance.LevelStateController = this;
        }

        //public void SetLevelBegan(bool hasBegun)
        //{
        //    _isLevelBegun = hasBegun;
        //}
        //public float _currentHp;
        //public float _maxHp;

        [SerializeField]
        private ShooterComponent _shooterComponent;
        [SerializeField] private string levelStartMessage;
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
            MessageDispatcher.Instance.DispatchMsg(levelStartMessage, 0f, _componentRepository.GetId(), null, null);
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
            MessageDispatcher.Instance.DispatchMsg("SetCurrentScore", 0f, _componentRepository.GetId(), "UI Controller", _currentScore);
            //MessageDispatcher.Instance.DispatchMsg("SetHp", 0f, this.GetId(), "Hud Controller", (_currentHp, _maxHp));
        }

        public override void StartListening()
        {
            base.StartListening();

            _addPointsIndex = MessageDispatcher.Instance.StartListening("AddPoints", Id, (data) =>
            {
                var points = (PointsData)data.ExtraInfo;
                _currentScore += points.Points;
                MessageDispatcher.Instance.DispatchMsg("SetCurrentScore", 0f, _componentRepository.GetId(), "UI Controller", _currentScore);
                MessageDispatcher.Instance.DispatchMsg("CreatePointsLabel", 0f, _componentRepository.GetId(), "UI Controller", points);
            });
            //_levelScoreIndex = MessageDispatcher.Instance.StartListening("LevelStart", _componentRepository.GetId(), (data) =>
            //{
            //    _isLevelBegun = true;
            //});
            //_setProjectileId = MessageDispatcher.Instance.StartListening("SetProjectile", "Level State", (data) =>
            //{
            //    var projectileConfig = data.ExtraInfo as ProjectileConfig;


            //    _currentProjectile = projectileConfig;

            //    var mainPlayer = EntityContainer.Instance.GetMainCharacter();
            //    MessageDispatcher.Instance.DispatchMsg("SetProjectile", 0f, "Level State", mainPlayer.GetId(), projectileConfig);
            //});
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("AddPoints", _componentRepository.GetId(), _addPointsIndex);
            //MessageDispatcher.Instance.StopListening("LevelStart", _componentRepository.GetId(), _levelScoreIndex);
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
