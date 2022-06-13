using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Controller.Persistence;
using UnityEngine;
using UnityEngine.Audio;

namespace BlueOrb.Controller.Manager
{
    [AddComponentMenu("BlueOrb/Components/Settings Controller 2")]
    public class SettingsController2 : ComponentBase<SettingsController2>
    {
        [SerializeField]
        private GameStateController gameState;

        [SerializeField]
        private AudioMixer masterMixer;
        [SerializeField]
        private string musicAudioParameter;
        [SerializeField]
        private string effectAudioParameter;
        private SettingsData settingsData;

        protected override void Awake()
        {
            base.Awake();
            LoadSettings();
            if (this.settingsData == null)
            {
                this.settingsData = new SettingsData
                {
                    MusicVolume = 0, // 0 is 100% volume, -80 is mute
                    SoundEffectVolume = 0,
                    Sensitivity = 1f
                };
            }

            SetMusicVolume(this.settingsData.MusicVolume);
            SetEffectVolume(this.settingsData.SoundEffectVolume);
        }

        public void SaveSettings() => this.gameState.PersistenceController.Save(this.gameState.PersistenceController.DataFileName, this.settingsData);

        public SettingsData LoadSettings() => this.settingsData = this.gameState.PersistenceController.Load<SettingsData>(this.gameState.PersistenceController.DataFileName);

        public void SetMusicVolume(float volume)
        {
            Debug.Log($"Setting Music Volume to {volume}");
            this.settingsData.MusicVolume = volume;
            this.masterMixer.SetFloat(musicAudioParameter, volume);
        }

        public float GetMusicVolume() => this.settingsData.MusicVolume;

        public void SetEffectVolume(float volume)
        {
            Debug.Log($"Setting Sfx Volume to {volume}");
            this.settingsData.SoundEffectVolume = volume;
            this.masterMixer.SetFloat(effectAudioParameter, volume);
        }

        public float GetEffectVolume() => this.settingsData.SoundEffectVolume;

        public void SetSensitivity(float value)
        {
            Debug.Log($"Setting sensitivity to {value}");
            this.settingsData.Sensitivity = value;
        }

        public float GetSensitivity() => this.settingsData.Sensitivity;
    }
}