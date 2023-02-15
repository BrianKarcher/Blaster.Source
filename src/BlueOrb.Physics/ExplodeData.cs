using UnityEngine;

namespace BlueOrb.Physics
{
    public class ExplodeData
    {
        public Vector3 ExplodePosition { get; set; }
        public float Force { get; set; }
        public float Radius { get; set; }
        public float UpwardModifier { get; set; }
        public float Damage { get; set; }
        public GameObject ExplodingEntity { get; set; }
    }
}