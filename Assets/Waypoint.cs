using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<Waypoint> neighbors = new List<Waypoint>();
    public float CostTo(Waypoint other) => Vector2.Distance(transform.position, other.transform.position);

    void OnDrawGizmos()
    {
        if (neighbors == null) return;

        Gizmos.color = Color.yellow;

        bool isSelected = false;

        isSelected = Selection.activeGameObject == this.gameObject;
        Gizmos.color = isSelected ? Color.red : Color.yellow;
        if (!isSelected && Selection.activeGameObject != null)
        {
            return;
        }

        foreach (var n in neighbors)
        {
            if (n == null) continue;

            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }
}
