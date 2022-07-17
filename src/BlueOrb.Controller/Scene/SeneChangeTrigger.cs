using BlueOrb.Base.Attributes;
using BlueOrb.Base.Global;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Manager;
using BlueOrb.Controller.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BlueOrb.Controller
{
    [AddComponentMenu("BlueOrb/Components/Scene Change Trigger")]
    public class SceneChangeTrigger : MonoBehaviour
    {
        /// <summary>
        /// Scene config to change to.
        /// </summary>
        [SerializeField]
        private SceneConfig _sceneConfig;
        public SceneConfig SceneConfig { get => _sceneConfig; set => _sceneConfig = value; }

        [SerializeField]
        private string _spawnPointUniqueId;
        public string SpawnPointUniqueId { get => _spawnPointUniqueId; set => _spawnPointUniqueId = value; }

        [Tag]
        [SerializeField]
        private string _tag;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag != _tag)
                return;
            Debug.Log($"Changing scenes to {_sceneConfig.SceneName} - {_spawnPointUniqueId}");
            GameStateController.Instance.LoadScene(_sceneConfig.SceneName, _spawnPointUniqueId, true);
        }

    }
}
