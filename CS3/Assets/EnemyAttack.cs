using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage;
    public float reloadTime;

    bool reloading = false;

    public void Attack()
    {
        if (Player.Instance == null || Player.Instance.health <= 0)
            return;

        Vector3 dir = Player.Instance.transform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);

        if (reloading)
            return;

        Player.Instance.health -= damage;
        reloading = true;
        StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        reloading = false;
    }
}
