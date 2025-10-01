using UnityEngine;
using System.Collections;

public class PlayerCharacter : CharacterBase
{
    [Header("Player Specific")]
    public Camera playerCamera;
    public GameObject crosshair;
    public WeaponBase currentWeapon;
    
    [Header("Input")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string fireButton = "Fire1";
    public string reloadButton = "Reload";
    public string jumpButton = "Jump";
    
    [Header("Player Progression")]
    public int experience = 0;
    public int tokens = 0;
    public int nextLevelExperience = 1000;
    
    private CharacterController controller;
    private Vector3 movement;
    private bool isGrounded;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        InitializeCharacter();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }
    
    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleCombat();
        HandleInteraction();
    }
    
    void HandleInput()
    {
        // Movement input
        float horizontal = Input.GetAxis(horizontalAxis);
        float vertical = Input.GetAxis(verticalAxis);
        movement = new Vector3(horizontal, 0, vertical);
        
        // Combat input
        if (Input.GetButton(fireButton) && currentWeapon != null)
        {
            currentWeapon.FireWeapon();
        }
        
        if (Input.GetButtonDown(reloadButton) && currentWeapon != null)
        {
            currentWeapon.Reload();
        }
        
        // Special ability
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSpecialAbility();
        }
        
        // Jump
        if (Input.GetButtonDown(jumpButton) && isGrounded)
        {
            movement.y = 8f; // Jump force
        }
    }
    
    void HandleMovement()
    {
        // Apply gravity
        if (!isGrounded)
        {
            movement.y -= 20f * Time.deltaTime; // Gravity
        }
        
        if (controller != null && movement.magnitude > 0.1f)
        {
            Vector3 moveDirection = transform.TransformDirection(movement.normalized);
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);
            
            // Rotate towards movement direction (for third-person)
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        // Check if grounded
        isGrounded = controller.isGrounded;
    }
    
    void HandleCombat()
    {
        // Handle aiming and weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2);
        }
    }
    
    void HandleInteraction()
    {
        // Interaction with environment
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }
    
    void SwitchWeapon(int weaponIndex)
    {
        // Weapon switching logic
        Debug.Log("Switching to weapon index: " + weaponIndex);
        // Implementation depends on your weapon management system
    }
    
    void TryInteract()
    {
        // Raycast to find interactable objects
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 3f))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
    }
    
    public override void UseSpecialAbility()
    {
        // Player special ability implementation
        Debug.Log("Player using special ability: Tactical Command");
        
        // Example: Temporary speed boost and damage reduction
        StartCoroutine(TacticalCommandBoost());
    }
    
    IEnumerator TacticalCommandBoost()
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= 1.5f;
        
        Debug.Log("Tactical Command activated! Speed boosted.");
        
        yield return new WaitForSeconds(10f);
        
        moveSpeed = originalSpeed;
        Debug.Log("Tactical Command ended.");
    }
    
    public void AddExperience(int amount)
    {
        experience += amount;
        Debug.Log("Gained " + amount + " experience. Total: " + experience);
        
        CheckLevelUp();
    }
    
    public void AddTokens(int amount)
    {
        tokens += amount;
        Debug.Log("Gained " + amount + " tokens. Total: " + tokens);
    }
    
    void CheckLevelUp()
    {
        while (experience >= nextLevelExperience)
        {
            LevelUp();
            experience -= nextLevelExperience;
            nextLevelExperience = Mathf.RoundToInt(nextLevelExperience * 1.5f); // 50% increase per level
        }
    }
    
    public override void LevelUp()
    {
        base.LevelUp();
        Debug.Log("Player leveled up to level " + level + "!");
        
        // Grant skill points or other progression rewards
        AddTokens(50); // Reward tokens on level up
    }
}

// Interface for interactable objects
public interface IInteractable
{
    void Interact(PlayerCharacter player);
}
