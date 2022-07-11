using System;

namespace BlueOrb.Controller.Persistence
{
    [Serializable]
    public class SettingsData
    {
        public string LootLockerDeviceId { get; set; }
        public string LootLockerMemberId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public float SoundEffectVolume { get; set; }
        public float MusicVolume { get; set; }
        public float Sensitivity { get; set; }
    }
}