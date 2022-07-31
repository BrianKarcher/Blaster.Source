using BlueOrb.Base.Manager;
using BlueOrb.Common.Components;
using BlueOrb.Controller.Buff;
using BlueOrb.Controller.Inventory;
using BlueOrb.Controller.UI;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Manager
{
    [AddComponentMenu("BlueOrb/Buff/Buff")]
    public class Buff : ComponentBase<Buff>
    {
        [SerializeField]
        private BuffConfig buffConfig;

        private IIconWithProgressBar iconWithProgressBar;

        private float endTime;
        private float startTime;

        private void Start()
        {
            this.startTime = Time.time;
            this.endTime = Time.time + buffConfig.Duration;
            iconWithProgressBar = GameStateController.Instance.UIController.HudController.CreateBuffUI().GetComponent<IIconWithProgressBar>();
            iconWithProgressBar.SetValue(0.5f);
            // TODO : Update the HUD for the buff icon
        }

        private void Update()
        {
            //if (Time.time > endTime)
            //{
            //    GameObject.Destroy(this._componentRepository.gameObject);
            //}
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MessageDispatcher.Instance.DispatchMsg(InventoryComponent.RemoveItemMessage, 0f, this._componentRepository.GetId(),
                LevelStateController.Id, buffConfig.UniqueId);
        }
    }
}