using UnityEngine;
using System.Collections;

public class TitanCharacter : AICharacter
{
    [Header("Titan - Assault Specialist")]
    public string callsign = "Titan";
    public string role = "Assault Specialist";
    
    [Header("Assault Special")]
    public float adrenalineRush = 1.5f;
    public int extraGrenades = 3;
    public float closeCombatBonus = 2f;
    
    [Header("Base Stats")]
    public float strength = 10.0f;
    public float agility = 7.0f;
    public float intelligence = 6.5f;
    public float accuracy = 8.0f;
    public float demolitionSkill = 7.5f;
    public float hackingSkill = 5.0f;
    public float breachSkill = 9.0f;
    
    [Header("Personality")]
    public float humorFactor = 0.7f;
    public float witFactor = 0.5f;
    public float teamwork = 0.95f;
    
    private string[] wittyRemarks = new string[]
    {
        "I'll draw their fire!",
        "Nothing stops the Titan!",
        "Heavy ordinance coming through!",
        "When in doubt, use more firepower."
    };
    
    private bool adrenalineActive = false;
    private float adrenalineTimeRemaining = 0f;
    
    void Start()
    {
        InitializeCharacter();
    }
    
    public override void InitializeCharacter()
    {
        base.InitializeCharacter();
        characterClass = CharacterClass.Assault;
        maxHealth = CalculateMaxHealth();
        health = maxHealth;
        Debug.Log("Titan ready! Let them come!");
    }
    
    protected override float CalculateMaxHealth()
    {
        return 120f + (strength * 6f) + (level * 4f);
    }
    
    public override void UseSpecialAbility()
    {
        // Activate adrenaline rush
        if (!adrenalineActive)
        {
            ActivateAdrenalineRush();
        }
    }
    
    void ActivateAdrenalineRush()
    {
        adrenalineActive = true;
        adrenalineTimeRemaining = 15f; // 15 seconds
        
        // Apply stat bonuses
        moveSpeed *= adrenalineRush;
        strength *= 1.3f;
        closeCombatBonus *= 1.5f;
        
        Debug.Log("Adrenaline rush activated! Let's dance!");
        
        StartCoroutine(AdrenalineCountdown());
    }
    
    IEnumerator AdrenalineCountdown()
    {
        while (adrenalineTimeRemaining > 0)
        {
            adrenalineTimeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        DeactivateAdrenalineRush();
    }
    
    void DeactivateAdrenalineRush()
    {
        adrenalineActive = false;
        
        // Revert stat changes
        moveSpeed /= adrenalineRush;
        strength /= 1.3f;
        closeCombatBonus /= 1.5f;
        
        Debug.Log("Adrenaline rush ended.");
    }
    
    public string GetCombatQuote()
    {
        if (wittyRemarks.Length > 0 && Random.Range(0f, 1f) < humorFactor)
        {
            return wittyRemarks[Random.Range(0, wittyRemarks.Length)];
        }
        return "Assault ready!";
    }
    
    void Update()
    {
        if (adrenalineActive)
        {
            // Visual effects for adrenaline rush
            // (Implementation depends on your visual effects system)
        }
    }
}
