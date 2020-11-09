using BlueOrb.Base.Attributes;
using System;
using UnityEngine;

namespace BlueOrb.Base.Config
{
    public class BaseConfig : ScriptableObject, IRQConfig
    {
        [UniqueIdentifier]
        public string UniqueId;

        public string GetUniqueId()
        {
            return UniqueId;
        }
        //protected StateMachine _stateMachine;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
