using BlueOrb.Base.Config;
using BlueOrb.Base.Item;
using UnityEngine;

namespace BlueOrb.Base.Skill
{
    public class ShardConfig : ItemConfig
    {
        [SerializeField]
        private GameObject _muzzleFlash;
        public GameObject MuzzleFlash => _muzzleFlash;
    }
}
