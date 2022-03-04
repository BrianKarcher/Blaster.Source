using BlueOrb.Common.Components;
using BlueOrb.Controller.Manager;
using BlueOrb.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Controller
{
    [AddComponentMenu("BlueOrb/Manager/Level Start Timer")]
    public class LevelStartTimer : ComponentBase<LevelStartTimer>
    {
        [SerializeField]
        private float _seconds;
        [SerializeField]
        private bool _immediate = false;
        [SerializeField]
        private List<GameObject> gameObjectsToEnable;
        //[SerializeField]
        //private SceneSetup sceneSetup;

        private float _current;
        private int _displayTime;

        protected override void Awake()
        {
            base.Awake();
            _current = 0;
            _displayTime = 0;
        }

        private void Start()
        {
            if (_immediate)
            {
                LevelStart();
            }
        }

        private void Update()
        {
            _current += Time.deltaTime;
            if (_current > _seconds)
            {
                LevelStart();
            }
            else if ((int) _current > _displayTime)
            {
                _displayTime = (int)_current;
                MessageDispatcher.Instance.DispatchMsg("SetTimer", 0f, _componentRepository.GetId(), "Hud Controller", _displayTime);
            }
        }

        private void LevelStart()
        {
            //MessageDispatcher.Instance.DispatchMsg("LevelStart", 0f, _componentRepository.GetId(), "Hud Controller", _displayTime);

            // Broadcast a level start to everybody listening
            MessageDispatcher.Instance.DispatchMsg("LevelStart", 0f, _componentRepository.GetId(), _componentRepository.GetId(), null);
            foreach (var go in gameObjectsToEnable)
            {
                go.SetActive(true);
            }
            GameObject.Destroy(gameObject);
        }
    }
}
