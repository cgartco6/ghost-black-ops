using UnityEngine;
using System.Collections;

public class CombatSystem : MonoBehaviour
{
    [Header("Combat Settings")]
    public LayerMask enemyLayerMask;
    public LayerMask obstacleLayerMask;
    public float criticalHitMultiplier = 2f;
    public float headshotMultiplier = 3f;
    public float limbDamageMultiplier = 0.7f;
    
    [Header("Damage System")]
    public float baseHeadshotChance = 0.1f;
    public float baseCriticalChance = 0.05f;
    
    public static CombatSystem Instance;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ProcessWeaponDamage(WeaponBase weapon, CharacterBase target, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (target == null || !target.isAlive) return;
        
        // Calculate base damage
        float baseDamage = weapon.baseDamage;
        
        // Determine hit location
        HitLocation hitLocation = DetermineHitLocation(target, hitPoint);
        
        // Calculate final damage with modifiers
        float finalDamage = CalculateFinalDamage(baseDamage, hitLocation, weapon.accuracy);
        
        // Apply damage to target
        target.TakeDamage(finalDamage);
        
        // Visual and audio effects
        PlayHitEffects(hitPoint, hitNormal, hitLocation, finalDamage);
        
        Debug.Log($"Combat: {weapon.weaponName} dealt {finalDamage} damage to {target.characterName} ({hitLocation})");
    }
    
    public void ProcessMeleeDamage(CharacterBase attacker, CharacterBase target, float baseDamage)
    {
        if (target == null || !target.isAlive) return;
        
        // Melee damage calculation
        float finalDamage = baseDamage;
        
        // Strength modifier for melee
        if (attacker is AICharacter aiAttacker)
        {
            finalDamage *= aiAttacker.strength * 0.1f;
        }
        
        target.TakeDamage(finalDamage);
        
        Debug.Log($"Melee: {attacker.characterName} dealt {finalDamage} damage to {target.characterName}");
    }
    
    HitLocation DetermineHitLocation(CharacterBase target, Vector3 hitPoint)
    {
        // Simple hit location determination
        // In a real game, you'd use ragdoll colliders or bone structure
        
        Vector3 targetTop = target.transform.position + Vector3.up * 2f;
        Vector3 targetCenter = target.transform.position + Vector3.up * 1f;
        Vector3 targetBottom = target.transform.position;
        
        float hitHeight = hitPoint.y - target.transform.position.y;
        
        if (hitHeight > 1.5f)
        {
            return HitLocation.Head;
        }
        else if (hitHeight > 0.8f)
        {
            return HitLocation.Body;
        }
        else if (hitHeight > 0.3f)
        {
            return HitLocation.Limb;
        }
        else
        {
            return HitLocation.Leg;
        }
    }
    
    float CalculateFinalDamage(float baseDamage, HitLocation hitLocation, float accuracy)
    {
        float damage = baseDamage;
        
        // Hit location modifiers
        switch (hitLocation)
        {
            case HitLocation.Head:
                damage *= headshotMultiplier;
                break;
                
            case HitLocation.Body:
                // Standard damage
                break;
                
            case HitLocation.Limb:
                damage *= limbDamageMultiplier;
                break;
                
            case HitLocation.Leg:
                damage *= limbDamageMultiplier;
                // Optional: Apply movement penalty
                break;
        }
        
        // Accuracy affects critical chance
        float criticalChance = baseCriticalChance + (accuracy * 0.1f);
        if (Random.Range(0f, 1f) < criticalChance)
        {
            damage *= criticalHitMultiplier;
            Debug.Log("Critical hit!");
        }
        
        // Random variance (±10%)
        damage *= Random.Range(0.9f, 1.1f);
        
        return Mathf.Round(damage);
    }
    
    void PlayHitEffects(Vector3 position, Vector3 normal, HitLocation location, float damage)
    {
        // Play blood effects, sound, etc.
        GameObject hitEffect = new GameObject("HitEffect");
        hitEffect.transform.position = position;
        hitEffect.transform.rotation = Quaternion.LookRotation(normal);
        
        // Different effects based on hit location and damage
        if (location == HitLocation.Head)
        {
            // Headshot effects
            Debug.Log("HEADSHOT!");
        }
        
        if (damage >= 75f)
        {
            // Heavy damage effects
            Debug.Log("Heavy impact!");
        }
        
        // Destroy effect after a short time
        Destroy(hitEffect, 2f);
    }
    
    public bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(from, direction.normalized, out hit, distance, obstacleLayerMask))
        {
            // Check if hit is the target or an obstacle
            return hit.collider.transform.position == to;
        }
        
        return true;
    }
    
    public float CalculateCoverEffectiveness(Vector3 position, Vector3 coverPosition, Vector3 threatPosition)
    {
        // Calculate how effective cover is against a threat
        Vector3 toThreat = threatPosition - position;
        Vector3 toCover = coverPosition - position;
        
        float angle = Vector3.Angle(toCover, toThreat);
        
        // Cover is most effective when directly between position and threat
        float effectiveness = 1f - (angle / 180f);
        
        return Mathf.Clamp01(effectiveness);
    }
    
    public enum HitLocation
    {
        Head,
        Body,
        Limb,
        Leg
    }
}
