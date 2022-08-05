using BlueOrb.Base.Manager;
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
                SetMaxHp(5);
                SetCurrentHp(5);
                UpdateHud(true);
                return;
            }
            SetMaxHp(sceneSetup.SceneConfig.MaxHp);
            SetCurrentHp(sceneSetup.SceneConfig.MaxHp);
            SetCurrentScore(0);
            UpdateHud(true);
            GameStateController.Instance.LevelStateController.UpdateUIScore(true);
        }

        protected override float GetCurrentHp() => GameStateController.Instance.LevelStateController.GetCurrentHp();

        protected override float GetMaxHp() => GameStateController.Instance.LevelStateController.GetMaxHp();

        protected override void SetCurrentHp(float hp) => GameStateController.Instance.LevelStateController.SetCurrentHp(hp);

        private void SetCurrentScore(int score) => GameStateController.Instance.LevelStateController.SetCurrentScore(score);

        protected override void SetMaxHp(float hp) => GameStateController.Instance.LevelStateController.SetMaxHp(hp);

        protected override void HpChanged() => UpdateHud(false);

        private void UpdateHud(bool immediate)
        {
            MessageDispatcher.Instance.DispatchMsg("UpdateStatsInHud", 0f, this.GetId(), "UI Controller", _entityStats);
            MessageDispatcher.Instance.DispatchMsg("SetHp", 0f, this.GetId(), "Hud Controller", (GetCurrentHp(), GetMaxHp(), immediate));
        }
    }
}