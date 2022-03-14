using BlueOrb.Base.VariableClasses;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Controller;
using BlueOrb.Controller.Inventory;
using BlueOrb.Controller.Player;
using BlueOrb.Controller.Scene;
using BlueOrb.Messaging;
using UnityEngine;
using System.Collections.Generic;
using BlueOrb.Controller.Damage;
using BlueOrb.Base.Config;
using BlueOrb.Controller.Manager;

namespace BlueOrb.Base.Manager
{
    // We are keeping this a MonoBehavior because it not only keeps this alive, but it keeps alive
    // everything within this gameObject, such as the AudioSource object so the sounds and music don't stop or skip
    // between scene loads
    /// <summary>
    /// Controls the state of the game as the player progresses through it.
    /// Scope: Entire game, from "Begin New Game" to when the player exits to the main screen.
    /// </summary>
    [AddComponentMenu("BlueOrb/Manager/Game State")]
    public class GameStateController : ComponentBase<GameStateController>
    {
        [SerializeField]
        private GameSettingsConfig gameSettingsConfig;
        public GameSettingsConfig GameSettingsConfig => gameSettingsConfig;

        //public SceneConfig NextSceneConfig { get; set; }
        //public string SpawnpointUniqueId { get; set; }
        //public SceneConfig CurrentSceneConfig { get; set; }

        /// <summary>
        /// The players current high scores in each level
        /// </summary>
        private Dictionary<string, int> _levelHighScore;

        //[SerializeField]
        //private Variables _globalVariables;
        //public Variables GlobalVariables => _globalVariables;
        // TODO Set this to true when the player starts a new game!
        //public bool BeginNewGame { get; set; }
        //public bool ChangingScene { get; set; }
        [SerializeField]
        private SceneController _sceneController;

        //private LevelStateController levelStateController;
        //public LevelStateController LevelStateController 
        //{ 
        //    get
        //    {
        //        if (levelStateController == null)
        //        {
        //            levelStateController = GameObject.FindObjectOfType<LevelStateController>();
        //        }
        //        return levelStateController;
        //    }
        //    set => levelStateController = value; 
        //}

        protected override void Awake()
        {
            base.Awake();
            _levelHighScore = new Dictionary<string, int>();
            //BeginNewGame = true;
        }

        public override string GetId()
        {
            return "Game State Controller";
        }

        public override void Init()
        {
            if (_hasInited)
                return;
            base.Init();

            //GlobalVariables.Init();
        }

