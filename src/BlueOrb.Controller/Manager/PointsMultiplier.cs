using BlueOrb.Base.Manager;
using BlueOrb.Controller.Buff;
using BlueOrb.Messaging;
using System;
using UnityEngine;

namespace BlueOrb.Controller.Manager
{
    [Serializable]
    public class PointsMultiplier
    {
        private const string SetMultiplierMessage = "SetMultiplier";
        private const string SetConsecutiveHitsMessage = "SetConsecutiveHits";
        private int pointsMultiplier;
        private int consecutiveHits;

        public void Clear()
        {
            this.pointsMultiplier = 1;
            SetConsecutiveHits(0);
            SetMultiplierInUI();
        }

        public void IncrementConsecutiveHits()
        {
            SetConsecutiveHits(this.consecutiveHits + 1);
            
            if (this.consecutiveHits % 5 == 0)
            {
                IncrementPointsMultiplier();
            }
        }

        public void ResetConsecutiveHits()
        {
            SetConsecutiveHits(0);
            ResetPointsMultiplier();
        }

        private void SetConsecutiveHits(int value)
        {
            this.consecutiveHits = value;
            MessageDispatcher.Instance.DispatchMsg(SetConsecutiveHitsMessage, 0f, null, "Hud Controller", value);
        }

        private void Log()
            => Debug.Log($"Setting consecutive hits to {this.consecutiveHits}");

        private void IncrementPointsMultiplier()
        {
            this.pointsMultiplier++;
            SetMultiplierInUI();
            SendNotification();
        }

        private void ResetPointsMultiplier()
        {
            this.pointsMultiplier = 1;
            SetMultiplierInUI();
        }

        private void SendNotification()
            => MessageDispatcher.Instance.DispatchMsg("Notification", 0f, null, "Hud Controller", $"x{this.pointsMultiplier}");

        private void SetMultiplierInUI()
            => MessageDispatcher.Instance.DispatchMsg(SetMultiplierMessage, 0f, null, "Hud Controller", $"x{this.pointsMultiplier}");

        public int GetPointsMultiplier => this.pointsMultiplier;
    }
}