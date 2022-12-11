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

        [SerializeField]
        private float maxDistanceFromPlayer = 300f;
        private float maxDistanceFromPlayerSquared;
        [SerializeField]
        private float FovCheckDegrees = 60f;

        //private float? lifeEndTime;

        private float nextSpawnTime;

        protected override void Awake()
        {
            base.Awake();
            maxDistanceFromPlayerSquared = maxDistanceFromPlayer * maxDistanceFromPlayer;
        }

        private void Start()
        {
            //StartCoroutine(Check());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(otherTag))
            {
                return;
            }
            this.nextSpawnTime = Time.time + this.initialDelay;
            this.gameObject.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(otherTag))
            {
                return;
            }
            this.gameObject.SetActive(false);
        }

        private void OnUpdate()
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
            GameObject toInstantiate = enemyGroupForLap[0].GetRandom(0);
            int spawnRnd = UnityEngine.Random.Range(0, this.spawnPoints.Length);
            GameObject.Instantiate(toInstantiate, this.spawnPoints[spawnRnd].transform.position, Quaternion.identity);
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