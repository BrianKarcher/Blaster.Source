﻿using BlueOrb.Base.Attributes;
using BlueOrb.Base.Item;
using BlueOrb.Common.Components;
using BlueOrb.Controller.Manager;
using BlueOrb.Messaging;
using UnityEngine;

namespace BlueOrb.Controller.Inventory
{
    [AddComponentMenu("BlueOrb/Item/Item Acquire Trigger")]
    public class ItemAcquireTrigger : ComponentBase<ItemAcquireTrigger>
    {
        [SerializeField]
        [Tag]
        private string targetTag;

        [SerializeField]
        private ItemConfig itemConfig;

        [SerializeField]
        private int qty = 1;

        [SerializeField]
        private bool destroyOnTrigger = true;

        [SerializeField]
        private string projectileHitMessage = "ProjectileHit";

        private long projectileHitId;

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody != null ? other.attachedRigidbody.CompareTag(targetTag) : other.CompareTag(targetTag))
            {
                Trigger();
            }
        }

        private void Trigger()
        {
            MessageDispatcher.Instance.DispatchMsg(InventoryComponent.AddItemMessage, 0f, null, LevelStateController.Id,
                new ItemDesc
                {
                    ItemConfig = this.itemConfig,
                    Qty = this.qty
                }
            );
            if (this.destroyOnTrigger)
            {
                // Destroy immediate so no further trigger entries count
                GameObject.DestroyImmediate(base._componentRepository.gameObject);
            }
        }

        public override void StartListening()
        {
            base.StartListening();
            projectileHitId = MessageDispatcher.Instance.StartListening(projectileHitMessage, base._componentRepository.GetId(), (data) =>
            {
                Trigger();
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher.Instance.StopListening(this.projectileHitMessage, base._componentRepository.GetId(), this.projectileHitId);
        }
    }
}