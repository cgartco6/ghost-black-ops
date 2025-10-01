using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITeamCoordinator : MonoBehaviour
{
    [Header("Team Coordination")]
    public List<AICharacter> teamMembers;
    public PlayerCharacter teamLeader;
    public float communicationRange = 50f;
    
    [Header("Team Formation")]
    public FormationType currentFormation = FormationType.Wedge;
    public float formationSpacing = 3f;
    
    [Header("Team Status")]
    public int teamMembersAlive;
    public float teamHealthPercentage;
    public bool teamInCombat = false;
    
    private Dictionary<AICharacter, Vector3> formationPositions = new Dictionary<AICharacter, Vector3>();
    
    public enum FormationType
    {
        Wedge,       // Offensive formation
        Line,        // Defensive formation
        Column,      // Movement formation  
        Diamond,     // All-around defense
        Echelon      // Flanking formation
    }
    
    void Start()
    {
        InitializeTeam();
    }
    
    void InitializeTeam()
    {
        // Find all AI team members
        AICharacter[] allAI = FindObjectsOfType<AICharacter>();
        teamMembers = new List<AICharacter>(allAI);
        
        teamLeader = FindObjectOfType<PlayerCharacter>();
        teamMembersAlive = teamMembers.Count + 1; // +1 for player
        
        CalculateFormationPositions();
        Debug.Log($"Team coordinated initialized with {teamMembers.Count} AI members");
    }
    
    void Update()
    {
        UpdateTeamStatus();
        CoordinateTeamActions();
        UpdateFormation();
    }
    
    void UpdateTeamStatus()
    {
        teamMembersAlive = 1; // Player
        
        foreach (AICharacter member in teamMembers)
        {
            if (member.isAlive)
            {
                teamMembersAlive++;
            }
        }
        
        // Calculate average team health
        float totalHealth = teamLeader.health;
        float totalMaxHealth = teamLeader.maxHealth;
        
        foreach (AICharacter member in teamMembers)
        {
            if (member.isAlive)
            {
                totalHealth += member.health;
                totalMaxHealth += member.maxHealth;
            }
        }
        
        teamHealthPercentage = totalHealth / totalMaxHealth;
        teamInCombat = CheckTeamInCombat();
    }
    
    bool CheckTeamInCombat()
    {
        foreach (AICharacter member in teamMembers)
        {
            if (member.isAlive && member.InCombat())
            {
                return true;
            }
        }
        return false;
    }
    
    void CoordinateTeamActions()
    {
        if (teamInCombat)
        {
            HandleCombatCoordination();
        }
        else
        {
            HandleExplorationCoordination();
        }
    }
    
    void HandleCombatCoordination()
    {
        // Assign combat roles based on character specialties
        foreach (AICharacter member in teamMembers)
        {
            if (!member.isAlive) continue;
            
            switch (member.characterClass)
            {
                case CharacterClass.Sniper:
                    // Snipers find elevated positions
                    member.FindSniperPosition();
                    break;
                    
                case CharacterClass.Demolitions:
                    // Demolitions handle heavy targets
                    member.EngageHeavyTargets();
                    break;
                    
                case CharacterClass.Hacker:
                    // Hackers disable enemy systems
                    member.DisableEnemySystems();
                    break;
                    
                case CharacterClass.Assault:
                    // Assault provides covering fire
                    member.ProvideCoverFire();
                    break;
            }
        }
    }
    
    void HandleExplorationCoordination()
    {
        // Team moves in formation during exploration
        foreach (AICharacter member in teamMembers)
        {
            if (!member.isAlive) continue;
            
            member.MoveToFormationPosition(formationPositions[member]);
        }
    }
    
    void CalculateFormationPositions()
    {
        formationPositions.Clear();
        
        Vector3 leaderPosition = teamLeader.transform.position;
        Vector3 leaderForward = teamLeader.transform.forward;
        Vector3 leaderRight = teamLeader.transform.right;
        
        for (int i = 0; i < teamMembers.Count; i++)
        {
            Vector3 formationOffset = Vector3.zero;
            
            switch (currentFormation)
            {
                case FormationType.Wedge:
                    formationOffset = CalculateWedgePosition(i, leaderForward, leaderRight);
                    break;
                    
                case FormationType.Line:
                    formationOffset = CalculateLinePosition(i, leaderRight);
                    break;
                    
                case FormationType.Column:
                    formationOffset = CalculateColumnPosition(i, leaderForward);
                    break;
                    
                case FormationType.Diamond:
                    formationOffset = CalculateDiamondPosition(i, leaderForward, leaderRight);
                    break;
                    
                case FormationType.Echelon:
                    formationOffset = CalculateEchelonPosition(i, leaderForward, leaderRight);
                    break;
            }
            
            formationPositions[teamMembers[i]] = leaderPosition + formationOffset;
        }
    }
    
    Vector3 CalculateWedgePosition(int index, Vector3 forward, Vector3 right)
    {
        // Wedge formation for offensive movement
        switch (index)
        {
            case 0: return forward * formationSpacing + right * formationSpacing;
            case 1: return forward * formationSpacing - right * formationSpacing;
            case 2: return forward * formationSpacing * 2f;
            case 3: return forward * formationSpacing * 2f + right * formationSpacing * 2f;
            default: return forward * formationSpacing * (index / 2) + right * formationSpacing * (index % 2 == 0 ? 1 : -1);
        }
    }
    
    Vector3 CalculateLinePosition(int index, Vector3 right)
    {
        // Line formation for defense
        float lineOffset = (index - (teamMembers.Count - 1) / 2f) * formationSpacing;
        return right * lineOffset;
    }
    
    void UpdateFormation()
    {
        // Update formation based on situation
        if (teamInCombat)
        {
            currentFormation = FormationType.Diamond; // All-around defense in combat
        }
        else if (IsInNarrowSpace())
        {
            currentFormation = FormationType.Column; // Single file in narrow spaces
        }
        else
        {
            currentFormation = FormationType.Wedge; // Default offensive formation
        }
        
        CalculateFormationPositions();
    }
    
    bool IsInNarrowSpace()
    {
        // Check if team is in a narrow space (implementation depends on your level design)
        return Physics.Raycast(teamLeader.transform.position, teamLeader.transform.right, 2f) ||
               Physics.Raycast(teamLeader.transform.position, -teamLeader.transform.right, 2f);
    }
    
    public void ChangeFormation(FormationType newFormation)
    {
        currentFormation = newFormation;
        CalculateFormationPositions();
        Debug.Log($"Team formation changed to: {newFormation}");
    }
    
    public void IssueTeamCommand(string command)
    {
        foreach (AICharacter member in teamMembers)
        {
            if (member.isAlive)
            {
                member.ReceiveCommand(command);
            }
        }
        
        Debug.Log($"Team command issued: {command}");
    }
    
    public AICharacter GetNearestAvailableMember(Vector3 position)
    {
        AICharacter nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (AICharacter member in teamMembers)
        {
            if (!member.isAlive || member.IsBusy()) continue;
            
            float distance = Vector3.Distance(member.transform.position, position);
            if (distance < nearestDistance)
            {
                nearest = member;
                nearestDistance = distance;
            }
        }
        
        return nearest;
    }
}
