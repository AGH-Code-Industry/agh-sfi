using DeadFusion.GameCore;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyMovement enemyMovement;
    FieldOfView fieldOfView;
    EnemyAttack attack;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        fieldOfView = GetComponent<FieldOfView>();
        attack = GetComponent<EnemyAttack>();
    }

    void Update()
    {
        if (fieldOfView.PlayerInRange())
        {
            attack.Attack();
        }
        else
        {
            enemyMovement.Move();
        }
    }
}
