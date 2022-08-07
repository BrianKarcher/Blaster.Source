using BlueOrb.Messaging;
using System;
using UnityEngine;

namespace BlueOrb.Controller.Manager
{
    [Serializable]
    public class PointsMultiplier
    {
        private int pointsMultiplier;
        private int consecutiveHits;

        public void Clear()
        {
            this.pointsMultiplier = 1;
            this.consecutiveHits = 0;
            SetMultiplierInUI();
        }

        public void IncrementConsecutiveHits()
        {
            this.consecutiveHits++;
            Log();
            if (this.consecutiveHits % 5 == 0)
            {
                IncrementPointsMultiplier();
            }
        }

        public void ResetConsecutiveHits()
        {
            this.consecutiveHits = 0;
            Log();
            ResetPointsMultiplier();
        }

        private void Log()
            => Debug.Log($"Setting consecutive hits to {this.consecutiveHits}");

        private void IncrementPointsMultiplier()
        {
            this.pointsMultiplier++;
            SendNotification();
        }

        private void ResetPointsMultiplier()
        {
            this.pointsMultiplier = 1;
            SendNotification();
        }

        private void SendNotification()
            => MessageDispatcher.Instance.DispatchMsg("Notification", 0f, null, "Hud Controller", $"x{this.pointsMultiplier}");

        private void SetMultiplierInUI()
            => MessageDispatcher.Instance.DispatchMsg("SetMultiplier", 0f, null, "Hud Controller", $"x{this.pointsMultiplier}");

        public int GetPointsMultiplier => this.pointsMultiplier;
    }
}