using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionM01 : MissionBase
{
    [Header("Mission: Operation Silent Entry")]
    public string missionType = "stealth_infiltration";
    public string location = "Urban Facility";
    
    [Header("Objectives")]
    public List<string> primaryObjectives = new List<string>()
    {
        "Infiltrate the compound undetected",
        "Hack the security system", 
        "Retrieve intelligence data"
    };
    
    public List<string> secondaryObjectives = new List<string>()
    {
        "Do not trigger alarms",
        "Collect additional evidence",
        "Extract without casualties"
    };
    
    [Header("Enemy Forces")]
    public List<string> enemyTypes = new List<string>()
    {
        "Guards",
        "Security Cameras",
        "Drones"
    };
    
    [Header("Special Conditions")]
    public List<string> specialConditions = new List<string>()
    {
        "Night operation",
        "Time limit: 30 minutes"
    };
    
    [Header("Mission Progress")]
    public bool compoundInfiltated = false;
    public bool securityHacked = false;
    public bool dataRetrieved = false;
    public int alarmsTriggered = 0;
    
    void Start()
    {
        missionName = "Operation Silent Entry";
        missionID = "M01";
        missionDifficulty = MissionDifficulty.Medium;
        timeLimit = 1800f; // 30 minutes in seconds
        
        InitializeMission();
    }
    
    public override void InitializeMission()
    {
        base.InitializeMission();
        
        // Setup mission-specific parameters
        SetExtractionPoints(2);
        SetTimeLimit(timeLimit);
        
        Debug.Log("Mission M01 initialized: Operation Silent Entry");
        Debug.Log("Primary Objectives:");
        foreach (string objective in primaryObjectives)
        {
            Debug.Log("- " + objective);
        }
    }
    
    public override void CompletePrimaryObjective(int objectiveIndex)
    {
        base.CompletePrimaryObjective(objectiveIndex);
        
        // Mission-specific completion logic
        switch (objectiveIndex)
        {
            case 0:
                // Infiltrate compound
                compoundInfiltated = true;
                GrantTeamExperience(500);
                Debug.Log("Compound infiltrated! Proceed to security room.");
                break;
                
            case 1:
                // Hack security system
                securityHacked = true;
                GrantTeamExperience(750);
                Debug.Log("Security system hacked! Cameras and alarms disabled.");
                break;
                
            case 2:
                // Retrieve intelligence data
                dataRetrieved = true;
                GrantTeamExperience(1000);
                Debug.Log("Intelligence data retrieved! Proceed to extraction.");
                break;
        }
        
        CheckMissionCompletion();
    }
    
    public void TriggerAlarm()
    {
        alarmsTriggered++;
        Debug.Log("Alarm triggered! Enemy alert level increased.");
        
        if (alarmsTriggered >= 3)
        {
            MissionFailure();
        }
    }
    
    void CheckMissionCompletion()
    {
        if (compoundInfiltated && securityHacked && dataRetrieved)
        {
            MissionSuccess();
        }
    }
    
    public override void MissionSuccess()
    {
        base.MissionSuccess();
        
        // Mission success rewards
        GrantTokens(100);
        UnlockNextMission();
        
        // Bonus for stealth completion
        if (alarmsTriggered == 0)
        {
            GrantTokens(50);
            Debug.Log("Stealth bonus: +50 tokens!");
        }
        
        Debug.Log("Mission accomplished! Excellent stealth work, Ghost team.");
    }
    
    public override void MissionFailure()
    {
        base.MissionFailure();
        
        Debug.Log("Mission failed. The compound is on high alert. Extraction recommended.");
    }
    
    void Update()
    {
        // Mission timer and condition checks
        UpdateMissionTimer();
        
        if (missionTimer <= 0 && !missionCompleted)
        {
            MissionFailure();
        }
    }
}
