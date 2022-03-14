using BlueOrb.Base.Config;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using System.Collections.Generic;
using UnityEngine;

namespace BlueOrb.Controller.Controller
{
    [AddComponentMenu("BlueOrb/Manager/Level Start Timer")]
    public class LevelStartTimer : ComponentBase<LevelStartTimer>
    {
        [SerializeField]
        private List<GameObject> gameObjectsToEnable;
        [SerializeField]
        private GameObject _levelState;
        [SerializeField]
        private string StartText = "START";

        private int _currentTime;
        private float _startTime;
        private int _displayedTime;
        private GameSettingsConfig gameSettingsConfig;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            gameSettingsConfig = GameStateController.Instance.GameSettingsConfig;
            _currentTime = gameSettingsConfig.LevelStartSeconds;
            _displayedTime = gameSettingsConfig.LevelStartSeconds;
            _startTime = Time.time;
            if (gameSettingsConfig.ImmediateStartGame)
            {
                LevelStart();
                return;
            }
            MessageDispatcher.Instance.DispatchMsg("ShowTimer", 0f, _componentRepository.GetId(), "Hud Controller", true);
            SetDisplay();
        }

        private void Update()
        {
            int timeElapsed = (int)((Time.time - _startTime) * gameSettingsConfig.LevelStartCountdownSpeed);
            _currentTime = gameSettingsConfig.LevelStartSeconds - timeElapsed;
            //if (_currentTime < _displayedTime && _currentTime == 0)
            //{
            //    _displayedTime = _currentTime;
            //    SetDisplayToStart();
            //}
            if (_currentTime < 0)
            {
                if (GameStateController.Instance.GameSettingsConfig.StartSound != null)
                {
                    GameStateController.Instance.AudioSource?.PlayOneShot(GameStateController.Instance.GameSettingsConfig.StartSound);
                }                
                LevelStart();
            }
            else if (_currentTime < _displayedTime)
            {
                if (GameStateController.Instance.GameSettingsConfig.CountdownSound != null)
                {
                    GameStateController.Instance.AudioSource?.PlayOneShot(GameStateController.Instance.GameSettingsConfig.CountdownSound);
                }                
                _displayedTime = _currentTime;
                SetDisplay();
            }
        }

        private void SetDisplay()
        {
            MessageDispatcher.Instance.DispatchMsg("SetTimer", 0f, _componentRepository.GetId(), "Hud Controller", _displayedTime);
        }

        //private void SetDisplayToStart()
        //{
        //    MessageDispatcher.Instance.DispatchMsg("SetTimer", 0f, _componentRepository.GetId(), "Hud Controller", StartText.ToString());
        //}

        private void LevelStart()
        {
            // Broadcast a level start to everybody listening
            MessageDispatcher.Instance.DispatchMsg("ShowTimer", 0f, _componentRepository.GetId(), "Hud Controller", false);
            IEntity levelStateId = _levelState.GetComponent<IEntity>();
            MessageDispatcher.Instance.DispatchMsg("LevelStart", 0f, _componentRepository.GetId(), levelStateId.GetId(), null);
            foreach (var go in gameObjectsToEnable)
            {
                go.SetActive(true);
            }
            GameObject.Destroy(gameObject);
        }
    }
}
