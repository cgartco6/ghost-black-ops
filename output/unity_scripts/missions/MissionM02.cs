using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionM02 : MissionBase
{
    [Header("Mission: Jungle Strike")]
    public string missionType = "direct_assault";
    public string location = "Jungle Outpost";
    
    [Header("Objectives")]
    public List<string> primaryObjectives = new List<string>()
    {
        "Eliminate enemy commander",
        "Destroy weapon cache",
        "Secure extraction point"
    };
    
    public List<string> secondaryObjectives = new List<string>()
    {
        "Rescue hostages",
        "Destroy communication array", 
        "Collect enemy intel"
    };
    
    [Header("Enemy Forces")]
    public List<string> enemyTypes = new List<string>()
    {
        "Heavy Soldiers",
        "Snipers",
        "Technical Vehicles"
    };
    
    [Header("Special Conditions")]
    public List<string> specialConditions = new List<string>()
    {
        "Daytime assault",
        "Reinforcements possible"
    };
    
    [Header("Mission Progress")]
    public bool commanderEliminated = false;
    public bool weaponCacheDestroyed = false;
    public bool extractionSecured = false;
    public int hostagesRescued = 0;
    public int totalHostages = 3;
    
    void Start()
    {
        missionName = "Jungle Strike";
        missionID = "M02";
        missionDifficulty = MissionDifficulty.Hard;
        
        InitializeMission();
    }
    
    public override void InitializeMission()
    {
        base.InitializeMission();
        
        Debug.Log("Mission M02 initialized: Jungle Strike");
        Debug.Log("Warning: Heavy enemy presence detected. Expect reinforcements.");
    }
    
    public override void CompletePrimaryObjective(int objectiveIndex)
    {
        base.CompletePrimaryObjective(objectiveIndex);
        
        switch (objectiveIndex)
        {
            case 0:
                // Eliminate commander
                commanderEliminated = true;
                GrantTeamExperience(1000);
                Debug.Log("Enemy commander eliminated! Enemy morale decreased.");
                break;
                
            case 1:
                // Destroy weapon cache
                weaponCacheDestroyed = true;
                GrantTeamExperience(800);
                Debug.Log("Weapon cache destroyed! Enemy combat effectiveness reduced.");
                break;
                
            case 2:
                // Secure extraction
                extractionSecured = true;
                GrantTeamExperience(600);
                Debug.Log("Extraction point secured! Ready for evacuation.");
                break;
        }
        
        CheckMissionCompletion();
    }
    
    public void RescueHostage()
    {
        hostagesRescued++;
        GrantTeamExperience(200);
        Debug.Log($"Hostage rescued! ({hostagesRescued}/{totalHostages})");
        
        if (hostagesRescued >= totalHostages)
        {
            CompleteSecondaryObjective(0);
        }
    }
    
    void CheckMissionCompletion()
    {
        if (commanderEliminated && weaponCacheDestroyed && extractionSecured)
        {
            MissionSuccess();
        }
    }
    
    public override void MissionSuccess()
    {
        base.MissionSuccess();
        
        // Mission success rewards
        GrantTokens(150);
        UnlockNextMission();
        
        // Bonus for hostage rescue
        if (hostagesRescued >= totalHostages)
        {
            GrantTokens(75);
            Debug.Log("All hostages rescued! Humanitarian bonus: +75 tokens");
        }
        
        Debug.Log("Jungle Strike successful! Enemy outpost neutralized.");
    }
    
    public override void MissionFailure()
    {
        base.MissionFailure();
        
        Debug.Log("Mission failed. Enemy reinforcements have overwhelmed our position.");
    }
}
