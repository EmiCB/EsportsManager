using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]

public class Graph : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();

    void OnValidate()
    {
        // keep list updated in editor
        waypoints.Clear();
        foreach (var wp in GetComponentsInChildren<Waypoint>()) waypoints.Add(wp);
    }

    // A* shortest path between two world positions (find nearest waypoints)
    public List<Vector2> FindPath(Vector2 fromPos, Vector2 toPos)
    {
        if (waypoints.Count == 0) return new List<Vector2> { toPos };

        Waypoint start = Nearest(fromPos);
        Waypoint goal = Nearest(toPos);
        var closed = new HashSet<Waypoint>();
        var gScore = new Dictionary<Waypoint, float>();
        var fScore = new Dictionary<Waypoint, float>();
        var cameFrom = new Dictionary<Waypoint, Waypoint>();
        var open = new List<Waypoint>() { start };

        foreach (var w in waypoints) { gScore[w] = float.PositiveInfinity; fScore[w] = float.PositiveInfinity; }
        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (open.Count > 0)
        {
            open.Sort((a, b) => fScore[a].CompareTo(fScore[b]));
            var current = open[0];
            if (current == goal) return ReconstructPath(cameFrom, current, fromPos, toPos);
            open.RemoveAt(0);
            closed.Add(current);

            foreach (var n in current.neighbors)
            {
                if (closed.Contains(n)) continue;
                float tentative = gScore[current] + current.CostTo(n);
                if (!open.Contains(n)) open.Add(n);
                else if (tentative >= gScore[n]) continue;
                cameFrom[n] = current;
                gScore[n] = tentative;
                fScore[n] = gScore[n] + Heuristic(n, goal);
            }
        }
        // fallback: straight line
        return new List<Vector2> { toPos };
    }

    Waypoint Nearest(Vector2 p)
    {
        Waypoint best = null; float bestD = float.MaxValue;
        foreach (var w in waypoints)
        {
            float d = Vector2.Distance(p, w.transform.position);
            if (d < bestD) { best = w; bestD = d; }
        }
        return best;
    }

    float Heuristic(Waypoint a, Waypoint b) => Vector2.Distance(a.transform.position, b.transform.position);

    /*    List<Vector2> ReconstructPath(Dictionary<Waypoint, Waypoint> cameFrom, Waypoint current, Vector2 fromPos, Vector2 toPos)
        {
            var path = new List<Vector2>();
            path.Add(toPos);
            var node = current;
            while (cameFrom.ContainsKey(node))
            {
                path.Add(node.transform.position);
                node = cameFrom[node];
            }
            path.Add(node.transform.position);
            path.Reverse();
            // Optionally insert start position for smoothness:
            path.Insert(0, fromPos);
            return path;
        }*/

    List<Vector2> ReconstructPath(
        Dictionary<Waypoint, Waypoint> cameFrom,
        Waypoint current,
        Vector2 fromPos,
        Vector2 toPos,
        float variance = 0.20f // tweak this to control randomness
    )
    {
        var path = new List<Vector2>();

        // Add goal with jitter
        path.Add(toPos + Random.insideUnitCircle * variance);

        var node = current;

        while (cameFrom.ContainsKey(node))
        {
            Vector2 pos = node.transform.position;

            // Add jitter so agents don't walk identical lines
            pos += Random.insideUnitCircle * variance;

            path.Add(pos);
            node = cameFrom[node];
        }

        // Start node
        Vector2 startPos = node.transform.position;
        startPos += Random.insideUnitCircle * variance;
        path.Add(startPos);

        path.Reverse();

        // Insert exact start position (optional)
        path.Insert(0, fromPos);

        return path;
    }
}