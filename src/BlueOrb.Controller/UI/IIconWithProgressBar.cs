using UnityEngine;

namespace BlueOrb.Controller.UI
{
    public interface IIconWithProgressBar
    {
        void SetImage(Sprite image);
        void SetValue(float value);
    }
}
