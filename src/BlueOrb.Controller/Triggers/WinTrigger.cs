using BlueOrb.Base.Attributes;
using BlueOrb.Base.Manager;
using BlueOrb.Common.Container;
using UnityEngine;

namespace BlueOrb.Controller.Triggers
{
    [AddComponentMenu("BlueOrb/Components/Win Trigger")]
    public class WinTrigger : MonoBehaviour
    {
        [SerializeField]
        [Tag]
        private string tag;

        private Collider thisCollider;
        private bool hasBegun = false;

        protected void Awake()
        {
            thisCollider = GetComponent<Collider>();
            thisCollider.enabled = false;
        }

        public void FixedUpdate()
        {
            if (hasBegun)
            {
                return;
            }
            if (GameStateController.Instance.LevelStateController.HasLevelBegun)
            {
                thisCollider.enabled = true;
                hasBegun = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeInHierarchy)
                return;
            Debug.Log($"Trigger entered: {other.name}");
            if (!other.CompareTag(tag))
            {
                return;
            }
            Debug.Log($"Trigger: {other.name}");

            GameStateController.Instance.LevelStateController.Win();
        }
    }
}