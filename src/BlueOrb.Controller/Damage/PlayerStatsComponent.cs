using BlueOrb.Common.Container;
using BlueOrb.Controller.Manager;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Damage
{
    [AddComponentMenu("BlueOrb/Components/Player Stats Component")]
    public class PlayerStatsComponent : EntityStatsComponent
    {
        private void Start()
        {
            SceneSetup sceneSetup = GameObject.FindObjectOfType<SceneSetup>();
            if (sceneSetup == null)
            {
                Debug.LogError("No scene setup. Must have scene setup!");
                return;
            }
            SetCurrentHp(sceneSetup.SceneConfig.MaxHp);
            UpdateHud();
        }

        protected override float GetCurrentHp()
        {
            return EntityContainer.Instance.LevelStateController.GetCurrentHp();
        }

        protected override float GetMaxHp()
        {
            return EntityContainer.Instance.LevelStateController.GetMaxHp();
        }

        protected override void SetCurrentHp(float hp)
        {
            EntityContainer.Instance.LevelStateController.SetCurrentHp(hp);
        }

        protected override void HpChanged()
        {
            UpdateHud();
        }

        private void UpdateHud()
        {
            MessageDispatcher.Instance.DispatchMsg("UpdateStatsInHud", 0f, this.GetId(), "UI Controller", _entityStats);
            MessageDispatcher.Instance.DispatchMsg("SetHp", 0f, this.GetId(), "Hud Controller", (GetCurrentHp(), _entityStats.MaxHP));
        }
    }
}