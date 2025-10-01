using UnityEngine;
using System.Collections;

public class WraithSniper : WeaponBase
{
    [Header("Wraith Sniper - Sniper Rifle")]
    public float baseDamage = 95f;
    public float fireRate = 40f;
    public float accuracy = 0.98f;
    public float effectiveRange = 800f;
    
    [Header("Weapon Features")]
    public string[] specialFeatures = new string[]
    {
        "Thermal Scope",
        "Bipod",
        "Armor Piercing"
    };
    
    [Header("Sniper Specific")]
    public float zoomMultiplier = 8f;
    public bool hasBipodDeployed = false;
    public float armorPenetration = 0.8f;
    
    [Header("Ammunition")]
    public int ammoCapacity = 10;
    public float reloadTime = 3.5f;
    
    private bool isZoomed = false;
    private float originalFOV;
    
    void Start()
    {
        weaponName = "Wraith Sniper";
        weaponType = WeaponType.SniperRifle;
        InitializeWeapon();
        
        if (Camera.main != null)
        {
            originalFOV = Camera.main.fieldOfView;
        }
    }
    
    public override void InitializeWeapon()
    {
        base.InitializeWeapon();
        currentAmmo = ammoCapacity;
        Debug.Log("Wraith Sniper zeroed and ready");
    }
    
    public override void FireWeapon()
    {
        if (CanFire())
        {
            base.FireWeapon();
            
            // Sniper-specific firing logic
            PlaySniperEffects();
            currentAmmo--;
            
            // High damage with armor penetration
            float finalDamage = baseDamage;
            if (hasBipodDeployed)
            {
                finalDamage *= 1.2f; // 20% damage bonus with bipod
            }
            
            Debug.Log($"Sniper shot fired! Damage: {finalDamage}");
        }
    }
    
    public void ToggleZoom()
    {
        if (Camera.main != null)
        {
            isZoomed = !isZoomed;
            Camera.main.fieldOfView = isZoomed ? originalFOV / zoomMultiplier : originalFOV;
            Debug.Log("Sniper zoom " + (isZoomed ? "activated" : "deactivated"));
        }
    }
    
    public void ToggleBipod()
    {
        hasBipodDeployed = !hasBipodDeployed;
        accuracy = hasBipodDeployed ? 1.0f : 0.98f;
        Debug.Log("Bipod " + (hasBipodDeployed ? "deployed" : "retracted"));
    }
    
    void PlaySniperEffects()
    {
        // Sniper-specific effects (sound, muzzle flash, shell ejection)
        Debug.Log("Wraith Sniper: *powerful shot sound*");
    }
    
    public override void Reload()
    {
        if (!isReloading && currentAmmo < ammoCapacity)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }
    
    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading Wraith Sniper...");
        
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = ammoCapacity;
        isReloading = false;
        
        Debug.Log("Wraith Sniper reloaded and ready");
    }
    
    bool CanFire()
    {
        return !isReloading && currentAmmo > 0 && Time.time >= nextFireTime;
    }
}
