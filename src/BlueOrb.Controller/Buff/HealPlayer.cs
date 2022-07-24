using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Controller.Buff
{
    [AddComponentMenu("BlueOrb/Buff/HealPlayer")]
    public class HealPlayer : MonoBehaviour
    {
        private void Start()
        {
            EntityContainer.Instance.LevelStateController.SetCurrentHp(EntityContainer.Instance.LevelStateController.GetMaxHp());
        }

        private void Update()
        {
            GameObject.Destroy(gameObject);
        }
    }
}