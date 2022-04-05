using BlueOrb.Base.Extensions;
using UnityEngine;

namespace BlueOrb.Physics.SteeringBehaviors2D
{
    public class LateralWave2 : SteeringBehaviorBase2
    {
        public LateralWave2(SteeringBehaviorManager manager)
            : base(manager)
        {
            _constantWeight = Constants2.SeekWeight;
        }

        protected override Vector2 CalculateForce()
            => SteeringBehaviorCalculations2.LateralWave(_steeringBehaviorManager.Entity);
    }
}