        /// <summary>
        /// Singleton
        /// </summary>
        private static GameStateController _instance;
        [HideInInspector]
        public static GameStateController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameStateController>();
                    _instance.Init();
                }
                return _instance;
            }
        }

        /// <summary>
        /// The game start init method. The first SceneConfig must already be set since we load data from the first scene when debugging so we can start the game in any scene. Call this before the first in-game LoadScene function call.
        /// </summary>
        public void BeginNewGame()
        {
            ////GameStateController.Instance.NewGame();
            ////GameStateController.Instance.StartInit();
            ////MessageDispatcher2.Instance.DispatchMsg("SetGold", 0f, this.UniqueId, "UI Manager", GameDataController.Instance.Data.Inventory.Gold);
            ////var sceneConfig = GameDataController.Instance.NextSceneConfig;
            ////if (sceneConfig == null)
            ////    sceneConfig = GameDataController.Instance.CurrentSceneConfig;

            //MessageDispatcher2.Instance.DispatchMsg("SetHUDSkill", 0f, this.UniqueId, "UI Manager", null);
            // Populate starting items

            //Debug.Log("Populating " + sceneConfig.StartingItems.Length + " Starting Items");
            //if (SceneConfig.StartingItems != null)
            //{
            //    bool isFirst = true;
            //    //Debug.Log("Adding Starting Items to inventory.");
            //    foreach (var startingItem in SceneConfig.StartingItems)
            //    {
            //        if (startingItem.ItemConfig == null)
            //            continue;
            //        //var addItemData = new ItemAndQuantityData()
            //        //{
            //        //    ItemConfig = startingItem.Item as IItemConfig,
            //        //    Quantity = startingItem.Quantity
            //        //};
            //        MessageDispatcher.Instance.DispatchMsg("AddItem", 0f, this.GetInstanceID().ToString(), _player.GetId(),
            //            startingItem);
            //        //if (isFirst)
            //        //{
            //        //    GameDataController.Instance.Data.SelectedSkill = startingItem.Item.UniqueId;
            //        //    MessageDispatcher2.Instance.DispatchMsg("SetHUDSkill", 0f, UniqueId, "UI Manager", startingItem.Item.UniqueId);
            //        //    isFirst = false;
            //        //}
            //    }

            //    //var molds = SceneConfig.StartingItems.Where(i => i.ItemConfig.ItemType == ItemTypeEnum.Mold).ToList();
            //    //var shards = SceneConfig.StartingItems.Where(i => i.ItemConfig.ItemType == ItemTypeEnum.Shard).ToList();

            //    //if (shards.Count != 0)
            //    //    GameStateController.Instance.CurrentShard = (ShardConfig)shards[0].ItemConfig;

            //    //MessageDispatcher.Instance.DispatchMsg("SetMoldData", 0f, string.Empty, "Hud Controller", molds);
            //    //MessageDispatcher.Instance.DispatchMsg("SetShardData", 0f, string.Empty, "Hud Controller", shards);

            //    //if (molds.Count > 0)
            //    //{
            //    //    Debug.Log("Setting Mold to " + molds[0].ItemConfig.name);
            //    //    GameStateController.Instance.CurrentMold = molds[0].ItemConfig as MoldConfig;
            //    //}
            //}
        }

        //public void ToggleMold(bool isDown)
        //{
        //    var player = EntityContainer.Instance.GetMainCharacter();
        //    //var playerController = player.Components.GetComponent<PlayerController>();
        //    var inventoryController = player.Components.GetComponent<InventoryComponent>();
        //    //inventoryController.
        //}



        public void LoadScene(string sceneName)
        {
            //ChangingScene = true;
            // Log the next Spawnpoint before ClearScene deletes it
            //SpawnpointUniqueId = spawnPointId;
            //if (GameDataController.Instance.Data != null)
            //{
            //    GameDataController.Instance.Data.SpawnpointUniqueId =
            //        GameDataController.Instance.Data.NextSpawnpointUniqueId;
            //}
            //ClearScene(false);

            //MessageDispatcher.Instance.RemoveByEarlyTermination(Enums.TelegramEarlyTermination.ChangeScenes);
            //Debug.Log("LoadLevel being called");
            //Debug.Log("Loading scene, entity count = " + EntityContainer._instance.EntityInstanceMap.Count);
            //Application.LoadLevel(sceneName);

            _sceneController.FadeAndLoadScene(sceneName);
            //Debug.Log("(GameStateController) FadeAndLoadScene called");


        }

        public void LoadScene(string sceneName, string spawnPointId)
        {
            LoadScene(sceneName);
            //ChangingScene = true;
            // Log the next Spawnpoint before ClearScene deletes it
            //SpawnpointUniqueId = spawnPointId;
            //if (GameDataController.Instance.Data != null)
            //{
            //    GameDataController.Instance.Data.SpawnpointUniqueId =
            //        GameDataController.Instance.Data.NextSpawnpointUniqueId;
            //}
            //ClearScene(false);

            //MessageDispatcher.Instance.RemoveByEarlyTermination(Enums.TelegramEarlyTermination.ChangeScenes);
            //Debug.Log("LoadLevel being called");
            //Debug.Log("Loading scene, entity count = " + EntityContainer._instance.EntityInstanceMap.Count);
            //Application.LoadLevel(sceneName);

            //_sceneController.FadeAndLoadScene(sceneName);
            //Debug.Log("(GameStateController) FadeAndLoadScene called");


        }
    }
}
