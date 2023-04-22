using BlueOrb.Base.Extensions;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    [AddComponentMenu("Blue Orb/Components/Spawner")]
    public class Spawner : ComponentBase<Spawner>
    {
        [SerializeField]
        private float initialDelay = 0f;
        [SerializeField]
        private float minDelay = 0f;
        [SerializeField]
        private float maxDelay = 0.2f;
        [SerializeField]
        private string spawnEvent = "SpawnEnemy";
        [SerializeField]
        private EnemyVariantConfig[] enemyVariantConfigs;
        [SerializeField]
        private GameObject spawnGameObject;
        [SerializeField]
        private EnemyGroupConfig spawnFromEnemyGroup;
        [SerializeField]
        private bool allLaps = true;
        [SerializeField]
        private int[] laps;
        [SerializeField]
        private bool facePlayer = true;

        private float nextSpawnTime;
        private long spawnEventId;
        private HashSet<int> lapsHash = new HashSet<int>();
        //[SerializeField]
        //private int entitiesToCreate = 10;
        //[SerializeField]
        //private GameObject[] spawnPoints;
        //[SerializeField]
        //private string otherTag = "Cart";
        //[SerializeField]
        //private string enemyTag = "Enemy";
        //[SerializeField]
        //private EnemyGroupConfig[] enemyGroupForLap;

        //protected override void Awake()
        //{
        //    base.Awake();
        //}

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < this.laps.Length; i++)
            {
                this.lapsHash.Add(this.laps[i]);
            }
        }

        private void Start()
        {
            this.enabled = false;
        }

        public override void StartListening()
        {
            base.StartListening();
            this.spawnEventId = MessageDispatcher.Instance.StartListening(spawnEvent, _componentRepository.GetId(), (data) =>
            {
                // TODO Implement laps
                Spawn(0);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(spawnEvent, _componentRepository.GetId(), this.spawnEventId);
        }

        public void Spawn(int lap)
        {
            if (!this.allLaps && !this.lapsHash.Contains(lap))
            {
                return;
            }
            this.enabled = true;
            this.nextSpawnTime = Time.time + this.initialDelay + UnityEngine.Random.Range(minDelay, maxDelay);
        }

        private void Update()
        {
            if (Time.time < this.nextSpawnTime || !GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                return;
            }
            //if (enemyGroupForLap.Length == 0)
            //{
            //    Debug.LogError($"No enemies to instantiate");
            //    return;
            //}
            //if (this.spawnPoints.Length == 0)
            //{
            //    Debug.LogError($"No spawn points for WaveSpawner");
            //    return;
            //}
            Quaternion quaternion = this.facePlayer ? Quaternion.LookRotation(EntityContainer.Instance.GetMainCharacter().transform.position
                - this.transform.position) : Quaternion.identity;
            if (this.enemyVariantConfigs.Length != 0)
            {
                EnemyVariantConfig enemyVariantConfig = enemyVariantConfigs[UnityEngine.Random.Range(0, this.enemyVariantConfigs.Length)];
                GameObject toInstantiate = enemyVariantConfig.GetVariant(0);
                //int spawnRnd = UnityEngine.Random.Range(0, this.spawnPoints.Length);
                // TODO : Do a check if another entity is in this position. If there is, do NOT instantiate.
                GameObject.Instantiate(toInstantiate, this.transform.position, quaternion);
            }
            if (this.spawnGameObject != null)
            {
                // TODO : Do a check if another entity is in this position. If there is, do NOT instantiate.
                GameObject.Instantiate(this.spawnGameObject, this.transform.position, quaternion);
            }
            if (spawnFromEnemyGroup != null)
            {
                // TODO Supply lap
                GameObject go = this.spawnFromEnemyGroup.GetRandom(0);
                // TODO : Do a check if another entity is in this position. If there is, do NOT instantiate.
                GameObject.Instantiate(go, this.transform.position, quaternion);
            }
            //entityCreatedCount++;
            //if (entityCreatedCount >= this.entitiesToCreate)
            //{
            //    this.enabled = false;
            //    return;
            //}
            //this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.minDelay, this.maxDelay);
            this.enabled = false;
        }
    }
}