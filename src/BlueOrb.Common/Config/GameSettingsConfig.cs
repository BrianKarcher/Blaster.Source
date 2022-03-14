using UnityEngine;

namespace BlueOrb.Base.Config
{
    public class GameSettingsConfig : BaseConfig
    {
        [SerializeField]
        private int levelStartSeconds = 3;
        public int LevelStartSeconds => levelStartSeconds;

        [SerializeField]
        private float levelStartCountdownSpeed = 1.0f;
        public float LevelStartCountdownSpeed => levelStartCountdownSpeed;

        [SerializeField]
        private bool immediateStartGame = false;
        public bool ImmediateStartGame => immediateStartGame;

        [SerializeField] 
        private AudioClip countdownSound;
        public AudioClip CountdownSound => countdownSound;

        [SerializeField] 
        private AudioClip startSound;
        public AudioClip StartSound => startSound;
    }
}
