﻿using BlueOrb.Common.Components;
using Cinemachine;
using Rewired;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Camera
{
    [AddComponentMenu("BlueOrb/Components/Rotate Only Camera Controller")]
    public class RotateOnlyCameraController : ComponentBase<RotateOnlyCameraController>, ICameraController
    {
        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;
            public float fov;
            private CinemachineVirtualCamera _virtualCam;

            public void SetVirtualCamera(CinemachineVirtualCamera virtualCam)
            {
                _virtualCam = virtualCam;
            }
            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

                x += rotatedTranslation.x;
                y += rotatedTranslation.y;
                z += rotatedTranslation.z;
            }

            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct, float fovLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
                //fov = Mathf.Lerp(fov, target.fov, fovLerpPct);
                fov = target.fov;

                x = Mathf.Lerp(x, target.x, positionLerpPct);
                y = Mathf.Lerp(y, target.y, positionLerpPct);
                z = Mathf.Lerp(z, target.z, positionLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
                _virtualCam.m_Lens.FieldOfView = fov;
            }
        }

        CameraState m_TargetCameraState = new CameraState();
        CameraState m_InterpolatingCameraState = new CameraState();

        [Header("Movement Settings")]
        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
        public float boost = 3.5f;

        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
        public float positionLerpTime = 0.2f;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.01f;

        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
        public float fovLerpTime = 0.01f;

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        [SerializeField]
        private string _verticalAxisName;

        [SerializeField]
        private string _horizontalAxisName;

        [SerializeField]
        private string _zoomButton;

        [SerializeField]
        private string _zoomInButton;

        [SerializeField]
        private string _zoomOutButton;

        [SerializeField]
        public float _pitchMin = -45f;
        [SerializeField]
        public float _pitchMax = 45f;

        [SerializeField]
        private bool _360Yaw = false;

        [SerializeField]
        public float _yawRange = 180f;

        [SerializeField]
        private float _fovZoomOut = 60f;

        [SerializeField]
        private float _fovZoomIn = 20f;

        private Rewired.Player _player;

        private UnityEngine.Camera _camera;

        private float _yawMin;
        private float _yawMax;

        private CinemachineVirtualCamera _virtualCam;

        //private CinemachineVirtualCamera cam

        protected override void Awake()
        {
            base.Awake();
            if (_camera == null)
            {
                //_camera = GetComponent<UnityEngine.Camera>();
                _camera = UnityEngine.Camera.main;
            }
            //if (_virtualCam == null)
            //{
                _virtualCam = GetComponent<CinemachineVirtualCamera>();
            //}                
        }

        private void Start()
        {
            if (!ReInput.isReady)
            {
                Debug.LogError("Rewired not ready.");
                return;
            }
            _player = ReInput.players.Players[0];
            m_InterpolatingCameraState.SetVirtualCamera(_virtualCam);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            transform.position = _camera.transform.position;
            transform.rotation = _camera.transform.rotation;
            m_TargetCameraState.fov = _camera.fieldOfView;
            m_InterpolatingCameraState.fov = _camera.fieldOfView;
            m_TargetCameraState.SetFromTransform(transform);
            m_InterpolatingCameraState.SetFromTransform(transform);
            if (!_360Yaw)
            {
                _yawMin = transform.rotation.eulerAngles.y - (_yawRange / 2.0f);
                _yawMax = transform.rotation.eulerAngles.y + (_yawRange / 2.0f);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            m_TargetCameraState.fov = _fovZoomOut;
            _virtualCam.m_Lens.FieldOfView = _fovZoomOut;
        }

        Vector3 GetInputTranslationDirection()
        {
            Vector3 direction = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.back;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                direction += Vector3.down;
            }
            if (Input.GetKey(KeyCode.E))
            {
                direction += Vector3.up;
            }
            return direction;
        }

        void Update()
        {
            Vector3 translation = Vector3.zero;

#if ENABLE_LEGACY_INPUT_MANAGER

            // Exit Sample  
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false; 
#endif
            }
            // Hide and lock cursor when right mouse button pressed
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Unlock and show cursor when right mouse button released
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            // Rotation
            if (Input.GetMouseButton(1))
            {
                var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
                
                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
            }
            
            // Translation
            translation = GetInputTranslationDirection() * Time.deltaTime;

            // Speed up movement when shift key held
            if (Input.GetKey(KeyCode.LeftShift))
            {
                translation *= 10.0f;
            }

            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
            boost += Input.mouseScrollDelta.y * 0.2f;
            translation *= Mathf.Pow(2.0f, boost);

