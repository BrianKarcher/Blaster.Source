using BlueOrb.Common.Components;
using BlueOrb.Controller.Buff;
using BlueOrb.Controller.Inventory;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Manager
{
    [AddComponentMenu("BlueOrb/Buff/BuffBase")]
    public class Buff : ComponentBase<Buff>
    {
        [SerializeField]
        private BuffConfig buffConfig;

        private float endTime;
        private float startTime;

        private void Start()
        {
            this.startTime = Time.time;
            this.endTime = Time.time + buffConfig.Duration;
            // TODO : Update the HUD for the buff icon
        }

        private void Update()
        {
            if (Time.time > endTime)
            {
                GameObject.Destroy(this._componentRepository.gameObject);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MessageDispatcher.Instance.DispatchMsg(InventoryComponent.RemoveItemMessage, 0f, this._componentRepository.GetId(), LevelStateController.Id, null);
        }
    }
}