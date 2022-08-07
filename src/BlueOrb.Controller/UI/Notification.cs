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
            MessageDispatcher.Instance.DispatchMsg("Notification", 0f, null, "Hud Controller", notificationMessage);
        }
    }
}