#elif USE_INPUT_SYSTEM
            // TODO: make the new input system work
#else
            var mouseMovement = GetAxisInput();

            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
            m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

            m_TargetCameraState.pitch = Mathf.Clamp(m_TargetCameraState.pitch, _pitchMin, _pitchMax);
            if (!_360Yaw)
            {
                m_TargetCameraState.yaw = Mathf.Clamp(m_TargetCameraState.yaw, _yawMin, _yawMax);
            }

            if (_player.GetButtonDown(_zoomButton))
            {
                if (m_TargetCameraState.fov == _fovZoomOut)
                {
                    m_TargetCameraState.fov = _fovZoomIn;
                }
                else
                {
                    m_TargetCameraState.fov = _fovZoomOut;
                }
            }

            if (_player.GetButtonDown(_zoomInButton))
            {
                m_TargetCameraState.fov = _fovZoomIn;
            }
            if (_player.GetButtonDown(_zoomOutButton))
            {
                m_TargetCameraState.fov = _fovZoomOut;
            }
#endif

            //m_TargetCameraState.Translate(translation);

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            //var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
            var fovLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / fovLerpTime) * Time.deltaTime);
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, 0f, rotationLerpPct, fovLerpPct);

            m_InterpolatingCameraState.UpdateTransform(transform);
        }

        public Vector2 GetAxisInput()
        {
            // read inputs
            if (_player == null)
            {
                return new Vector2(Input.GetAxis(_horizontalAxisName), Input.GetAxis(_verticalAxisName) * (invertY ? 1 : -1));
            }
            else
            {
                return new Vector2(_player.GetAxis(_horizontalAxisName), _player.GetAxis(_verticalAxisName) * (invertY ? 1 : -1));
            }
        }

        public bool Raycast(float maxDistance, int layerMask, out RaycastHit hitInfo)
        {
            // Example usage:
            //     Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            // RaycastHit hit;
            // if (Physics.Raycast(ray, out hit))
            //     print("I'm looking at " + hit.transform.name);
            // else
            //     print("I'm looking at nothing!");

            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            // Create a vector at the center of our camera's viewport
            // Vector3 rayOrigin = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

            // var rtn = UnityEngine.Physics.Raycast(rayOrigin, _camera.transform.forward, out hitInfo, maxDistance, layerMask);
            var rtn = UnityEngine.Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
            Debug.DrawRay(ray.origin, ray.direction, Color.green, 1.0f);
            return rtn;
        }
    }
}


// Unity SimpleCameraController (It's a first-Person camera including movement and rotation - for reference)
//#if ENABLE_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM_PACKAGE
//#define USE_INPUT_SYSTEM
//    using UnityEngine.InputSystem;
//    using UnityEngine.InputSystem.Controls;
//#endif

//using UnityEngine;

//namespace UnityTemplateProjects
//{
//    public class SimpleCameraController : MonoBehaviour
//    {
//        class CameraState
//        {
//            public float yaw;
//            public float pitch;
//            public float roll;
//            public float x;
//            public float y;
//            public float z;

//            public void SetFromTransform(Transform t)
//            {
//                pitch = t.eulerAngles.x;
//                yaw = t.eulerAngles.y;
//                roll = t.eulerAngles.z;
//                x = t.position.x;
//                y = t.position.y;
//                z = t.position.z;
//            }

