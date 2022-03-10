using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Damage
{
    [AddComponentMenu("RQ/Components/Entity Stats Component")]
    public class EntityStatsComponent : ComponentBase<EntityStatsComponent>
    {
        [SerializeField]
        private EntityStatsData _entityStats = new EntityStatsData();

        private long _setMaxHpId, _setCurrentHpId;

        protected override void Awake()
        {
            base.Awake();
        }

        public EntityStatsData GetEntityStats()
        {
            return _entityStats;
        }

        public override void StartListening()
        {
            base.StartListening();
            _setMaxHpId = MessageDispatcher.Instance.StartListening("SetMaxHp", _componentRepository.GetId(), (data) =>
            {
                float maxHp = (float)data.ExtraInfo;
                _entityStats.MaxHP = maxHp;
                Debug.Log($"Setting max hp to {maxHp}");
                UpdateHud();
            });
            _setCurrentHpId = MessageDispatcher.Instance.StartListening("SetCurrentHp", _componentRepository.GetId(), (data) =>
            {
                float hp = (float)data.ExtraInfo;
                _entityStats.CurrentHP = hp;
                Debug.Log($"Setting current hp to {hp}");
                UpdateHud();
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("SetMaxHp", _componentRepository.GetId(), _setMaxHpId);
            MessageDispatcher.Instance.StopListening("SetCurrentHp", _componentRepository.GetId(), _setCurrentHpId);
        }

        private void RaiseCurrentHP(float amount)
        {
            _entityStats.CurrentHP += amount;
            if (_entityStats.CurrentHP > _entityStats.MaxHP)
                _entityStats.CurrentHP = _entityStats.MaxHP;
        }

        public void AddHp(float hp)
        {
            Debug.Log($"Adding {hp} HP to {_componentRepository.name}");
            _entityStats.CurrentHP += hp;
            Debug.Log($"{_componentRepository.name} HP remaining: {_entityStats.CurrentHP}");

            if (this.tag == "Player")
            {
                UpdateHud();
            }

            if (_entityStats.CurrentHP <= 0)
            {
                MessageDispatcher.Instance.DispatchMsg("EntityDied", 0f, this.GetId(), _componentRepository.GetId(), null);
            }
            else
            {
                Debug.Log($"Sending Damaged message to self {_componentRepository.name}");
                MessageDispatcher.Instance.DispatchMsg("Damaged", 0f, GetId(),
                    _componentRepository.GetId(), null);
            }
            MessageDispatcher.Instance.DispatchMsg("EntityHPChanged", 0f, this.GetId(), _componentRepository.GetId(), _entityStats.CurrentHP);

        }

        private void UpdateHud()
        {
            if (this.tag != "Player")
            {
                return;
            }
            MessageDispatcher.Instance.DispatchMsg("UpdateStatsInHud", 0f, this.GetId(), "UI Controller", _entityStats);
            MessageDispatcher.Instance.DispatchMsg("SetHp", 0f, this.GetId(), "Hud Controller", (_entityStats.CurrentHP, _entityStats.MaxHP));
        }

        public bool IsDead()
        {
            return _entityStats.CurrentHP <= 0;
        }
    }
}
