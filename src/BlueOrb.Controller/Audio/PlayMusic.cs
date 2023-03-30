using BlueOrb.Base.Manager;
using UnityEngine;

namespace BlueOrb.Controller.Audio
{
    [AddComponentMenu("BlueOrb/Audio/Play Music")]
    public class PlayMusic : MonoBehaviour
    {
        [SerializeField]
        private AudioClip musicClip;

        private void Awake()
        {
            GameStateController.Instance.PlayMusic(this.musicClip);
        }
    }
}