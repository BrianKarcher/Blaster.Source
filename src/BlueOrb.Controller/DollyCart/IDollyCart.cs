using UnityEngine;

namespace BlueOrb.Controller.DollyCart
{
    public interface IDollyCart
    {
        float SmoothTime { get; }
        float Speed { get; }
        float TargetSpeed { get; }
        string GetId();
        void Brake();
        float GetSpeed();
        void ProcessDollyCartSpeedChange();
        void ResetPosition();
        void SetSpeed(float speed);
        void SetPosition(float pos);
        Vector3 GetWorldPosition();
        Quaternion GetWorldRotation();
        void SetTargetSpeed(float speed);
        void StartAcceleration(float speed, float time = 1, bool immediate = false);
        void Stop();
        float FindPositionClosestToPoint(Vector3 pos);
    }
}