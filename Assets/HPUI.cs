// HealthBarUI.cs
using UnityEngine;

public class HPUI : MonoBehaviour
{
    public Transform target;
    public Agent agent;
    public Vector3 offset;
    public UnityEngine.UI.Image fill;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        transform.position = cam.WorldToScreenPoint(target.position + offset);
        fill.fillAmount = agent.profile.health / agent.profile.maxHealth;
    }

    public void SetHealth(float pct)
    {
        fill.fillAmount = pct;
    }
}
