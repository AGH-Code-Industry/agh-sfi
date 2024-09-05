using UnityEngine;

namespace DeadFusion.GameCore
{
    public class DestroyableLimb : MonoBehaviour
    {
        [field: SerializeField] public float destroyDamageThreshold;

        public void TryDestroy(float damage)
        {
            if (damage < destroyDamageThreshold)
                return;

            transform.localScale = Vector3.zero;
        }
    }
}