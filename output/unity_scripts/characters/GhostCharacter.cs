using UnityEngine;
using System.Collections;

public class GhostCharacter : PlayerCharacter
{
    [Header("Ghost - Team Leader")]
    public string callsign = "Ghost";
    public string role = "Team Leader";
    
    [Header("Base Stats")]
    public float strength = 8.5f;
    public float agility = 9.0f;
    public float intelligence = 8.0f;
    public float accuracy = 9.5f;
    public float demolitionSkill = 7.0f;
    public float hackingSkill = 6.5f;
    public float breachSkill = 8.0f;
    
    [Header("Personality")]
    public float humorFactor = 0.7f;
    public float witFactor = 0.9f;
    public float teamwork = 1.0f;
    
    [Header("Special Abilities")]
    public string[] specialAbilities = new string[]
    {
        "Tactical Command",
        "Enhanced Perception", 
        "Combat Instincts"
    };
    
    private string[] wittyRemarks = new string[]
    {
        "I'm not saying I'm Batman, but have you ever seen us in the same room?",
        "My accuracy is 110%. The extra 10% is for style points.",
        "They told me I couldn't blow up the sun. Challenge accepted.",
        "I hack reality. Computers are just practice."
    };
    
    void Start()
    {
        InitializeCharacter();
    }
    
    public override void InitializeCharacter()
    {
        base.InitializeCharacter();
        maxHealth = CalculateMaxHealth();
        health = maxHealth;
        Debug.Log("Ghost reporting for duty. Ready to lead!");
    }
    
    protected override float CalculateMaxHealth()
    {
        return 100f + (strength * 5f) + (level * 3f);
    }
    
    public string GetCombatQuote()
    {
        if (wittyRemarks.Length > 0 && Random.Range(0f, 1f) < humorFactor)
        {
            return wittyRemarks[Random.Range(0, wittyRemarks.Length)];
        }
        return "Team, move out!";
    }
    
    public override void UseSpecialAbility()
    {
        // Tactical Command - Boost team performance
        Debug.Log("Ghost using Tactical Command: Team performance boosted!");
        
        // Find all AI team members and boost their stats
        AICharacter[] teamMembers = FindObjectsOfType<AICharacter>();
        foreach (AICharacter member in teamMembers)
        {
            member.ApplyTeamBoost(1.2f, 10f); // 20% boost for 10 seconds
        }
    }
    
    public void IssueTeamCommand(string command)
    {
        // Issue commands to AI team members
        AICharacter[] teamMembers = FindObjectsOfType<AICharacter>();
        foreach (AICharacter member in teamMembers)
        {
            member.ReceiveCommand(command);
        }
    }
}
