"""
Mission Planner AI Agent - Creates missions and objectives
"""

import asyncio
import json
from typing import Dict, List, Any
from pathlib import Path

class MissionPlanner:
    def __init__(self, config: Dict, logger):
        self.config = config
        self.logger = logger
        self.missions_created = 0
        
    async def initialize(self):
        """Initialize the mission planner"""
        self.logger.info("Mission Planner initialized")
        return True
    
    async def design_game_narrative(self):
        """Design the game narrative and story"""
        self.logger.info("Designing game narrative...")
        
        game_narrative = {
            'title': 'Ghost: Black Ops',
            'synopsis': 'An elite special forces team must stop a global terrorist organization from unleashing advanced AI weapons.',
            'main_characters': {
                'player': 'Ghost - Team Leader',
                'team_members': ['Viper - Demolitions', 'Spectre - Sniper', 'Cipher - Hacker', 'Titan - Assault'],
                'antagonists': ['General Markov - Main Antagonist', 'Dr. Chen - AI Specialist', 'The Syndicate - Terror Organization']
            },
            'story_arcs': [
                {
                    'act': 'I',
                    'title': 'The Awakening',
                    'missions': 5,
                    'objective': 'Uncover the terrorist plot'
                },
                {
                    'act': 'II', 
                    'title': 'Global Hunt',
                    'missions': 8,
                    'objective': 'Track down syndicate leaders'
                },
                {
                    'act': 'III',
                    'title': 'Final Confrontation', 
                    'missions': 4,
                    'objective': 'Prevent global catastrophe'
                }
            ],
            'themes': ['Loyalty', 'Sacrifice', 'Technology vs Humanity', 'Teamwork']
        }
        
        await self._save_narrative_design(game_narrative)
        
        return {
            'agent': 'mission_planner',
            'task': 'design_game_narrative',
            'result': 'success',
            'performance': 0.89
        }
    
    async def create_mission_structure(self):
        """Create mission structure and objectives"""
        self.logger.info("Creating mission structure...")
        
        missions = [
            {
                'mission_id': 'M01',
                'name': 'Operation Silent Entry',
                'type': 'stealth_infiltration',
                'location': 'Urban Facility',
                'primary_objectives': [
                    'Infiltrate the compound undetected',
                    'Hack the security system',
                    'Retrieve intelligence data'
                ],
                'secondary_objectives': [
                    'Do not trigger alarms',
                    'Collect additional evidence',
                    'Extract without casualties'
                ],
                'enemy_types': ['Guards', 'Security Cameras', 'Drones'],
                'special_conditions': ['Night operation', 'Time limit: 30 minutes']
            },
            {
                'mission_id': 'M02',
                'name': 'Jungle Ambush',
                'type': 'direct_assault',
                'location': 'Jungle Outpost',
                'primary_objectives': [
                    'Eliminate enemy commander',
                    'Destroy weapon cache',
                    'Secure extraction point'
                ],
                'secondary_objectives': [
                    'Rescue hostages',
                    'Destroy communication array',
                    'Collect enemy intel'
                ],
                'enemy_types': ['Heavy Soldiers', 'Snipers', 'Technical Vehicles'],
                'special_conditions': ['Daytime assault', 'Reinforcements possible']
            }
        ]
        
        creation_tasks = []
        for mission in missions:
            creation_tasks.append(self._create_mission(mission))
        
        await asyncio.gather(*creation_tasks)
        
        return {
            'agent': 'mission_planner',
            'task': 'create_mission_structure', 
            'result': 'success',
            'missions_created': len(missions),
            'performance': 0.93
        }
    
    async def _create_mission(self, mission_data: Dict):
        """Create individual mission"""
        # Generate mission script
        mission_script = self._generate_mission_script(mission_data)
        
        # Save mission data
        missions_dir = Path("output/game_assets/missions")
        missions_dir.mkdir(parents=True, exist_ok=True)
        
        with open(missions_dir / f"{mission_data['mission_id']}.json", 'w') as f:
            json.dump(mission_data, f, indent=2)
        
        # Save Unity mission script
        scripts_dir = Path("output/unity_scripts/missions")
        scripts_dir.mkdir(parents=True, exist_ok=True)
        
        with open(scripts_dir / f"Mission_{mission_data['mission_id']}.cs", 'w') as f:
            f.write(mission_script)
        
        self.missions_created += 1
    
    def _generate_mission_script(self, mission_data: Dict) -> str:
        """Generate Unity C# script for mission"""
        script = f"""
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mission{mission_data['mission_id']} : MissionBase
{{
    [Header("Mission: {mission_data['name']}")]
    public string missionType = "{mission_data['type']}";
    public string location = "{mission_data['location']}";
    
    [Header("Objectives")]
    public List<string> primaryObjectives = new List<string>()
    {{
        {', '.join(f'"{obj}"' for obj in mission_data['primary_objectives'])}
    }};
    
    public List<string> secondaryObjectives = new List<string>()
    {{
        {', '.join(f'"{obj}"' for obj in mission_data['secondary_objectives'])}
    }};
    
    [Header("Enemy Forces")]
    public List<string> enemyTypes = new List<string>()
    {{
        {', '.join(f'"{enemy}"' for enemy in mission_data['enemy_types'])}
    }};
    
    [Header("Special Conditions")]
    public List<string> specialConditions = new List<string>()
    {{
        {', '.join(f'"{cond}"' for cond in mission_data['special_conditions'])}
    }};
    
    void Start()
    {{
        missionName = "{mission_data['name']}";
        missionID = "{mission_data['mission_id']}";
        InitializeMission();
    }}
    
    public override void InitializeMission()
    {{
        base.InitializeMission();
        
        // Setup mission-specific parameters
        SetTimeLimit(1800); // 30 minutes in seconds
        SetExtractionPoints(3);
        
        Debug.Log($"Mission {mission_data['mission_id']} initialized: {mission_data['name']}");
    }}
    
    public override void CompletePrimaryObjective(int objectiveIndex)
    {{
        base.CompletePrimaryObjective(objectiveIndex);
        
        // Mission-specific completion logic
        switch (objectiveIndex)
        {{
"""
        
        # Add objective-specific logic
        for i, objective in enumerate(mission_data['primary_objectives']):
            script += f"""
            case {i}:
                // {objective}
                GrantTeamExperience(500);
                break;
"""
        
        script += """
        }
    }
    
    public override void MissionSuccess()
    {
        base.MissionSuccess();
        
        // Mission success rewards
        GrantTokens(100);
        UnlockNextMission();
        
        Debug.Log("Mission accomplished! Excellent work, Ghost team.");
    }
    
    public override void MissionFailure()
    {
        base.MissionFailure();
        
        Debug.Log("Mission failed. Regroup and try again.");
    }
}
"""
        return script
    
    async def _save_narrative_design(self, narrative: Dict):
        """Save narrative design document"""
        design_dir = Path("output/game_design")
        design_dir.mkdir(parents=True, exist_ok=True)
        
        with open(design_dir / "narrative_design.json", 'w') as f:
            json.dump(narrative, f, indent=2)
    
    async def execute_primary_task(self):
        """Execute primary mission planning task"""
        return await self.create_mission_structure()
