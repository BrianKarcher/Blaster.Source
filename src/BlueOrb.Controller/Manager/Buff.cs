using BlueOrb.Base.Item;
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
        private GameObject iconWithProgressBarGo;

        private float endTime;
        private float startTime;

        private void Start()
        {
            this.startTime = Time.time;
            this.endTime = Time.time + buffConfig.Duration;
            iconWithProgressBarGo = GameStateController.Instance.UIController.HudController.CreateBuffUI();
            iconWithProgressBar = iconWithProgressBarGo.GetComponent<IIconWithProgressBar>();
            iconWithProgressBar.SetValue(0.5f);
            MessageDispatcher.Instance.DispatchMsg(InventoryComponent.AddItemMessage, 0f, this._componentRepository.GetId(),
                LevelStateController.Id, new ItemDesc() { ItemConfig = buffConfig, Qty = 1 });
            // TODO : Update the HUD for the buff icon
            if (buffConfig.HUDImageSelected != null)
            {
                iconWithProgressBar.SetImage(buffConfig.HUDImageSelected);
            }
        }

        private void Update()
        {
            float progress = 1f - ((Time.time - startTime) / (this.endTime - this.startTime));
            iconWithProgressBar.SetValue(progress);
            if (Time.time > endTime)
            {
                GameObject.Destroy(iconWithProgressBarGo);
                GameObject.Destroy(this._componentRepository.gameObject);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MessageDispatcher.Instance.DispatchMsg(InventoryComponent.AddItemMessage, 0f, this._componentRepository.GetId(),
                LevelStateController.Id, new ItemDesc() { ItemConfig = buffConfig, Qty = -1 });
        }
    }
}