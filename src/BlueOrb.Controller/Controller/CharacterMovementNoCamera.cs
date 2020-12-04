using BlueOrb.Common.Components;
using Cinemachine;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Camera
{
    [AddComponentMenu("BlueOrb/Components/Character Movement No Camera")]
    public class CharacterMovementNoCamera : ComponentBase<CharacterMovementNoCamera>
    {
        public Transform InvisibleCameraOrigin;
        [SerializeField]
        private Transform _objectToRotate;

        //public float StrafeSpeed = 0.1f;
        //public float TurnSpeed = 3;
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

        [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
        public bool invertY = false;

        [Header("Rotation Settings")]
        [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));
        public float Damping = 0.2f;
        [SerializeField]
        private bool _360Yaw = false;
        [SerializeField]
        public float _yawRange = 180f;
        //public float VerticalRotMin = -80;
        //public float VerticalRotMax = 80;
        [SerializeField]
        public float _pitchMin = -45f;
        [SerializeField]
        public float _pitchMax = 45f;

        [SerializeField]
        private float _fovZoomOut = 60f;

        [SerializeField]
        private float _fovZoomIn = 20f;

        //public KeyCode sprintJoystick = KeyCode.JoystickButton2;
        //public KeyCode sprintKeyboard = KeyCode.Space;

        //private bool isSprinting;
        private Animator anim;
        //private float currentStrafeSpeed;
        private Vector2 currentVelocity;
        private Rewired.Player _player;
        [SerializeField]
        private float _yawMin;
        [SerializeField]
        private float _yawMax;

        [SerializeField]
        private CinemachineVirtualCamera _cinemachineVirtualCamera;
        private bool _isZoomedIn;

        public float yaw;
        public float pitch;
        public float roll;

        private bool _hasBeenEnabled = false;

        protected override void Awake()
        {
            base.Awake();
            _isZoomedIn = false;
        }

        void Start()
        {
            _player = ReInput.players.Players[0];
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (anim == null)
            {
                anim = GetComponent<Animator>();
            }
            currentVelocity = Vector2.zero;
            pitch = InvisibleCameraOrigin.localEulerAngles.x;
            yaw = _objectToRotate.localEulerAngles.y;
            roll = _objectToRotate.localEulerAngles.z;
            //if (_hasBeenEnabled)
            //{
            //    pitch = UnityEngine.Camera.main.transform.rotation.eulerAngles.x;
            //    yaw = UnityEngine.Camera.main.transform.rotation.eulerAngles.y;
            //    roll = UnityEngine.Camera.main.transform.rotation.eulerAngles.z;
            //    _componentRepository.transform.position = UnityEngine.Camera.main.transform.position - transform.localPosition;
            //    //transform.position = UnityEngine.Camera.main.transform.position;
            //}
            
            if (!_360Yaw)
            {
                _yawMin = _objectToRotate.localEulerAngles.y - (_yawRange / 2.0f);
                _yawMax = _objectToRotate.localEulerAngles.y + (_yawRange / 2.0f);
            }
            //currentStrafeSpeed = 0;
            //isSprinting = false;
            _hasBeenEnabled = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _cinemachineVirtualCamera.m_Lens.FieldOfView = _fovZoomOut;
            _isZoomedIn = false;
        }

        void Update()
        {
            var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            var speed = input.y;
            speed = Mathf.Clamp(speed, -1f, 1f);
            //speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref currentVelocity.y, Damping);
            //anim.SetFloat("Speed", speed);
            //anim.SetFloat("Direction", speed);

            // set sprinting
            //isSprinting = (Input.GetKey(sprintJoystick) || Input.GetKey(sprintKeyboard)) && speed > 0;
            //anim.SetBool("isSprinting", isSprinting);

            // strafing
            //currentStrafeSpeed = Mathf.SmoothDamp(
            //    currentStrafeSpeed, input.x * StrafeSpeed, ref currentVelocity.x, Damping);
            //transform.position += transform.TransformDirection(Vector3.right) * currentStrafeSpeed;

            //var rotInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            var mouseMovement = GetAxisInput();

            ProcessZoomInput();

            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            if (_isZoomedIn)
                mouseSensitivityFactor /= 8;

            //var rot = _objectToRotate.eulerAngles;
            //rot.y += mouseMovement.x * mouseSensitivityFactor;
            yaw += mouseMovement.x * mouseSensitivityFactor;
            if (!_360Yaw)
            {
                yaw = Mathf.Clamp(yaw, _yawMin, _yawMax);
                //rot.y = Mathf.Clamp(rot.y, _yawMin, _yawMax);
            }
            _objectToRotate.localRotation = Quaternion.Euler(0f, yaw, 0f);

            if (InvisibleCameraOrigin != null)
            {
                //rot = InvisibleCameraOrigin.localRotation.eulerAngles;
                //rot.x -= mouseMovement.y * mouseSensitivityFactor;
                ////if (rot.x > 180)
                ////    rot.x -= 360;
                //rot.x = Mathf.Clamp(rot.x, _pitchMin, _pitchMax);
                //rot = InvisibleCameraOrigin.localRotation.eulerAngles;
                pitch -= mouseMovement.y * mouseSensitivityFactor;
                //if (rot.x > 180)
                //    rot.x -= 360;
                pitch = Mathf.Clamp(pitch, _pitchMin, _pitchMax);
                if (InvisibleCameraOrigin == _objectToRotate)
                {
                    InvisibleCameraOrigin.localRotation = Quaternion.Euler(pitch, yaw, 0f);
                }
                else
                {
                    InvisibleCameraOrigin.localRotation = Quaternion.Euler(pitch, 0f, 0f);
                }
            }
        }

        public void CenterHead(float seconds)
        {
            StartCoroutine(CenterHeadProcess(seconds));
        }

        private IEnumerator CenterHeadProcess(float seconds)
        {
            Debug.Log("CenterHeadProcess started");
            float time = 0f;
            var initialInvisibleCameraRotation = InvisibleCameraOrigin.localRotation;
            var initialObjectToRotateRotation = _objectToRotate.localRotation;
            var destRotation = Quaternion.Euler(0f, 0f, 0f);

            while (time < seconds)
            {
                // If both are the same, just do the slerp once.
                if (InvisibleCameraOrigin == _objectToRotate)
                {
                    //Quaternion.
                    //InvisibleCameraOrigin.localRotation.
                    InvisibleCameraOrigin.localRotation = Quaternion.Slerp(InvisibleCameraOrigin.localRotation, destRotation, Time.deltaTime / seconds);
                    //InvisibleCameraOrigin.localRotation = Quaternion.Slerp(InvisibleCameraOrigin.localRotation, destRotation, seconds / Time.deltaTime);
                    //_objectToRotate.localRotation = Quaternion.Slerp(_objectToRotate.localRotation, destRotation, seconds / Time.deltaTime);
                }
                else
                {
                    InvisibleCameraOrigin.localRotation = Quaternion.Slerp(InvisibleCameraOrigin.localRotation, destRotation, Time.deltaTime / seconds);
                    _objectToRotate.localRotation = Quaternion.Slerp(_objectToRotate.localRotation, destRotation, Time.deltaTime / seconds);
                    //InvisibleCameraOrigin.localRotation = Quaternion.Slerp(InvisibleCameraOrigin.localRotation, destRotation, seconds / Time.deltaTime);
                    //_objectToRotate.localRotation = Quaternion.Slerp(_objectToRotate.localRotation, destRotation, seconds / Time.deltaTime);
                }
                time += Time.deltaTime;
                yield return null;
            }
            Debug.Log("CenterHeadProcess finished");
        }

        private void ProcessZoomInput()
        {
            if (_player.GetButtonDown(_zoomButton))
            {
                if (!_isZoomedIn)
                {
                    _cinemachineVirtualCamera.m_Lens.FieldOfView = _fovZoomIn;
                    _isZoomedIn = true;
                }
                else
                {
                    _cinemachineVirtualCamera.m_Lens.FieldOfView = _fovZoomOut;
                    _isZoomedIn = false;
                }
            }

            if (_player.GetButtonDown(_zoomInButton))
            {
                _cinemachineVirtualCamera.m_Lens.FieldOfView = _fovZoomIn;
                _isZoomedIn = true;
            }
            if (_player.GetButtonDown(_zoomOutButton))
            {
                _cinemachineVirtualCamera.m_Lens.FieldOfView = _fovZoomOut;
                _isZoomedIn = false;
            }
        }

        public Vector2 GetAxisInput()
        {
            // read inputs
            if (_player == null)
            {
                return new Vector2(Input.GetAxis(_horizontalAxisName), Input.GetAxis(_verticalAxisName) * (invertY ? -1 : 1));
            }
            else
            {
                return new Vector2(_player.GetAxis(_horizontalAxisName), _player.GetAxis(_verticalAxisName) * (invertY ? -1 : 1));
            }
        }
    }
}
