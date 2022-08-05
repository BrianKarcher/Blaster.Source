using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Controller.Buff
{
    [AddComponentMenu("BlueOrb/Buff/HealPlayerBuff")]
    public class HealPlayerBuff : MonoBehaviour
    {
        private void Start()
        {
            GameStateController.Instance.LevelStateController.SetCurrentHp(GameStateController.Instance.LevelStateController.GetMaxHp());
        }

        private void Update()
        {
            GameObject.Destroy(gameObject);
        }
    }
}