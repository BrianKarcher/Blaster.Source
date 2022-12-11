using BlueOrb.Base.Config;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    public class EnemyGroupConfig : BaseConfig
    {
        [SerializeField]
        private EnemyVariantConfig[] variants;

        public GameObject[] GetAll(int level)
        {
            GameObject[] group = new GameObject[variants.Length];
            for (int i = 0; i < group.Length; i++)
            {
                group[i] = variants[i].GetVariant(level);
            }
            return group;
        }

        public GameObject GetRandom(int level)
        {
            if (variants.Length == 0)
                return null;
            int rnd = Random.Range(0, variants.Length);
            return variants[rnd].GetVariant(level);
        }
    }
}