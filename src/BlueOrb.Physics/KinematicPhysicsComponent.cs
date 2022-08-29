using BlueOrb.Common.Components;
using BlueOrb.Base.Extensions;
using UnityEngine;
using BlueOrb.Controller;

namespace BlueOrb.Physics
{
    [AddComponentMenu("BlueOrb/Physics/KinematicPhysics")]
    public class KinematicPhysicsComponent : ComponentBase<KinematicPhysicsComponent>, IPhysicsComponent, IMovementController
    {
        [SerializeField]
        private PhysicsLogic _controller;

        public PhysicsLogic Controller => _controller;

        [SerializeField]
        private bool _isEnabled = true;

        private Animator _animator;
        private AnimationComponent _animationComponent;
        private Vector3 velocity;
        private Vector3 previousPos;

        protected override void Awake()
        {
            base.Awake();
            _isEnabled = true;
            _controller?.SetMovementController(this);
            _controller.Awake();
            _animator = GetComponent<Animator>();
        }

        protected void Start()
        {
            if (_animationComponent == null)
            {
                _animationComponent = _componentRepository.Components.GetComponent<AnimationComponent>();
            }
            previousPos = transform.position;
        }

        protected void FixedUpdate()
        {
            // Record the world-space velocity movement since this is not controlled by this class
            this.velocity = (transform.position - this.previousPos) * (1f / Time.fixedDeltaTime);
        }

        public bool AutoApplyToAnimator
        {
            get => this.Controller.GetPhysicsData().AutoApplyToAnimator;
            set => this.Controller.GetPhysicsData().AutoApplyToAnimator = value;
        }

        public void RevertAutoApplyToAnimator()
            => this.Controller.GetPhysicsData().AutoApplyToAnimator = this.Controller.OriginalPhysicsData.AutoApplyToAnimator;

        public Vector3 GetWorldPos3()
        {
            var height = _componentRepository.GetHeight();
            return transform.position + new Vector3(0f, height / 2f, 0f);
        }

        public Vector2 GetWorldPos2()
            => transform.position.xz();

        // Height needs to stay the same
        public void SetWorldPos2(Vector2 newPos)
            => this.transform.position = new Vector3(newPos.x, this.transform.position.y, newPos.y);

        public void SetWorldPos3(Vector3 new_pos) => this.transform.position = new_pos;

        public Vector2 GetVelocity2() => this.velocity.xz();

        public Vector3 GetVelocity3() => this.velocity;

        public bool GetIsGrounded() => this._controller.GetIsGrounded();

        public void SetVelocity3(Vector3 velocity) => this.velocity = velocity;

        public void SetVelocity2(Vector2 velocity) => this.velocity = new Vector3(velocity.x, this.velocity.y, velocity.y);

        public float MaxSpeed => this.velocity.magnitude;

        public void Move(Vector3 motion)
            => transform.position += motion * Time.deltaTime;

        public void AddForce(Vector3 force) { }

        public void AddForce2(Vector2 force) { }

        public void AddForce(Vector3 force, ForceMode forceMode) { }

        public void AddForce2(Vector2 force, ForceMode forceMode) { }

        public void Jump() => AddForce(GetPhysicsData().JumpVelocity, ForceMode.VelocityChange);

        public virtual Vector2 GetFeetWorldPosition2() => transform.position.xz();

        public virtual Vector3 GetFeetWorldPosition3() => transform.position;

        public Vector3 GetLocalPos3() => transform.localPosition;

        public void Stop() => SetVelocity3(Vector3.zero);

        public ISteeringBehaviorManager GetSteering() => null;

        public void SetEnabled(bool enabled) => _isEnabled = enabled;

        public bool GetEnabled() => _isEnabled;

        public PhysicsData GetPhysicsData() => _controller.GetPhysicsData();

        public PhysicsData GetOriginalPhysicsData() => _controller.GetOriginalPhysicsData();

        public void Explode(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier)
        {
            
        }
    }
}