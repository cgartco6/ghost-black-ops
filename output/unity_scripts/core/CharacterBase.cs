using UnityEngine;
using System.Collections;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Character Base Properties")]
    public string characterName;
    public int level = 1;
    public float health = 100f;
    public float maxHealth = 100f;
    public bool isAlive = true;
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    
    [Header("Combat")]
    public bool inCombat = false;
    public float lastCombatTime = 0f;
    public float combatCooldown = 10f;
    
    // Events
    public System.Action<CharacterBase> OnDeath;
    public System.Action<CharacterBase, float> OnDamageTaken;
    public System.Action<CharacterBase, float> OnHealed;
    
    void Start()
    {
        InitializeCharacter();
    }
    
    public virtual void InitializeCharacter()
    {
        maxHealth = CalculateMaxHealth();
        health = maxHealth;
        isAlive = true;
        inCombat = false;
    }
    
    protected virtual float CalculateMaxHealth()
    {
        return 100f + (level * 10f);
    }
    
    public virtual void TakeDamage(float damage)
    {
        if (!isAlive) return;
        
        health -= damage;
        inCombat = true;
        lastCombatTime = Time.time;
        
        OnDamageTaken?.Invoke(this, damage);
        
        if (health <= 0)
        {
            Die();
        }
        else
        {
            Debug.Log(characterName + " took " + damage + " damage. Health: " + health + "/" + maxHealth);
        }
    }
    
    public virtual void Heal(float amount)
    {
        if (!isAlive) return;
        
        float oldHealth = health;
        health = Mathf.Min(health + amount, maxHealth);
        float actualHeal = health - oldHealth;
        
        OnHealed?.Invoke(this, actualHeal);
        
        Debug.Log(characterName + " healed for " + actualHeal + ". Health: " + health + "/" + maxHealth);
    }
    
    protected virtual void Die()
    {
        isAlive = false;
        health = 0f;
        inCombat = false;
        
        OnDeath?.Invoke(this);
        Debug.Log(characterName + " has been eliminated");
        
        // Play death animation, drop loot, etc.
        StartCoroutine(DeathSequence());
    }
    
    protected virtual IEnumerator DeathSequence()
    {
        // Basic death sequence - override for specific character death behaviors
        yield return new WaitForSeconds(3f);
        
        // Optionally destroy the game object or set to inactive
        // gameObject.SetActive(false);
    }
    
    public virtual void LevelUp()
    {
        level++;
        float oldMaxHealth = maxHealth;
        maxHealth = CalculateMaxHealth();
        
        // Heal the character by the health increase
        float healthIncrease = maxHealth - oldMaxHealth;
        health += healthIncrease;
        
        Debug.Log(characterName + " leveled up to level " + level + "! Max health: " + maxHealth);
    }
    
    public float GetHealthPercentage()
    {
        return health / maxHealth;
    }
    
    public bool IsInCombat()
    {
        return inCombat && (Time.time - lastCombatTime) < combatCooldown;
    }
    
    void Update()
    {
        // Check combat status
        if (inCombat && (Time.time - lastCombatTime) >= combatCooldown)
        {
            inCombat = false;
            Debug.Log(characterName + " is no longer in combat");
        }
    }
    
    public abstract void UseSpecialAbility();
}
