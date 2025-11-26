using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PathFollower : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 720f;
    public Graph graph;
    List<Vector2> path = new List<Vector2>();
    int index = 0;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (path == null || path.Count == 0)
        {
            var rand = new System.Random();
            var item = graph.waypoints[rand.Next(graph.waypoints.Count)];
            MoveTo(item.transform.position);
            return;
        };
        Vector2 pos = rb.position;
        Vector2 target = path[index];
        Vector2 dir = (target - pos);
        if (dir.magnitude < 0.1f)
        {
            index++;
            if (index >= path.Count) { path.Clear(); return; }
            target = path[index];
            dir = (target - pos);
        }
        Vector2 vel = dir.normalized * moveSpeed;
        rb.MovePosition(pos + vel * Time.fixedDeltaTime);
        Debug.Log("test: " + target.x + " " + target.y);
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        rb.rotation = Mathf.MoveTowardsAngle(rb.rotation, angle, rotateSpeed * Time.fixedDeltaTime);
    }

    // Request path to world position
    public void MoveTo(Vector2 worldTarget)
    {
        if (graph == null) return;
        path = graph.FindPath(transform.position, worldTarget);
        index = 0;
    }

    public bool HasPath => path != null && path.Count > 0;
    public Vector2? CurrentTarget => (HasPath && index < path.Count) ? path[index] : (Vector2?)null;

    // debug draw
    void OnDrawGizmos()
    {
        if (path == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < path.Count - 1; i++) Gizmos.DrawLine(path[i], path[i + 1]);
    }
}
