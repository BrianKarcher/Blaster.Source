using System;
using UnityEngine;

namespace BlueOrb.Controller.Damage
{
    [Serializable]
    public class EntityStatsData
    {
        [SerializeField]
        private float _maxHP;
        public float MaxHP { get { return _maxHP; } set { _maxHP = value; } }

        [SerializeField]
        private float _currentHP;
        public float CurrentHP { get { return _currentHP; } set { _currentHP = value; } }

        public EntityStatsData Clone()
        {
            return new EntityStatsData()
            {
                _maxHP = this._maxHP,
                _currentHP = this._currentHP
            };
        }
    }
}
