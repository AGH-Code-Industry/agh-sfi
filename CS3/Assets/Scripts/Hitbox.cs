using UnityEngine;

namespace DeadFusion.GameCore
{
    public class Hitbox : MonoBehaviour
    {
        [field: SerializeField]
        public float Multiplier { get; private set; } = 1f;
        [field: SerializeField]
        public bool IsCritical { get; private set; } = false;
    }
}