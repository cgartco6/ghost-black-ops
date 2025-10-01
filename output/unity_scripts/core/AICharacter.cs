using UnityEngine;
using System.Collections;

public class AICharacter : CharacterBase
{
    [Header("AI Specific")]
    public CharacterClass characterClass;
    public AITeamCoordinator teamCoordinator;
    public AIBehaviorTree behaviorTree;
    
    [Header("AI Behavior")]
    public AIState currentState = AIState.Idle;
    public float detectionRange = 15f;
    public float attackRange = 10f;
    public float stoppingDistance = 2f;
    
    [Header("Combat Behavior")]
    public Transform currentTarget;
    public Vector3 lastKnownPosition;
    public bool hasLineOfSight = false;
    
    [Header("Movement")]
    public NavMeshAgent navAgent;
    public Vector3 destination;
    public bool isMoving = false;
    
    [Header("Teamwork")]
    public bool isFollowingTeam = true;
    public bool isProvidingCover = false;
    public string currentCommand = "";
    
    public enum AIState
    {
        Idle,
        Patrol,
        Follow,
        Combat,
        Cover,
        SpecialAbility,
        Dead
    }
    
    public enum CharacterClass
    {
        Assault,
        Sniper,
        Demolitions,
        Hacker,
        Medic,
        Leader
    }
    
    void Start()
    {
        InitializeAI();
    }
    
    void InitializeAI()
    {
        // Get components
        navAgent = GetComponent<NavMeshAgent>();
        behaviorTree = GetComponent<AIBehaviorTree>();
        teamCoordinator = FindObjectOfType<AITeamCoordinator>();
        
        // Setup AI parameters
        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
            navAgent.stoppingDistance = stoppingDistance;
        }
        
        InitializeCharacter();
        ChangeState(AIState.Patrol);
        
