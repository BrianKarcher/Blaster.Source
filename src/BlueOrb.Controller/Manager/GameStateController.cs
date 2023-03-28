using BlueOrb.Common.Components;
using BlueOrb.Controller;
using UnityEngine;
using System.Collections.Generic;
using BlueOrb.Base.Config;
using BlueOrb.Controller.Persistence;
using BlueOrb.Controller.Manager;
using BlueOrb.Base.Global;
using BlueOrb.Common.Container;
using BlueOrb.Controller.UI;
using BlueOrb.Base.Interfaces;

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

        [SerializeField]
        private PersistenceController persistenceController;
        public PersistenceController PersistenceController => persistenceController;

        [SerializeField]
        private SettingsController2 settingsController;
        public SettingsController2 SettingsController => settingsController;

        /// <summary>
        /// The players current high scores in each level
        /// </summary>
        private Dictionary<string, int> levelHighScores;

        [SerializeField]
        private SceneController _sceneController;

        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioSource musicAudioSource;
        public AudioSource MusicAudioSource => musicAudioSource;

        public AudioSource AudioSource => audioSource;

        [SerializeField]
        private GameObject goUIController;

        private IUIController uiController;
        public IUIController UIController => uiController;

        private ILevelStateController _levelStateController;

        public ILevelStateController LevelStateController { get => _levelStateController; set => _levelStateController = value; }

        protected override void Awake()
        {
            base.Awake();
            this.uiController = goUIController.GetComponent<IUIController>();
            LoadGame();
        }

        public override string GetId() => "Game State Controller";

        public override void Init()
        {
            if (_hasInited)
                return;
            base.Init();
        }

        public void SaveGame()
        {
            PersistUserScores persistUserScores = new PersistUserScores
            {
                HighScores = levelHighScores
            };

            this.persistenceController.Save(persistenceController.SaveFileName, persistUserScores);
        }

        public void LoadGame()
        {
            PersistUserScores persistData = this.persistenceController.Load<PersistUserScores>(persistenceController.SaveFileName);
            if (persistData == null)
            {
                this.levelHighScores = new Dictionary<string, int>();
                return;
            }
            this.levelHighScores = persistData.HighScores;
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

        public void LoadScene(string sceneName, bool fade)
        {
            EntityContainer.Instance.ResetEntityList();
            _sceneController.FadeAndLoadScene(sceneName, fade);
        }

        public void LoadScene(string sceneName, string spawnPointId, bool fade)
        {
            LoadScene(sceneName, fade);
        }

        public bool IsLoadingScene => this._sceneController.IsLoadingScene;

        public bool EnterHighScore(string level, int levelScore)
        {
            this.levelHighScores.TryGetValue(level, out int highScore);
            if (levelScore > highScore)
            {
                highScore = levelScore;
                this.levelHighScores[level] = highScore;
                GlobalStatic.NewHighScore = true;
                SaveGame();
                Debug.Log("New PB!");
                return true;
            }
            return false;
        }

        public int GetHighScore(string uniqueId)
        {
            int score;
            levelHighScores.TryGetValue(uniqueId, out score);
            return score;
        }
    }
}