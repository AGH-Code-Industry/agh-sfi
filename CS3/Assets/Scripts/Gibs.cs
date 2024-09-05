using UnityEngine;

namespace DeadFusion.GameCore
{
    public class Gibs : MonoBehaviour
    {
        [SerializeField] Rigidbody[] gibObjects;
        [SerializeField] Ragdoll ragdoll;

        public void Activate()
        {
            foreach (Rigidbody gib in gibObjects)
            {
                gib.gameObject.SetActive(true);
                gib.isKinematic = false;
                gib.transform.parent = transform;
            }
            ragdoll.gameObject.SetActive(false);
        }
    }
}