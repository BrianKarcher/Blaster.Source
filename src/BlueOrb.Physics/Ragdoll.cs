using BlueOrb.Controller;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlueOrb.Physics
{
    [Serializable]
    public class Ragdoll
    {
        [SerializeField]
        private GameObject[] mainColliders;

        [SerializeField]
        private GameObject ragdollRigidbodySource;

        [SerializeField]
        // rigidbody that is  activated when not using ragdoll
        private Rigidbody mainRigidbody;

        private Collider[] ragdollColliders;
        // HashSet helps to automatically remove duplicates
        private HashSet<Rigidbody> ragdollRigidbodies;
        private AnimationComponent animationComponent;

        public void Init(AnimationComponent animationComponent)
        {
            this.animationComponent = animationComponent;
            if (ragdollRigidbodySource == null)
            {
                return;
            }

            ragdollColliders = ragdollRigidbodySource.GetComponentsInChildren<Collider>(true);
            ragdollRigidbodies = new HashSet<Rigidbody>();
            foreach (Collider coll in ragdollColliders)
            {
                Rigidbody body = coll.GetComponent<Rigidbody>();
                if (body != null)
                {
                    ragdollRigidbodies.Add(body);
                }
            }

            EnableColliders(ragdollColliders, false);
            foreach (Rigidbody ragdollRigidBody in this.ragdollRigidbodies)
            {
                ragdollRigidBody.useGravity = false; // make rigidbody use gravity if ragdoll is active
                ragdollRigidBody.isKinematic = true; // enable or disable kinematic accordig to enableRagdoll variable
            }
        }

        public void EnableRagdoll(bool enableRagdoll)
        {
            if (ragdollRigidbodySource == null)
            {
                return;
            }
            // The animator needs to be disabled in order for Ragdoll to work.
            this.animationComponent.SetEnabled(!enableRagdoll);
            List<Collider> mainColliders = GetMainColliders();
            EnableColliders(mainColliders, !enableRagdoll);
            EnableColliders(this.ragdollColliders, enableRagdoll);
            foreach (Rigidbody ragdollRigidBody in this.ragdollRigidbodies)
            {
                ragdollRigidBody.useGravity = enableRagdoll; // make rigidbody use gravity if ragdoll is active
                ragdollRigidBody.isKinematic = !enableRagdoll; // enable or disable kinematic accordig to enableRagdoll variable
            }
            mainRigidbody.useGravity = !enableRagdoll; // normal rigidbody dont use gravity when ragdoll is active
            mainRigidbody.isKinematic = enableRagdoll;
        }

        private List<Collider> GetMainColliders()
        {
            List<Collider> mainColliders = new List<Collider>();
            foreach (GameObject coll in this.mainColliders)
            {
                Collider collider = coll.GetComponent<Collider>();
                if (collider != null)
                {
                    mainColliders.Add(collider);
                }
            }
            return mainColliders;
        }

        private void EnableColliders(IEnumerable<Collider> colliders, bool enable)
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = enable; // enable all colliders if ragdoll is set to enabled
            }
        }
    }
}