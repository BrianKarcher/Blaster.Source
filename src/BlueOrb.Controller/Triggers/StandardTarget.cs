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
    public class StandardTarget : ComponentBase<StandardTarget>
    {
        [SerializeField]
        private bool allProjectiles = true;
        [SerializeField]
        private string allProjectileHitMessage = "ProjectileHit";
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
        [SerializeField]
        private Material deathMaterial;
        private long[] projectileMessageIds;
        private long allProjectileMessageId;

        public override void StartListening()
        {
            base.StartListening();
            if (this.allProjectiles)
            {
                this.allProjectileMessageId = MessageDispatcher.Instance.StartListening(allProjectileHitMessage,
                    _componentRepository.GetId(), MessageCallback);
            }
            this.projectileMessageIds = new long[projectiles.Length];
            for (int i = 0; i < this.projectiles.Length; i++)
            {
                projectileMessageIds[i] = MessageDispatcher.Instance.StartListening(projectiles[i].Message,
                    _componentRepository.GetId(), MessageCallback);
            }
        }

        public override void StopListening()
        {
            base.StopListening();
            if (this.allProjectiles)
            {
                MessageDispatcher.Instance.StopListening(this.allProjectileHitMessage, _componentRepository.GetId(),
                    this.allProjectileMessageId);
            }

            for (int i = 0; i < this.projectiles.Length; i++)
            {
                MessageDispatcher.Instance.StopListening(this.projectiles[i].Message, _componentRepository.GetId(),
                    this.projectileMessageIds[i]);
            }
        }

        private void MessageCallback(Telegram data)
        {
            hp--;
            if (hp <= 0)
            {
                DeathNotification();
                PlayDeathAudioClip();
                SetDeathMaterial();
            }
            else
            {
                PlayShotAudioClip();
            }
        }

        private void SetDeathMaterial()
        {
            Renderer mr = GetComponent<Renderer>();
            for (int i = 0; i < mr.materials.Length; i++)
            {
                mr.materials[i] = this.deathMaterial;
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
    }
}