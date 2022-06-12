using System;

namespace BlueOrb.Controller.Persistence
{
    [Serializable]
    public class SettingsData
    {
        public float SoundEffectVolume { get; set; }
        public float MusicVolume { get; set; }
    }
}