using UnityEngine;

namespace BlueOrb.Controller.UI
{
    public interface IHudController
    {
        GameObject CreateBuffUI();
        void CreateAlert(string text);
        void CreateNotification(string text);
    }
}