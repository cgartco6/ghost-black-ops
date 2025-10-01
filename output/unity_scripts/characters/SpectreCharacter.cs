using UnityEngine;
using System.Collections;

public class SpectreCharacter : AICharacter
{
    [Header("Spectre - Sniper Specialist")]
    public string callsign = "Spectre";
    public string role = "Sniper Specialist";
    
    [Header("Sniper Special")]
    public float zoomMultiplier = 8f;
    public float steadyAim = 0.95f;
    public float criticalHitChance = 0.3f;
    
    [Header("Base Stats")]
    public float strength = 7.0f;
    public float agility = 8.5f;
    public float intelligence = 8.0f;
    public float accuracy = 10.0f;
    public float demolitionSkill = 6.0f;
    public float hackingSkill = 7.0f;
    public float breachSkill = 7.0f;
    
    [Header("Personality")]
    public float humorFactor = 0.4f;
    public float witFactor = 0.6f;
    public float teamwork = 0.8f;
    
    private string[] wittyRemarks = new string[]
    {
        "One shot, one kill.",
        "Patience is a weapon.",
        "Target acquired... and eliminated.",
        "The quiet professionals."
    };
    
    private bool isZoomed = false;
    private float originalFOV;
    
    void Start()
    {
        InitializeCharacter();
        originalFOV = Camera.main.fieldOfView;
    }
    
    public override void InitializeCharacter()
    {
        base.InitializeCharacter();
        characterClass = CharacterClass.Sniper;
        Debug.Log("Spectre in position. Ready to provide overwatch.");
    }
    
    public override void UseSpecialAbility()
    {
        // Enter focused aim mode
        if (!isZoomed)
        {
            EnterSniperMode();
        }
        else
        {
            ExitSniperMode();
        }
    }
    
    void EnterSniperMode()
    {
        isZoomed = true;
        Camera.main.fieldOfView = originalFOV / zoomMultiplier;
        Debug.Log("Sniper mode activated. Steady aim...");
        
        // Increase accuracy and critical chance
        accuracy *= 1.2f;
        criticalHitChance *= 1.5f;
    }
    
    void ExitSniperMode()
    {
        isZoomed = false;
        Camera.main.fieldOfView = originalFOV;
        Debug.Log("Sniper mode deactivated.");
        
        // Reset stats
        accuracy = 10.0f;
        criticalHitChance = 0.3f;
    }
    
    public string GetCombatQuote()
    {
        if (wittyRemarks.Length > 0 && Random.Range(0f, 1f) < humorFactor)
        {
            return wittyRemarks[Random.Range(0, wittyRemarks.Length)];
        }
        return "Sniper ready.";
    }
    
    void Update()
    {
        if (isZoomed && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitSniperMode();
        }
    }
}
