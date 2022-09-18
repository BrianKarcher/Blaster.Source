//using Cinemachine;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using static BlueOrb.Controller.DollyCartComponent;

//namespace BlueOrb.Controller.DollyCart
//{
//    public enum LerpType
//    {
//        Lerp = 0,
//        Slerp = 1,
//        SmoothDamp = 2,
//        InverseLerp = 3,
//        SmoothStep = 4
//    }

//    [AddComponentMenu("BlueOrb/Components/Dolly Cart")]
//    public class DollyCart : MonoBehaviour, IDollyCart
//    {
//        [SerializeField]
//        private CinemachineDollyCart _cinemachineDollyCart;
//        //private GameObject cinemachineDollyCartGameObject;

//        [SerializeField]
//        public LerpType _speedChangeType = LerpType.SmoothDamp;

//        [SerializeField]
//        private float startSpeed;
//        [SerializeField]
//        private float speed;
//        public float Speed => speed;

//        [SerializeField]
//        private float targetSpeed;
//        public float TargetSpeed => targetSpeed;
//        public void SetTargetSpeed(float speed) => targetSpeed = speed;

//        [SerializeField]
//        private bool updatingSpeed = false;

//        private float _velocity;

//        [SerializeField]
//        private float _smoothTime = 2f;
//        public float SmoothTime => _smoothTime;

//        [SerializeField]
//        private bool _speedDecreasing = false;
//        [SerializeField]
//        private float emergencyBrakeTime = 0.5f;

//        public float GetSpeed() => _cinemachineDollyCart.m_Speed;

//        private void Awake()
//        {
//            _cinemachineDollyCart = GetComponent<CinemachineDollyCart>();
//        }

//        public void SetSpeed(float speed)
//        {
//            this.speed = speed;
//            _cinemachineDollyCart.m_Speed = speed;
//        }

//        public float FindPositionClosestToPoint(Vector3 pos) => 0;

//        public Vector3 GetWorldPosition() => this.transform.position;

//        public Quaternion GetWorldRotation() => this.transform.rotation;

//        public void SetPosition(float pos) => _cinemachineDollyCart.m_Position = pos;

//        public void Reset() => this._cinemachineDollyCart.m_Position = 0;

//        public void Stop()
//        {
//            updatingSpeed = false;
//            SetSpeed(0f);
//        }

//        public void StartAcceleration(float speed, float time = 1f, bool immediate = false)
//        {
//            updatingSpeed = true;
//            startSpeed = GetSpeed();
//            this.speed = immediate ? speed : startSpeed;
//            SetSpeed(this.speed);
//            _smoothTime = time;
//            targetSpeed = speed;
//            if (startSpeed < targetSpeed)
//            {
//                _speedDecreasing = false;
//            }
//            else
//            {
//                _speedDecreasing = true;
//            }
//        }

//        public void Brake() => StartAcceleration(0f, this.emergencyBrakeTime);

//        public void ProcessDollyCartSpeedChange()
//        {
//            switch (_speedChangeType)
//            {
//                case LerpType.Lerp:
//                    SetSpeed(Mathf.Lerp(GetSpeed(), targetSpeed, 1f / _smoothTime * Time.deltaTime));
//                    //_cinemachineDollyCart.m_Speed = Mathf.Lerp(_startSpeed, _targetSpeed, 1f / _smoothTime * Time.deltaTime);
//                    //_cinemachineDollyCart.m_Speed = Mathf.Lerp(_cinemachineDollyCart.m_Speed, _targetSpeed, _smoothTime * Time.deltaTime);

//                    this.speed = GetSpeed();
//                    break;
//                case LerpType.SmoothDamp:
//                    SetSpeed(Mathf.SmoothDamp(this.speed, targetSpeed, ref _velocity, _smoothTime));
//                    this.speed = GetSpeed();
//                    break;
//                case LerpType.SmoothStep:
//                    SetSpeed(Mathf.SmoothStep(this.speed, targetSpeed, _smoothTime));
//                    break;
//            }
//            if (Mathf.Approximately(this.speed, targetSpeed) || (_speedDecreasing && this.speed <= targetSpeed + 0.01f))
//            {
//                this.speed = targetSpeed;
//                updatingSpeed = false;
//            }
//            else if (Mathf.Approximately(this.speed, targetSpeed) || (!_speedDecreasing && this.speed >= targetSpeed - 0.01f))
//            {
//                this.speed = targetSpeed;
//                updatingSpeed = false;
//            }
//            SetSpeed(this.speed);
//        }
//    }
//}