        Debug.Log(characterName + " AI initialized as " + characterClass);
    }
    
    void Update()
    {
        if (!isAlive) return;
        
        UpdateAI();
        UpdateState();
    }
    
    void UpdateAI()
    {
        // Update target information
        UpdateTarget();
        
        // Update behavior tree
        if (behaviorTree != null)
        {
            behaviorTree.Update();
        }
    }
    
    void UpdateState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                UpdateIdleState();
                break;
                
            case AIState.Patrol:
                UpdatePatrolState();
                break;
                
            case AIState.Follow:
                UpdateFollowState();
                break;
                
            case AIState.Combat:
                UpdateCombatState();
                break;
                
            case AIState.Cover:
                UpdateCoverState();
                break;
                
            case AIState.SpecialAbility:
                UpdateSpecialAbilityState();
                break;
        }
    }
    
    void UpdateIdleState()
    {
        // Look around, check environment
        if (HasEnemyInSight())
        {
            ChangeState(AIState.Combat);
        }
        else if (isFollowingTeam)
        {
            ChangeState(AIState.Follow);
        }
    }
    
    void UpdatePatrolState()
    {
        if (!isMoving)
        {
            SetRandomDestination();
        }
        
        if (HasEnemyInSight())
        {
            ChangeState(AIState.Combat);
        }
    }
    
    void UpdateFollowState()
    {
        if (teamCoordinator != null && teamCoordinator.teamLeader != null)
        {
            Vector3 followPosition = teamCoordinator.teamLeader.transform.position;
            followPosition += GetFormationOffset();
            
            MoveToPosition(followPosition);
        }
        
        if (HasEnemyInSight())
        {
            ChangeState(AIState.Combat);
        }
    }
    
    void UpdateCombatState()
    {
        if (currentTarget == null)
        {
            ChangeState(AIState.Follow);
            return;
        }
        
        // Engage enemy
        if (Vector3.Distance(transform.position, currentTarget.position) <= attackRange && hasLineOfSight)
        {
            // Attack target
            FaceTarget(currentTarget.position);
            // Implement attack logic based on weapon
        }
        else
        {
            // Move to engage
            MoveToPosition(lastKnownPosition);
        }
    }
    
    void ChangeState(AIState newState)
    {
        if (currentState == newState) return;
        
        ExitState(currentState);
        currentState = newState;
        EnterState(newState);
        
        Debug.Log(characterName + " changing state to: " + newState);
    }
    
    void EnterState(AIState state)
    {
        switch (state)
        {
            case AIState.Combat:
                inCombat = true;
                lastCombatTime = Time.time;
                break;
                
            case AIState.SpecialAbility:
                // Prepare for special ability
                break;
        }
    }
    
    void ExitState(AIState state)
    {
        switch (state)
        {
            case AIState.Combat:
                // Clean up combat state
                break;
        }
    }
    
    public void MoveToPosition(Vector3 position)
    {
        if (navAgent != null && navAgent.isActiveAndEnabled)
        {
            navAgent.SetDestination(position);
            isMoving = true;
            destination = position;
        }
    }
    
    public void StopMovement()
    {
        if (navAgent != null)
        {
            navAgent.ResetPath();
            isMoving = false;
        }
    }
    
    public bool HasEnemyInSight()
    {
        // Implementation for enemy detection
        // This would use raycasting and layer masks
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= detectionRange)
            {
                // Check line of sight
                RaycastHit hit;
                if (Physics.Raycast(transform.position, (enemy.transform.position - transform.position).normalized, out hit, detectionRange))
                {
                    if (hit.collider.gameObject == enemy)
                    {
                        currentTarget = enemy.transform;
                        lastKnownPosition = enemy.transform.position;
                        hasLineOfSight = true;
                        return true;
                    }
                }
            }
        }
        
        hasLineOfSight = false;
        return false;
    }
    
    public void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    
    Vector3 GetFormationOffset()
    {
        // Get formation position from team coordinator
        if (teamCoordinator != null && teamCoordinator.formationPositions.ContainsKey(this))
        {
            return teamCoordinator.formationPositions[this] - teamCoordinator.teamLeader.transform.position;
        }
        
        return Vector3.zero;
    }
    
    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            MoveToPosition(hit.position);
        }
    }
    
    public void ReceiveCommand(string command)
    {
        currentCommand = command;
        
        switch (command)
        {
            case "Follow":
                ChangeState(AIState.Follow);
                break;
                
            case "Hold Position":
                ChangeState(AIState.Idle);
                break;
                
            case "Provide Cover":
                ChangeState(AIState.Cover);
                break;
                
            case "Use Ability":
                ChangeState(AIState.SpecialAbility);
                break;
        }
        
        Debug.Log(characterName + " received command: " + command);
    }
    
    public bool IsBusy()
    {
        return currentState == AIState.Combat || currentState == AIState.SpecialAbility;
    }
    
    public bool InCombat()
    {
        return inCombat;
    }
    
    // Class-specific methods
    public void FindSniperPosition()
    {
        // Sniper finds elevated position
        Debug.Log(characterName + " finding sniper position");
    }
    
    public void EngageHeavyTargets()
    {
        // Demolitions engage heavy targets
        Debug.Log(characterName + " engaging heavy targets");
    }
    
    public void DisableEnemySystems()
    {
        // Hacker disables enemy systems
        Debug.Log(characterName + " disabling enemy systems");
    }
    
    public void ProvideCoverFire()
    {
        // Assault provides covering fire
        Debug.Log(characterName + " providing cover fire");
    }
    
    public void FollowTeam()
    {
        ChangeState(AIState.Follow);
    }
    
    public void CompleteObjective()
    {
        // Move to complete mission objective
        Debug.Log(characterName + " working on objective");
    }
    
    public void ApplyTeamBoost(float multiplier, float duration)
    {
        // Apply temporary team boost
        StartCoroutine(TeamBoostRoutine(multiplier, duration));
    }
    
    IEnumerator TeamBoostRoutine(float multiplier, float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed *= multiplier;
        
        Debug.Log(characterName + " received team boost!");
        
        yield return new WaitForSeconds(duration);
        
        moveSpeed = originalSpeed;
        Debug.Log(characterName + " team boost ended.");
    }
    
    public override void UseSpecialAbility()
    {
        // Base AI special ability - override in specific AI classes
        Debug.Log(characterName + " using special ability");
        ChangeState(AIState.SpecialAbility);
    }
}
