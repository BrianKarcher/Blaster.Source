using BlueOrb.Base.Config;
using UnityEngine;
using BlueOrb.Base.Item;
using System.Collections.Generic;

namespace BlueOrb.Controller.Scene
{
    public class SceneConfig : BaseConfig
    {
        public float MaxHp = 1;
        /// <summary>
        /// Pointer to the Scene file (.unity)
        /// </summary>
        public UnityEngine.Object Scene;

        [HideInInspector]
        public string SceneName;

        [SerializeField]
        public ItemDesc[] StartingItems;

        [SerializeField]
        public List<SpawnPointConfig> SpawnPoints;
    }
}
