using UnityEngine;

public abstract class HitboxComponent : MonoBehaviour
{
    public abstract void OnHit(bool fatal);
}
