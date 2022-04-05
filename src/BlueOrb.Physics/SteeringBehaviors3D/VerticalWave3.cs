using BlueOrb.Physics.SteeringBehaviors3D;
using UnityEngine;

namespace BlueOrb.Physics.SteeringBehaviors2D
{
    public class VerticalWave3 : SteeringBehaviorBase3
    {
        public VerticalWave3(SteeringBehaviorManager manager)
            : base(manager)
        {
            ConstantWeight = Constants2.SeekWeight;
        }

        protected override Vector3 CalculateForce()
            => SteeringBehaviorCalculations3.LateralWave(SteeringBehaviorManager.Entity);
    }
}