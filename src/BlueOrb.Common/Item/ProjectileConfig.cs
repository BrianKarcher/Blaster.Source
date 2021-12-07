using BlueOrb.Base.Config;
using UnityEngine;

namespace BlueOrb.Base.Item
{
    public class ProjectileConfig : ItemConfig
    {
        public float MaxSpeed = 200f;
        public int Ammo = 5;
        public bool IsSecondary = true;
    }
}
