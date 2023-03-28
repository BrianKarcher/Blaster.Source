using BlueOrb.Common.Container;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Common.Components
{
    public interface IComponentBase : IMessagingObject
    {
        IEntity GetComponentRepository();
        string ComponentName { get; set; }
        void ReAwaken();
        void Init();
        void Destroy();
        Transform transform { get; }
        GameObject gameObject { get; }
    }
}
