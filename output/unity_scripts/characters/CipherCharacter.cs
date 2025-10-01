using UnityEngine;
using System.Collections;

public class CipherCharacter : AICharacter
{
    [Header("Cipher - Hacking Specialist")]
    public string callsign = "Cipher";
    public string role = "Hacking Specialist";
    
    [Header("Hacking Special")]
    public float hackSpeed = 1.5f;
    public int maxActiveHacks = 3;
    public float firewallPenetration = 0.8f;
    
    [Header("Base Stats")]
    public float strength = 5.5f;
    public float agility = 7.0f;
    public float intelligence = 10.0f;
    public float accuracy = 6.5f;
    public float demolitionSkill = 5.0f;
    public float hackingSkill = 10.0f;
    public float breachSkill = 7.5f;
    
    [Header("Personality")]
    public float humorFactor = 0.6f;
    public float witFactor = 0.9f;
    public float teamwork = 0.7f;
    
    private string[] wittyRemarks = new string[]
    {
        "I speak firewall.",
        "Your security is my playground.",
        "Firewalls are just suggestions.",
        "Let me renegotiate their security parameters."
    };
    
    private int currentActiveHacks = 0;
    
    void Start()
    {
        InitializeCharacter();
    }
    
    public override void InitializeCharacter()
    {
        base.InitializeCharacter();
        characterClass = CharacterClass.Hacker;
        Debug.Log("Cipher online. Systems are my specialty.");
    }
    
    public override void UseSpecialAbility()
    {
        // Initiate system hack
        if (currentActiveHacks < maxActiveHacks)
        {
            Debug.Log("Cipher initiating system override!");
            StartCoroutine(PerformSystemHack());
        }
        else
        {
            Debug.Log("Maximum active hacks reached!");
        }
    }
    
    IEnumerator PerformSystemHack()
    {
        currentActiveHacks++;
        Debug.Log("Hacking in progress...");
        
        // Simulate hacking time
        float hackTime = 3f / hackSpeed;
        yield return new WaitForSeconds(hackTime);
        
        // Hack completed
        Debug.Log("System override successful!");
        ApplyHackEffects();
        
        currentActiveHacks--;
    }
    
    void ApplyHackEffects()
    {
        // Disable enemy systems, open doors, etc.
        GameObject[] securitySystems = GameObject.FindGameObjectsWithTag("Security");
        foreach (GameObject system in securitySystems)
        {
            // Disable security system
            system.SetActive(false);
        }
        
        Debug.Log("Security systems disabled. Proceed with caution.");
    }
    
    public string GetCombatQuote()
    {
        if (wittyRemarks.Length > 0 && Random.Range(0f, 1f) < humorFactor)
        {
            return wittyRemarks[Random.Range(0, wittyRemarks.Length)];
        }
        return "Hacking systems.";
    }
}
