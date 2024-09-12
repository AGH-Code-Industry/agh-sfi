using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Range(0f, 15f)]
    [SerializeField] float speed = 1f;
    [Header("Path")]
    [SerializeField] GameObject pathParent;

    Vector3[] points;
    int currentPoint;

    private void Start()
    {
        points = new Vector3[pathParent.transform.childCount];
        for (int i = 0; i < pathParent.transform.childCount; i++)
        {
            points[i] = pathParent.transform.GetChild(i).transform.position;
        }
    }

    public void Move()
    {
        Vector3 dir = points[currentPoint] - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
        transform.position = Vector3.MoveTowards(transform.position, points[currentPoint], Time.deltaTime * speed);

        if (transform.position == points[currentPoint])
        {
            currentPoint++;
            currentPoint %= points.Length;
        }
    }
}
