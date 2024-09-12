using DeadFusion.GameCore;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamagable
{
    public int health = 100;

    public HitType OnDamage(DamageInfo info)
    {
        health -= Mathf.FloorToInt(info.amount);
        if (Mathf.FloorToInt(info.amount) > 0)
            return HitType.Standard;
        else
            return HitType.None;
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
