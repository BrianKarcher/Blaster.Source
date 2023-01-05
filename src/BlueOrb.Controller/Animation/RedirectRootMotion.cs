using BlueOrb.Common.Components;
using UnityEngine;

namespace BlueOrb.Controller.Enemy
{
    [AddComponentMenu("Blue Orb/Components/Redirect Root Motion")]
    public class RedirectRootMotion : ComponentBase<RedirectRootMotion>
    {
        [SerializeField] private Transform _MotionTransform;
        [SerializeField] private Rigidbody _MotionRigidbody;
        [SerializeField] private CharacterController _MotionCharacterController;

        private Animator animator;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorMove()
        {
            if (!animator.applyRootMotion)
                return;

            if (_MotionTransform != null)
            {
                _MotionTransform.position += animator.deltaPosition;
                _MotionTransform.rotation *= animator.deltaRotation;
            }
            else if (_MotionRigidbody != null)
            {
                _MotionRigidbody.MovePosition(_MotionRigidbody.position + animator.deltaPosition);
                _MotionRigidbody.MoveRotation(_MotionRigidbody.rotation * animator.deltaRotation);
            }
            else if (_MotionCharacterController != null)
            {
                _MotionCharacterController.Move(animator.deltaPosition);
                _MotionCharacterController.transform.rotation *= animator.deltaRotation;
            }
            else
            {
                // If we aren't retargeting, just let Unity apply the Root Motion normally.
                this.animator.ApplyBuiltinRootMotion();
            }
        }
    }
}