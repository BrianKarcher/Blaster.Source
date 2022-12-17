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
    [AddComponentMenu("Blue Orb/Components/Wave Spawner")]
    public class WaveSpawnerController : ComponentBase<WaveSpawnerController>
    {
        [SerializeField]
        private float initialDelay = 0f;
        [SerializeField]
        private float minDelay = 1.5f;
        [SerializeField]
        private float maxDelay = 3.0f;
        [SerializeField]
        private int entitiesToCreate = 10;
        [SerializeField]
        private Transform[] spawnPoints;
        [SerializeField]
        private string otherTag = "Cart";
        [SerializeField]
        private string enemyTag = "Enemy";
        [SerializeField]
        private EnemyGroupConfig[] enemyGroupForLap;

        private float nextSpawnTime;
        private int entityCreatedCount = 0;

        protected override void Awake()
        {
            base.Awake();
            if (spawnPoints.Length == 0)
            {
                List<Transform> spawn = new List<Transform>();
                foreach (Transform child in transform)
                {
                    spawn.Add(child);
                }
                this.spawnPoints = spawn.ToArray();
            }
        }

        private void Start()
        {
            this.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError($"Wave Spawner: Collided with {other.tag}");
            if (!other.CompareTag(otherTag))
            {
                return;
            }
            this.nextSpawnTime = Time.time + this.initialDelay;
            this.entityCreatedCount = 0;
            this.enabled = true;
            Debug.LogError($"Wave Spawner set to Active");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(otherTag))
            {
                return;
            }
            this.enabled = false;
            Debug.LogError($"Wave Spawner set to Deactive");
        }

        private void Update()
        {
            if (Time.time < this.nextSpawnTime || !GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                return;
            }
            if (enemyGroupForLap.Length == 0)
            {
                Debug.LogError($"No enemies to instantiate");
                return;
            }
            if (this.spawnPoints.Length == 0)
            {
                Debug.LogError($"No spawn points for WaveSpawner");
                return;
            }
            GameObject toInstantiate = enemyGroupForLap[0].GetRandom(0);
            int spawnRnd = UnityEngine.Random.Range(0, this.spawnPoints.Length);
            // TODO : Do a check if another entity is in this position. If there is, do NOT instantiate.
            GameObject.Instantiate(toInstantiate, this.spawnPoints[spawnRnd].position, Quaternion.identity);
            entityCreatedCount++;
            if (entityCreatedCount >= this.entitiesToCreate)
            {
                this.enabled = false;
                return;
            }
            this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.minDelay, this.maxDelay);
        }
    }
}