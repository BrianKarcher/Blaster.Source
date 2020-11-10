using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Controller.Damage
{
    [AddComponentMenu("RQ/Components/Collision Component")]
    public class CollisionComponent : ComponentBase<CollisionComponent>
    {
        [SerializeField]
        private CollisionData _collisionData = null;
        public CollisionData CollisionData => _collisionData;

    }
}
