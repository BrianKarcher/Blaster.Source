using BlueOrb.Base.Extensions;
using UnityEngine;

namespace BlueOrb.Physics.SteeringBehaviors2D
{
    public class LateralWave2 : SteeringBehaviorBase2
    {
        private float time;

        public LateralWave2(SteeringBehaviorManager manager)
            : base(manager)
        {
            base.SetWeight(manager.SteeringData.LateralWaveWeight);
            time = 0f;
        }

        protected override Vector2 CalculateForce()
        {
            time += Time.deltaTime;
            return SteeringBehaviorCalculations2.LateralWave(_steeringBehaviorManager.Entity, time);
        }
    }
}