//            public void Translate(Vector3 translation)
//            {
//                Vector3 rotatedTranslation = Quaternion.Euler(pitch, yaw, roll) * translation;

//                x += rotatedTranslation.x;
//                y += rotatedTranslation.y;
//                z += rotatedTranslation.z;
//            }

//            public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
//            {
//                yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
//                pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
//                roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

//                x = Mathf.Lerp(x, target.x, positionLerpPct);
//                y = Mathf.Lerp(y, target.y, positionLerpPct);
//                z = Mathf.Lerp(z, target.z, positionLerpPct);
//            }

//            public void UpdateTransform(Transform t)
//            {
//                t.eulerAngles = new Vector3(pitch, yaw, roll);
//                t.position = new Vector3(x, y, z);
//            }
//        }

//        CameraState m_TargetCameraState = new CameraState();
//        CameraState m_InterpolatingCameraState = new CameraState();

//        [Header("Movement Settings")]
//        [Tooltip("Exponential boost factor on translation, controllable by mouse wheel.")]
//        public float boost = 3.5f;

//        [Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
//        public float positionLerpTime = 0.2f;

//        [Header("Rotation Settings")]
//        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
//        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

//        [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
//        public float rotationLerpTime = 0.01f;

//        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
//        public bool invertY = false;

//        void OnEnable()
//        {
//            m_TargetCameraState.SetFromTransform(transform);
//            m_InterpolatingCameraState.SetFromTransform(transform);
//        }

//        Vector3 GetInputTranslationDirection()
//        {
//            Vector3 direction = new Vector3();
//            if (Input.GetKey(KeyCode.W))
//            {
//                direction += Vector3.forward;
//            }
//            if (Input.GetKey(KeyCode.S))
//            {
//                direction += Vector3.back;
//            }
//            if (Input.GetKey(KeyCode.A))
//            {
//                direction += Vector3.left;
//            }
//            if (Input.GetKey(KeyCode.D))
//            {
//                direction += Vector3.right;
//            }
//            if (Input.GetKey(KeyCode.Q))
//            {
//                direction += Vector3.down;
//            }
//            if (Input.GetKey(KeyCode.E))
//            {
//                direction += Vector3.up;
//            }
//            return direction;
//        }

//        void Update()
//        {
//            Vector3 translation = Vector3.zero;

//#if ENABLE_LEGACY_INPUT_MANAGER

//            // Exit Sample  
//            if (Input.GetKey(KeyCode.Escape))
//            {
//                Application.Quit();
//#if UNITY_EDITOR
//				UnityEditor.EditorApplication.isPlaying = false; 
//#endif
//            }
//            // Hide and lock cursor when right mouse button pressed
//            if (Input.GetMouseButtonDown(1))
//            {
//                Cursor.lockState = CursorLockMode.Locked;
//            }

//            // Unlock and show cursor when right mouse button released
//            if (Input.GetMouseButtonUp(1))
//            {
//                Cursor.visible = true;
//                Cursor.lockState = CursorLockMode.None;
//            }

//            // Rotation
//            if (Input.GetMouseButton(1))
//            {
//                var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));
                
//                var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

//                m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
//                m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
//            }
            
//            // Translation
//            translation = GetInputTranslationDirection() * Time.deltaTime;

//            // Speed up movement when shift key held
//            if (Input.GetKey(KeyCode.LeftShift))
//            {
//                translation *= 10.0f;
//            }

//            // Modify movement by a boost factor (defined in Inspector and modified in play mode through the mouse scroll wheel)
//            boost += Input.mouseScrollDelta.y * 0.2f;
//            translation *= Mathf.Pow(2.0f, boost);

//#elif USE_INPUT_SYSTEM
//            // TODO: make the new input system work
//#endif

//            m_TargetCameraState.Translate(translation);

//            // Framerate-independent interpolation
//            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
//            var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
//            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
//            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

//            m_InterpolatingCameraState.UpdateTransform(transform);
//        }
//    }

//}
