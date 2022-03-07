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
        private int _seconds;
        [SerializeField]
        private bool _immediate = false;
        [SerializeField]
        private List<GameObject> gameObjectsToEnable;
        [SerializeField]
        private GameObject _levelState;

        private int _currentTime;
        private float _startTime;
        private int _displayedTime;

        protected override void Awake()
        {
            base.Awake();
            _currentTime = _seconds;
            _displayedTime = _seconds;
            _startTime = Time.time;
        }

        private void Start()
        {
            if (_immediate)
            {
                LevelStart();
                return;
            }
            MessageDispatcher.Instance.DispatchMsg("ShowTimer", 0f, _componentRepository.GetId(), "Hud Controller", true);
            SetDisplay();
        }

        private void Update()
        {
            int timeElapsed = (int)(Time.time - _startTime);
            _currentTime = _seconds - timeElapsed;
            if (_currentTime <= 0)
            {
                LevelStart();
            }
            else if (_currentTime < _displayedTime)
            {
                _displayedTime = _currentTime;
                SetDisplay();
            }
        }

        private void SetDisplay()
        {
            MessageDispatcher.Instance.DispatchMsg("SetTimer", 0f, _componentRepository.GetId(), "Hud Controller", _displayedTime);
        }

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
