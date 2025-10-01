using UnityEngine;
using System.Collections;

public class PhantomRifle : WeaponBase
{
    [Header("Phantom Rifle - Assault Rifle")]
    public float baseDamage = 45f;
    public float fireRate = 650f;
    public float accuracy = 0.85f;
    public float effectiveRange = 300f;
    
    [Header("Weapon Features")]
    public string[] specialFeatures = new string[]
    {
        "Silenced",
        "Custom Optics",
        "Underbarrel Launcher"
    };
    
    [Header("Ammunition")]
    public int ammoCapacity = 30;
    public float reloadTime = 2.5f;
    
    private bool isSilenced = true;
    private bool hasCustomOptics = true;
    
    void Start()
    {
        weaponName = "Phantom Rifle";
        weaponType = WeaponType.AssaultRifle;
        InitializeWeapon();
    }
    
    public override void InitializeWeapon()
    {
        base.InitializeWeapon();
        
        currentAmmo = ammoCapacity;
        isReloading = false;
        
        Debug.Log("Phantom Rifle initialized and ready for combat");
    }
    
    public override void FireWeapon()
    {
        if (CanFire())
        {
            base.FireWeapon();
            
            // Weapon-specific firing logic
            ApplyRecoil();
            PlayFireEffects();
            currentAmmo--;
            
            Debug.Log("Firing Phantom Rifle" + (isSilenced ? " (Silenced)" : ""));
        }
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
        Debug.Log("Reloading Phantom Rifle...");
        
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = ammoCapacity;
        isReloading = false;
        
        Debug.Log("Phantom Rifle reloaded!");
    }
    
    void ApplyRecoil()
    {
        // Apply weapon-specific recoil pattern
        float recoilAmount = 0.5f * (1f - accuracy);
        // Implementation depends on your camera/recoil system
    }
    
    void PlayFireEffects()
    {
        // Play muzzle flash, sound, etc.
        if (isSilenced)
        {
            // Play silenced sound
        }
        else
        {
            // Play loud sound
        }
    }
    
    public void ToggleSilencer()
    {
        isSilenced = !isSilenced;
        Debug.Log("Silencer " + (isSilenced ? "activated" : "deactivated"));
    }
    
    public void ApplySpecialFeature(string feature)
    {
        switch (feature)
        {
            case "Silenced":
                ToggleSilencer();
                break;
            case "Custom Optics":
                EnableCustomOptics();
                break;
            case "Underbarrel Launcher":
                FireUnderbarrel();
                break;
        }
    }
    
    void EnableCustomOptics()
    {
        hasCustomOptics = true;
        accuracy *= 1.1f; // 10% accuracy boost
        Debug.Log("Custom optics activated");
    }
    
    void FireUnderbarrel()
    {
        // Implementation for underbarrel grenade launcher
        Debug.Log("Underbarrel launcher fired!");
    }
    
    bool CanFire()
    {
        return !isReloading && currentAmmo > 0 && Time.time >= nextFireTime;
    }
}
