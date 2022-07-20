using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Buff
{
    [AddComponentMenu("BlueOrb/Buff/HealPlayer")]
    public class HealPlayer : MonoBehaviour
    {
        [SerializeField]
        private string notificationMessage = "HEALED";

        private void Start()
        {
            EntityContainer.Instance.LevelStateController.SetCurrentHp(EntityContainer.Instance.LevelStateController.GetMaxHp());
            MessageDispatcher.Instance.DispatchMsg("Notification", 0f, null, "Hud Controller", notificationMessage);
            GameObject.Destroy(gameObject);
        }
    }
}