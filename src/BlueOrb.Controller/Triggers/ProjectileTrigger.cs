using BlueOrb.Base.Item;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Triggers
{
    public class ProjectileTrigger : ComponentBase<ProjectileTrigger>
    {
        [SerializeField]
        private ProjectileConfig[] projectiles;
        [SerializeField]
        private float hp = 1;
        [SerializeField]
        private GameObject notifyOnDeath;
        [SerializeField]
        private string notificationDeathMessage;
        [SerializeField]
        private AudioClip shotAudioClip;
        [SerializeField]
        private AudioClip deathAudioClip;
        private long[] projectileMessageIds;

        public override void StartListening()
        {
            base.StartListening();
            this.projectileMessageIds = new long[projectiles.Length];
            for (int i = 0; i < this.projectiles.Length; i++)
            {
                projectileMessageIds[i] = MessageDispatcher.Instance.StartListening(projectiles[i].Message,
                    _componentRepository.GetId(), (data) =>
                    {
                        hp--;
                        if (hp <= 0)
                        {
                            DeathNotification();
                            PlayDeathAudioClip();
                        }
                        else
                        {
                            PlayShotAudioClip();
                        }
                    });
            }
        }

        private void PlayShotAudioClip()
        {
            if (this.shotAudioClip == null)
                return;
            GameStateController.Instance.AudioSource.PlayOneShot(this.shotAudioClip);
        }

        private void PlayDeathAudioClip()
        {
            if (this.deathAudioClip == null)
                return;
            GameStateController.Instance.AudioSource.PlayOneShot(this.deathAudioClip);
        }

        private void DeathNotification()
        {
            if (notifyOnDeath == null)
                return;
            string deathId = notifyOnDeath?.GetComponent<IEntity>()?.GetId();
            if (deathId == null)
                return;
            MessageDispatcher.Instance.DispatchMsg(this.notificationDeathMessage, 0f, _componentRepository.GetId(),
                deathId, null);
        }

        public override void StopListening()
        {
            base.StopListening();
            for (int i = 0; i < this.projectiles.Length; i++)
            {
                MessageDispatcher.Instance.StopListening(this.projectiles[i].Message, _componentRepository.GetId(),
                    this.projectileMessageIds[i]);
            }
        }
    }
}