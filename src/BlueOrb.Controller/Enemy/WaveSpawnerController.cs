using BlueOrb.Base.Extensions;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using System;
using System.Collections;
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
        private GameObject[] spawnPoints;
        [SerializeField]
        private string otherTag = "Cart";
        [SerializeField]
        private EnemyGroupConfig[] enemyGroupForLap;

        //private float? lifeEndTime;

        private float nextSpawnTime;
        private int entityCreatedCount = 0;

        protected override void Awake()
        {
            base.Awake();
            //this.gameObject.SetActive(false);
        }

        private void Start()
        {
            //StartCoroutine(Check());
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
            //this.gameObject.SetActive(true);
            this.enabled = true;
            Debug.LogError($"Wave Spawner set to Active");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(otherTag))
            {
                return;
            }
            //this.gameObject.SetActive(false);
            this.enabled = false;
            Debug.LogError($"Wave Spawner set to Deactive");
        }

        private void Update()
        {
            if (Time.time < this.nextSpawnTime)
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
            GameObject.Instantiate(toInstantiate, this.spawnPoints[spawnRnd].transform.position, Quaternion.identity);
            entityCreatedCount++;
            if (entityCreatedCount >= this.entitiesToCreate)
            {
                this.gameObject.SetActive(false);
                return;
            }
            this.nextSpawnTime = Time.time + UnityEngine.Random.Range(this.minDelay, this.maxDelay);
        }

        //private IEnumerator Check()
        //{
        //    while (true)
        //    {
        //        if (!GameStateController.Instance.LevelStateController.HasLevelBegun)
        //        {
        //            yield return new WaitForSeconds(1f);
        //        }
        //        if (lifeEndTime == null)
        //        {
        //            this.lifeEndTime = Time.time + lifetimeSeconds;
        //        }
        //        if (Time.time > this.lifeEndTime)
        //        {
        //            this.GetComponentRepository().Destroy();
        //            break;
        //        }
        //        // TODO Add a field of view check to this
        //        if (Vector2.SqrMagnitude(this.GetComponentRepository().GetFootPosition().xz() - EntityContainer.Instance.GetMainCharacter().GetFootPosition().xz()) > maxDistanceFromPlayerSquared)
        //        {
        //            float angle = Vector3.Angle(this.transform.forward, EntityContainer.Instance.GetMainCharacter().transform.forward);
        //            if (angle <= FovCheckDegrees)
        //            {
        //                yield return new WaitForSeconds(1f);
        //                continue;
        //            }
        //            this.GetComponentRepository().Destroy();
        //            break;
        //        }
        //        yield return new WaitForSeconds(1f);
        //    }
        //}
        }
}