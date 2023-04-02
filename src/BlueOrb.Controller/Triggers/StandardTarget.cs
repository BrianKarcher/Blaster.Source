using BlueOrb.Base.Item;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Triggers
{
    public class StandardTarget : ComponentBase<StandardTarget>
    {
        [SerializeField]
        private bool startEnabled = true;
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
        private GameObject deathVfx;
        [SerializeField]
        private GameObject deathVfxSpawnPoint;
        [SerializeField]
        private AudioClip shotAudioClip;
        [SerializeField]
        private AudioClip deathAudioClip;
        [SerializeField]
        private Material enableMaterial;
        [SerializeField]
        private Material disabledMaterial;
        [SerializeField]
        private string enableMessage = "Enable";
        [SerializeField]
        private string disableMessage = "Disable";
        [SerializeField]
        private string destroyMessage = "Destroy";
        [SerializeField]
        private string reviveMessage = "Revive";

        private long[] projectileMessageIds;
        private long allProjectileMessageId, enableMessageId, disableMessageId, reviveMessageId, destroyMessageId;
        private Collider collider;
        private float currentHp;

        protected override void Awake()
        {
            base.Awake();
            this.collider = GetComponent<Collider>();
            this.currentHp = hp;
        }

        private void Start()
        {
            EnableCollider(this.startEnabled);
        }

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
            enableMessageId = MessageDispatcher.Instance.StartListening(this.enableMessage, _componentRepository.GetId(), (data) =>
            {
                EnableCollider(true);
            });
            disableMessageId = MessageDispatcher.Instance.StartListening(this.disableMessage, _componentRepository.GetId(), (data) =>
            {
                EnableCollider(false);
            });
            reviveMessageId = MessageDispatcher.Instance.StartListening(this.reviveMessage, _componentRepository.GetId(), (data) =>
            {
                this.currentHp = hp;
            });
            destroyMessageId = MessageDispatcher.Instance.StartListening(this.destroyMessage, _componentRepository.GetId(), (data) =>
            {
                DeathVfx();
                GameObject.Destroy(gameObject);
            });
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
            MessageDispatcher.Instance.StopListening(this.enableMessage, _componentRepository.GetId(), this.enableMessageId);
            MessageDispatcher.Instance.StopListening(this.disableMessage, _componentRepository.GetId(), this.disableMessageId);
            MessageDispatcher.Instance.StopListening(this.reviveMessage, _componentRepository.GetId(), this.reviveMessageId);
        }

        private void EnableCollider(bool enable)
        {
            Debug.Log($"Setting Enable Collider on {this.name} to {enable}");
            if (this.collider != null)
            {
                this.collider.enabled = enable;
            }
            SetMaterial(enable ? this.enableMaterial : this.disabledMaterial);
        }

        private void MessageCallback(Telegram data)
        {
            currentHp--;
            if (currentHp <= 0)
            {
                DeathNotification();
                PlayDeathAudioClip();
                SetMaterial(this.disabledMaterial);
            }
            else
            {
                PlayShotAudioClip();
            }
        }

        private void SetMaterial(Material material)
        {
            if (material == null)
                return;
            Debug.Log($"Setting material on {this.name} to {material.name}");
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.material = material;
            for (int i = 0; i < mr.materials.Length; i++)
            {
                mr.materials[i] = material;
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

        private void DeathVfx()
        {
            if (this.deathVfx != null)
            {
                Vector3 pos = this.deathVfxSpawnPoint == null ? this.transform.position : this.deathVfxSpawnPoint.transform.position;
                GameObject.Instantiate(this.deathVfx, pos, Quaternion.identity);
            }
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