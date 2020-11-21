using Rewired;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlueOrb.Controller.Camera
{
    [AddComponentMenu("BlueOrb/Components/Character Movement No Camera")]
    public class CharacterMovementNoCamera : MonoBehaviour
    {
        public Transform InvisibleCameraOrigin;

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
        //public KeyCode sprintJoystick = KeyCode.JoystickButton2;
        //public KeyCode sprintKeyboard = KeyCode.Space;

        //private bool isSprinting;
        private Animator anim;
        //private float currentStrafeSpeed;
        private Vector2 currentVelocity;
        private Rewired.Player _player;
        private float _yawMin;
        private float _yawMax;

        void Start()
        {
            _player = ReInput.players.Players[0];
        }

        void OnEnable()
        {
            if (anim == null)
            {
                anim = GetComponent<Animator>();
            }
            currentVelocity = Vector2.zero;
            if (!_360Yaw)
            {
                _yawMin = transform.rotation.eulerAngles.y - (_yawRange / 2.0f);
                _yawMax = transform.rotation.eulerAngles.y + (_yawRange / 2.0f);
            }
            //currentStrafeSpeed = 0;
            //isSprinting = false;
        }

        void Update()
        {
            var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            var speed = input.y;
            speed = Mathf.Clamp(speed, -1f, 1f);
            speed = Mathf.SmoothDamp(anim.GetFloat("Speed"), speed, ref currentVelocity.y, Damping);
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

            var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

            var rot = transform.eulerAngles;
            rot.y += mouseMovement.x * mouseSensitivityFactor;
            transform.rotation = Quaternion.Euler(rot);

            if (InvisibleCameraOrigin != null)
            {
                rot = InvisibleCameraOrigin.localRotation.eulerAngles;
                rot.x -= mouseMovement.y * mouseSensitivityFactor;
                if (rot.x > 180)
                    rot.x -= 360;
                rot.x = Mathf.Clamp(rot.x, _yawMin, _yawMax);
                InvisibleCameraOrigin.localRotation = Quaternion.Euler(rot);
            }
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
    }
}
