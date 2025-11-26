using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Perception : MonoBehaviour
{
    public float viewDistance = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public LayerMask agentMask;
    public float checkInterval = 0.1f;

    public event Action<Agent> OnAgentSeen;
    public event Action<Agent> OnAgentLost;

    HashSet<Agent> seen = new HashSet<Agent>();
    float timer = 0f;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) { timer = checkInterval; Scan(); }
    }

    void Scan()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, viewDistance, agentMask);
        var currentlySeen = new HashSet<Agent>();
        foreach (var c in hits)
        {
            Agent a = c.GetComponent<Agent>();
            if (a == null) continue;
            if (a.gameObject == gameObject) continue;
            Vector2 dir = (a.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.right, dir); // assume forward is right
            if (angle > viewAngle * 0.5f) continue;

            RaycastHit2D hit = Physics2D.Linecast(transform.position, a.transform.position, obstacleMask);
            if (hit.collider != null) continue;
            currentlySeen.Add(a);
            if (!seen.Contains(a))
            {
                seen.Add(a);
            }
            OnAgentSeen?.Invoke(a);
        }

        var toRemove = new List<Agent>();
        foreach (var s in seen) if (!currentlySeen.Contains(s)) toRemove.Add(s);
        foreach (var r in toRemove)
        {
            seen.Remove(r);
            OnAgentLost?.Invoke(r);
        }
    }

    public bool Sees(Agent other) => seen.Contains(other);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
        Vector3 fwd = transform.right * viewDistance;
        Quaternion left = Quaternion.Euler(0, 0, viewAngle / 2f);
        Quaternion right = Quaternion.Euler(0, 0, -viewAngle / 2f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + left * fwd);
        Gizmos.DrawLine(transform.position, transform.position + right * fwd);


        Gizmos.color = Color.magenta;
        foreach (var a in seen)
        {
            if (a == null) continue;
            Gizmos.DrawLine(transform.position, a.transform.position);
            Gizmos.DrawSphere(a.transform.position, 0.15f);
        }
    }
}