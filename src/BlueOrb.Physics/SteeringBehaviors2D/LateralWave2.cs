using BlueOrb.Base.Extensions;
using UnityEngine;

namespace BlueOrb.Physics.SteeringBehaviors2D
{
    public class LateralWave2 : SteeringBehaviorBase2
    {
        public LateralWave2(SteeringBehaviorManager manager)
            : base(manager)
        {
            base.SetWeight(manager.SteeringData.LateralWaveWeight);
        }

        protected override Vector2 CalculateForce()
            => SteeringBehaviorCalculations2.LateralWave(_steeringBehaviorManager.Entity);
    }
}