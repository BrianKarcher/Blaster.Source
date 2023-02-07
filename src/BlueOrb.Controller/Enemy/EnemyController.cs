using BlueOrb.Base.Extensions;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using System.Collections;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    [AddComponentMenu("Blue Orb/Components/Enemy Controller")]
    public class EnemyController : ComponentBase<EnemyController>
    {
        [SerializeField]
        private float lifetimeSeconds = 60f;

        [SerializeField]
        private float maxDistanceFromPlayer = 300f;
        private float maxDistanceFromPlayerSquared;
        [SerializeField]
        private float FovCheckDegrees = 60f;

        private float? lifeEndTime;

        protected override void Awake()
        {
            base.Awake();
            maxDistanceFromPlayerSquared = maxDistanceFromPlayer * maxDistanceFromPlayer;
        }

        private void Start() => StartCoroutine(Check());

        private IEnumerator Check()
        {
            while (true)
            {
                if (!GameStateController.Instance.LevelStateController.HasLevelBegun)
                {
                    yield return new WaitForSeconds(1f);
                }
                if (lifeEndTime == null)
                {
                    this.lifeEndTime = Time.time + lifetimeSeconds;
                }
                if (Time.time > this.lifeEndTime)
                {
                    this.GetComponentRepository().Destroy();
                    break;
                }
                // TODO Add a field of view check to this
                if (Vector2.SqrMagnitude(this.GetComponentRepository().GetFootPosition().xz() - EntityContainer.Instance.GetMainCharacter().GetFootPosition().xz()) > maxDistanceFromPlayerSquared)
                {
                    float angle = Vector3.Angle(this.transform.forward, EntityContainer.Instance.GetMainCharacter().transform.forward);
                    if (angle <= FovCheckDegrees)
                    {
                        yield return new WaitForSeconds(1f);
                        continue;
                    }
                    this.GetComponentRepository().Destroy();
                    break;
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}