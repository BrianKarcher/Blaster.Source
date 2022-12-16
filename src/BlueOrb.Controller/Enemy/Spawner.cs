using BlueOrb.Base.Extensions;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using System;
using System.Collections;
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
        private float nextSpawnTime;
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

        private void Start()
        {
            this.enabled = false;
        }

        public override void StartListening()
        {
            base.StartListening();
            MessageDispatcher.Instance.StartListening(spawnEvent, _componentRepository.GetId(), (data) =>
            {
                this.enabled = true;
                this.nextSpawnTime = Time.time + this.initialDelay + UnityEngine.Random.Range(minDelay, maxDelay);

            });
        }

        public override void StopListening()
        {
            base.StopListening();
        }

        private void Update()
        {
            if (Time.time < this.nextSpawnTime || !GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                return;
            }
            if (this.enemyVariantConfigs.Length == 0)
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
            EnemyVariantConfig enemyVariantConfig = enemyVariantConfigs[UnityEngine.Random.Range(0, this.enemyVariantConfigs.Length)];
            GameObject toInstantiate = enemyVariantConfig.GetVariant(0);
            //int spawnRnd = UnityEngine.Random.Range(0, this.spawnPoints.Length);
            // TODO : Do a check if another entity is in this position. If there is, do NOT instantiate.
            GameObject.Instantiate(toInstantiate, this.transform.position, Quaternion.identity);
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