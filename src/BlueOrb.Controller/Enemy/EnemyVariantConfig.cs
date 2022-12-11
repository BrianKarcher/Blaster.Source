using BlueOrb.Base.Config;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    public class EnemyVariantConfig : BaseConfig
    {
        [SerializeField]
        private GameObject[] variants;

        public GameObject GetVariant(int level)
        {
            if (variants == null || variants.Length == 0)
                return null;
            if (level > variants.Length - 1) 
                return variants[variants.Length - 1];
            if (level < 0)
                return variants[0];
            return variants[level];
        }
    }
}