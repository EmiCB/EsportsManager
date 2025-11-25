using System;
using UnityEngine;

[RequireComponent(typeof(PathFollower)/*, typeof(Perception), typeof(CombatSystem)*/)]
public class Agent : MonoBehaviour
{
    public string agentName = "T";
    public Team team = Team.Terrorist;
    public AgentProfile profile;
    public PathFollower follower;
    public Vector2 target;
    //public Perception perception;
    //public CombatSystem combat;

    void Awake()
    {
        follower = GetComponent<PathFollower>();
        //perception = GetComponent<Perception>();
        //combat = GetComponent<CombatSystem>();

        follower.MoveTo(target);
    }
}

public enum Team { Terrorist, CounterTerrorist }

[System.Serializable]
public class AgentProfile
{
    public float health = 100f;
    //public Weapon weapon;
    // Add accuracy modifiers, reaction time etc.
}
