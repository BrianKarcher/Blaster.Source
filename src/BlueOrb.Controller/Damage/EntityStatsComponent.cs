using BlueOrb.Common.Components;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Damage
{
    [AddComponentMenu("BlueOrb/Components/Entity Stats Component")]
    public class EntityStatsComponent : ComponentBase<EntityStatsComponent>, IEntityStatsComponent
    {
        [SerializeField]
        protected EntityStatsData _entityStats = new EntityStatsData();

        private long _setMaxHpId, _setCurrentHpId;

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
                HpChanged();
            });
            _setCurrentHpId = MessageDispatcher.Instance.StartListening("SetCurrentHp", _componentRepository.GetId(), (data) =>
            {
                float hp = (float)data.ExtraInfo;
                _entityStats.CurrentHP = hp;
                Debug.Log($"Setting current hp to {hp}");
                HpChanged();
            });
        }

        protected virtual void HpChanged()
        {

        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening("SetMaxHp", _componentRepository.GetId(), _setMaxHpId);
            MessageDispatcher.Instance.StopListening("SetCurrentHp", _componentRepository.GetId(), _setCurrentHpId);
        }

        public void AddHp(float hp)
        {
            Debug.Log($"Adding {hp} HP to {_componentRepository.name}");
            SetCurrentHp(GetCurrentHp() + hp);
            Debug.Log($"{_componentRepository.name} HP remaining: {_entityStats.CurrentHP}");
            HpChanged();
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

        protected virtual float GetCurrentHp()
        {
            Debug.Log($"(EntityStatsComponent) GetCurrentHp returning {_entityStats.CurrentHP} for {name}, {_componentRepository.name}");
            return _entityStats.CurrentHP;
        }

        protected virtual void SetCurrentHp(float hp)
        {
            hp = Mathf.Max(0, hp);
            _entityStats.CurrentHP = hp;
        }

        protected virtual float GetMaxHp() => _entityStats.MaxHP;

        protected virtual void SetMaxHp(float hp) => _entityStats.MaxHP = hp;

        public bool IsDead() => _entityStats.CurrentHP <= 0;
    }
}