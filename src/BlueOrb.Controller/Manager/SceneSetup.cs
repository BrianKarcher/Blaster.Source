using BlueOrb.Base.Item;
using BlueOrb.Base.Manager;
using BlueOrb.Base.SpawnPoint;
using BlueOrb.Common.Container;
using BlueOrb.Controller.Scene;
using BlueOrb.Messaging;
using System;
using System.Linq;
using UnityEngine;

namespace BlueOrb.Controller.Manager
{
    [AddComponentMenu("RQ/Common/Scene Setup")]
    public class SceneSetup : MonoBehaviour
    {
        public SceneConfig SceneConfig;
        [SerializeField]
        private GameObject SpawnPoints;

        [SerializeField]
        private GameObject _actorsRoot;

        [SerializeField]
        private bool _startCountdownImmediately = false;

        public bool StartCoundownImmediately => _startCountdownImmediately;

        //private Cinemachine.

        private SpawnPointComponent[] _spawnPointComponents;

        private IEntity _player;
        //private LevelStateController _levelStateController;

        //private void Start()
        //{
        //    //_levelStateController = GameObject.Find<LevelStateController>();
        //    //InitAllEntities();
            
        //}

        private void Start()
        {
            _player = EntityContainer.Instance.GetMainCharacter();
            Debug.Log("Scene Setup Starting");
            //if (SceneConfig != null)
            //{
            //    //EntityContainer.Instance.LevelStateController.SetMaxHp(SceneConfig.MaxHp);

            //    MessageDispatcher.Instance.DispatchMsg("SetMaxHp", 0f, string.Empty, EntityContainer.Instance.GetMainCharacter().GetId(), SceneConfig.MaxHp);
            //    MessageDispatcher.Instance.DispatchMsg("SetCurrentHp", 0f, string.Empty, EntityContainer.Instance.GetMainCharacter().GetId(), SceneConfig.MaxHp);
            //}
            
            //if (GameStateController.Instance.BeginNewGame)
            //{
            //    BeginNewGame();
            //    // Set this variable in a FSM state... maybe the beginning of Play mode?
            //    GameStateController.Instance.BeginNewGame = false;
            //}
            //PlacePlayerAtSpawnPoint();
            //if (_startCountdownImmediately)
            //{
            //    StartCountdown();
            //}
        }

        //private void StartCountdown()
        //{
        //    MessageDispatcher.Instance.DispatchMsg("StartCountdown", 0f, string.Empty, "Level Controller", null);
        //}

        public void InitAllEntities()
        {
            //Debug.Log(this.name + " - InitAllEntities called");
            //var actorsRoot = GameController.Instance.GetSceneSetup().GetActorsRoot();
            //var entities = _actorsRoot.GetComponentsInChildren<ComponentRepository>(true);
            //foreach (var entity in entities)
            //{
            //    try
            //    {
            //        //if (!entity.isActiveAndEnabled)
            //        entity.Init();
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.LogError(ex);
            //    }
            //}
        }

        //private void PlacePlayerAtSpawnPoint()
        //{
        //    if (!String.IsNullOrEmpty(GameStateController.Instance.SpawnpointUniqueId))
        //    {
        //        _spawnPointComponents = GetSpawnPoints();
        //        //var sceneSetup = GameController.Instance.GetSceneSetup();

        //        //for (int i = 0; i < sceneSetup.SpawnPointComponents.Count; i++)
        //        //{

        //        //}
        //        //var spawnPoints = sceneSetup.SpawnPointComponents.Where(i =>
        //        //i.SpawnPointUniqueId == GameDataController.Instance.Data.SpawnpointUniqueId);

        //        PlaceEntityAtSpawnPoint(_spawnPointComponents);
        //        //PlaceEntityAtSpawnPoint(_spawnPointComponents, Enums.EntityType.Companion);
        //        GameStateController.Instance.SpawnpointUniqueId = null;
        //    }
        //}

        //private void PlaceEntityAtSpawnPoint(SpawnPointComponent[] spawnPoints)
        //{
        //    SpawnPointComponent spawnPoint = null;

        //    for (int i = 0; i < spawnPoints.Length; i++)
        //    {
        //        if (spawnPoints[i].SpawnPointUniqueId == GameStateController.Instance.SpawnpointUniqueId)
        //        {
        //            spawnPoint = spawnPoints[i];
        //            Debug.Log($"Located spawn point {spawnPoint.SpawnPointUniqueId}");
        //            break;
        //        }
        //    }

        //    //var spawnPoint = spawnPoints.FirstOrDefault(i => i.EntityType == entityType);
        //    if (spawnPoint == null)
        //    {
        //        Debug.LogError($"(PlaceEntityAtSpawnPoint) Could not locate spawn point {GameStateController.Instance.SpawnpointUniqueId}.");
        //        return;
        //    }

        //    var pos = spawnPoint.transform.position;

        //    var player = EntityContainer.Instance.GetMainCharacter();
        //    if (player == null)
        //    {
        //        Debug.LogWarning($"(PlaceEntityAtSpawnPoint) Could not locate player.");
        //        return;
        //    }

        //    player.transform.position = pos;

        //    //string uniqueId = player;

        //    // Place the main character at the spawn point
        //    //var spawnPoint = GameData.Instance.CurrentScene.SpawnPoints[GameData.Instance.SpawnpointUniqueId];
        //    //MessageDispatcher.Instance.DispatchMsg(0f, this.UniqueId,
        //    //    uniqueId, Enums.Telegrams.SetPos,
        //    //    new Vector2D(pos.x, pos.y));
        //    //MessageDispatcher.Instance.DispatchMsg(0f, this.UniqueId, uniqueId,
        //    //    Enums.Telegrams.SetLevelHeight, spawnPoint.LevelLayer);


        //}

        //public SpawnPointComponent[] GetSpawnPoints()
        //{
        //    if (SpawnPoints == null)
        //        return null;

        //    return SpawnPoints.GetComponentsInChildren<SpawnPointComponent>();

        //    //if (spawnPointComponents != null)
        //    //{
        //    //    SpawnPointComponents.AddRange(spawnPointComponents);
        //    //}
        //}
    }
}
