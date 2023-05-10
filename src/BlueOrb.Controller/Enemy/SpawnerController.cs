using BlueOrb.Base.Extensions;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    [AddComponentMenu("Blue Orb/Components/Spawner Controller")]
    public class SpawnerController : ComponentBase<SpawnerController>
    {
        [SerializeField]
        private float initialDelay = 0f;
        [SerializeField]
        private float minDelay = 1.5f;
        [SerializeField]
        private float maxDelay = 3.0f;
        //[SerializeField]
        //private int entitiesToCreate = 10;
        [SerializeField]
        private Spawner[] spawnPoints;
        [SerializeField]
        private string otherTag = "Cart";
        //[SerializeField]
        //private string enemyTag = "Enemy";
        //[SerializeField]
        //private EnemyGroupConfig[] enemyGroupForLap;

        private float nextSpawnTime;
        //private int entityCreatedCount = 0;
        private bool active = false;
        private float awakeTime = 0f;

        protected override void Awake()
        {
            base.Awake();
            if (spawnPoints.Length == 0)
            {
                this.spawnPoints = transform.GetComponentsInChildren<Spawner>();
            }
        }

        private void Start()
        {
            this.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError($"Wave Spawner: Collided with {other.tag}");
            if (!other.CompareTag(otherTag))
            {
                return;
            }
            if (Time.time < this.awakeTime)
            {
                return;
            }
            this.nextSpawnTime = Time.time + this.initialDelay;
            //this.entityCreatedCount = 0;
            this.active = true;
            // Sleep for 20 seconds.
            this.awakeTime = Time.time + 20;
            //this.enabled = true;
            //Debug.LogError($"SpawnerController set to Active");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(otherTag))
            {
                return;
            }
            //this.enabled = false;
            //Debug.LogError($"SpawnerController set to Deactive");
        }

        private void Update()
        {
            if (Time.time < this.nextSpawnTime || !GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                return;
            }
            if (!this.active)
            {
                return;
            }
            Debug.Log($"{this.name} is Spawning enemies!");
            //if (enemyGroupForLap.Length == 0)
            //{
            //    Debug.LogError($"No enemies to instantiate");
            //    return;
            //}
            if (this.spawnPoints.Length == 0)
            {
                Debug.LogError($"No spawn points for SpawnerController");
                return;
            }
            for (int i = 0; i < this.spawnPoints.Length; i++)
            {
                this.spawnPoints[i].Spawn(GameStateController.Instance.Lap);
            }
            //GameObject toInstantiate = enemyGroupForLap[0].GetRandom(0);
            //int spawnRnd = UnityEngine.Random.Range(0, this.spawnPoints.Length);
            // TODO : Do a check if another entity is in this position. If there is, do NOT instantiate.
            //GameObject.Instantiate(toInstantiate, this.spawnPoints[spawnRnd].position, Quaternion.identity);
            //entityCreatedCount++;
            //if (entityCreatedCount >= this.entitiesToCreate)
            //{
            //    this.enabled = false;
            //    return;
            //}
            //this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.minDelay, this.maxDelay);
            this.active = false;
            //this.enabled = false;
        }
    }
}