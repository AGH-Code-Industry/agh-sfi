using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DeadFusion.GameCore
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] AnimationCurve forceFromDamage;
        [SerializeField] float explosionForce;
        [SerializeField] AnimationCurve explosionMultiplierFromDamage;
        [Description("First Rigidbody is the main one.")]
        [SerializeField] List<Rigidbody> rigidbodies = new List<Rigidbody>();
        [SerializeField] Animator animator;

        [Space()]
        [SerializeField] NavMeshAgent agent;
        [SerializeField] float agentVelocityMultiplier;

        public void DoRagdoll(Vector3 direction, float damage, Rigidbody hitCollider = null)
        {
            if (animator != null)
                animator.enabled = false;

            Vector3 velocity = Vector3.zero;
            if (agent != null && agent.enabled) 
            {
                velocity = agent.velocity;
            }

            foreach (Rigidbody rb in rigidbodies)
            {
                rb.isKinematic = false;
            }
            if (hitCollider != null && rigidbodies.Contains(hitCollider))
            {
                hitCollider.velocity = velocity * agentVelocityMultiplier;
                hitCollider.AddForce(direction * forceFromDamage.Evaluate(damage));

                if (hitCollider.TryGetComponent(out DestroyableLimb limb))
                    limb.TryDestroy(damage);
            }
        }

        public void EnableColliders()
        {
            foreach (Rigidbody rb in rigidbodies)
            {
                Collider[] colliders = rb.GetComponents<Collider>();
                foreach (Collider collider in colliders)
                    collider.enabled = true;
            }
        }

        public void AddExplosionForce(Vector3 point, float damage, float radius)
        {
            foreach (Rigidbody rb in rigidbodies)
            {
                rb.AddExplosionForce(explosionMultiplierFromDamage.Evaluate(damage) * explosionForce, point, radius);
            }
        }
    }
}