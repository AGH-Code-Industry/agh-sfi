using DeadFusion.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] float viewAngle;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public bool PlayerInRange()
    {
        if (PlayerInteraction.Instance == null)
            return false;

        Vector3 dir = transform.forward;
        Vector3 playerDir = PlayerInteraction.Instance.transform.position - transform.position;
        dir.y = 0f;
        playerDir.y = 0f;

        if (Vector3.Angle(dir, playerDir) > viewAngle)
            return false;

        if (Physics.Raycast(transform.position, PlayerInteraction.Instance.transform.position - transform.position, Vector3.Distance(transform.position, PlayerInteraction.Instance.transform.position), obstacleLayer))
        {
            return false;
        }

        return true;
    }
}
