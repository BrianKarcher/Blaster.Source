using BlueOrb.Base.Config;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    public class EnemyGroupConfig : BaseConfig
    {
        [SerializeField]
        private EnemyVariantConfig[] variants;

        public GameObject[] GetWholeGroup(int level)
        {
            GameObject[] group = new GameObject[variants.Length];
            for (int i = 0; i < group.Length; i++)
            {
                group[i] = variants[i].GetVariant(level);
            }
            return group;
        }
    }
}