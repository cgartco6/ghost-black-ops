using UnityEngine;
using System.Collections;

public class ViperCharacter : AICharacter
{
    [Header("Viper - Demolitions Expert")]
    public string callsign = "Viper";
    public string role = "Demolitions Expert";
    
    [Header("Demolitions Special")]
    public int maxExplosives = 5;
    public float explosionRadius = 10f;
    public float demolitionBonus = 1.5f;
    
    [Header("Base Stats")]
    public float strength = 9.0f;
    public float agility = 6.5f;
    public float intelligence = 7.0f;
    public float accuracy = 7.5f;
    public float demolitionSkill = 10.0f;
    public float hackingSkill = 5.0f;
    public float breachSkill = 8.5f;
    
    [Header("Personality")]
    public float humorFactor = 0.8f;
    public float witFactor = 0.7f;
    public float teamwork = 0.9f;
    
    private string[] wittyRemarks = new string[]
    {
        "I make things go boom!",
        "That's not a problem, it's a target.",
        "Stand back, this might get loud!",
        "Explosives are my love language."
    };
    
    void Start()
    {
        InitializeCharacter();
    }
    
    public override void InitializeCharacter()
    {
        base.InitializeCharacter();
        characterClass = CharacterClass.Demolitions;
        Debug.Log("Viper here. Things are about to get explosive!");
    }
    
    public override void UseSpecialAbility()
    {
        // Place explosive charge
        Debug.Log("Viper placing explosive charge!");
        
        // Implementation for explosive placement
        if (maxExplosives > 0)
        {
            PlaceExplosiveCharge();
            maxExplosives--;
        }
    }
    
    void PlaceExplosiveCharge()
    {
        // Create explosive charge at position
        GameObject explosive = new GameObject("ExplosiveCharge");
        explosive.transform.position = transform.position + transform.forward * 2f;
        
        // Add explosive component
        ExplosiveCharge charge = explosive.AddComponent<ExplosiveCharge>();
        charge.explosionRadius = explosionRadius;
        charge.damage = 100f * demolitionBonus;
        charge.detonationTime = 5f;
        
        Debug.Log("Explosive charge placed! Detonating in 5 seconds.");
    }
    
    public string GetCombatQuote()
    {
        if (wittyRemarks.Length > 0 && Random.Range(0f, 1f) < humorFactor)
        {
            return wittyRemarks[Random.Range(0, wittyRemarks.Length)];
        }
        return "Demolitions ready!";
    }
}
