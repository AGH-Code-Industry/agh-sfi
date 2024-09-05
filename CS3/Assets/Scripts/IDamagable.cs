using Unity.Collections;
using UnityEngine;

namespace DeadFusion.GameCore
{
    /// <summary>
    /// The <c>IDamagable</c> interface requires Network Object and is run server side.
    /// </summary>
    public interface IDamagable
    {
        HitType OnDamage(DamageInfo info);
    }

    public enum HitType
    {
        None,
        Standard,
        Critical
    }

    [System.Serializable]
    public struct DamageInfo
    {
        public enum DamageType
        {
            Uncategorized = 0,
            Bullet = 1,
            Explosion = 2
        }

        public float amount;
        public DamageType type;
        public FixedString32Bytes weapon;
        public Vector3 normal;
        public Vector3 direction;
        public Vector3 point;
        public int colliderIndex;
    }
}