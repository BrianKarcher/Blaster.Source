using BlueOrb.Base.Manager;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.UI
{
    [AddComponentMenu("BlueOrb/UI/Notification")]
    public class Notification : MonoBehaviour
    {
        [SerializeField]
        private string notificationMessage = "Notification";

        private void Start()
        {
            GameStateController.Instance.LevelStateController.SetCurrentHp(GameStateController.Instance.LevelStateController.GetMaxHp());
            MessageDispatcher.Instance.DispatchMsg("Notification", 0f, null, "Hud Controller", notificationMessage);
        }
    }
}