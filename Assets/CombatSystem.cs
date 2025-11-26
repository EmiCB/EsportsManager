using UnityEngine;
using System;

public class CombatSystem : MonoBehaviour
{
    public Agent agent;
    public float lastEngageTime;
    public float engagementCooldown = 0.0f; // how often to attempt to shoot

    void Awake()
    {
        agent = GetComponent<Agent>();
    }

    void OnEnable()
    {
        agent.perception.OnAgentSeen += HandleSeen;
    }

    void OnDisable()
    {
        if (agent?.perception != null) agent.perception.OnAgentSeen -= HandleSeen;
    }

    void HandleSeen(Agent other)
    {
        // optionally only respond to enemies
        if (other.team == agent.team) return;
        TryEngage(other);
    }

    void TryEngage(Agent target)
    {
        if (Time.time - lastEngageTime < engagementCooldown) return;
        lastEngageTime = Time.time;
        // compute dice roll
        var w = agent.profile.weapon;
        float distance = Vector2.Distance(transform.position, target.transform.position);
        // basic hit chance formula: base * (weaponRangeFactor) * skill * cover modifier
        float baseChance = w.baseHitChance;
        float rangeFactor = Mathf.Clamp01(1f - (distance / w.maxEffectiveRange));
        float finalChance = baseChance * (0.5f + 0.5f * rangeFactor); // simple curve

        int diceCount = w.diceCount;
        int sum = 0;
        for (int i = 0; i < diceCount; i++) sum += UnityEngine.Random.Range(1, 7);
        int threshold = Mathf.RoundToInt(finalChance * diceCount * 6);
        bool hit = sum >= threshold;

        if (hit)
        {
            float damage = w.damage;
            damage *= (0.8f + 0.2f * rangeFactor) * 0.1f;
            target.profile.health -= damage;
            if (target.profile.health <= 0)
            {
                target.gameObject.SetActive(false);
                // TODO geoffrey
            }
        }
    }
}
