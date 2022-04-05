using BlueOrb.Base.Extensions;
using BlueOrb.Physics.SteeringBehaviors3D;
using UnityEngine;

namespace BlueOrb.Physics.SteeringBehaviors2D
{
    public class LateralWave3 : SteeringBehaviorBase3
    {
        public LateralWave3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants2.SeekWeight;
        }

        protected override Vector3 CalculateForce()
            => SteeringBehaviorCalculations3.LateralWave(SteeringBehaviorManager.Entity);
    }
}