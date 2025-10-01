"""
Character Creator AI Agent - Creates player and AI characters
"""

import asyncio
import json
from typing import Dict, List, Any
from dataclasses import dataclass

@dataclass
class CharacterAttributes:
    strength: float
    agility: float
    intelligence: float
    accuracy: float
    demolition: float
    hacking: float
    breach: float

@dataclass
class CharacterPersonality:
    humor_level: float
    wit_level: float
    teamwork: float
    aggression: float
    loyalty: float

class CharacterCreator:
    def __init__(self, config: Dict, logger):
        self.config = config
        self.logger = logger
        self.characters_created = 0
        self.specializations = ['assault', 'sniper', 'demolitions', 'hacker', 'medic']
        
    async def initialize(self):
        """Initialize the character creator"""
        self.logger.info("Character Creator initialized")
        return True
    
    async def design_character_system(self):
        """Design the complete character system"""
        self.logger.info("Designing character system...")
        
        character_system = {
            'system_name': 'Ghost: Black Ops Character System',
            'base_attributes': ['strength', 'agility', 'intelligence', 'accuracy'],
            'special_skills': ['demolition', 'hacking', 'breach', 'stealth'],
            'personality_traits': ['humor', 'wit', 'teamwork', 'loyalty'],
            'progression_system': {
                'level_cap': 50,
                'skill_points_per_level': 5,
                'prestige_levels': 5
            }
        }
        
        # Save character system design
        await self._save_character_system(character_system)
        
        return {
            'agent': 'character_creator',
            'task': 'design_character_system',
            'result': 'success',
            'characters_designed': len(character_system['base_attributes']),
            'performance': 0.95
        }
    
    async def create_player_character(self):
        """Create the main player character"""
        self.logger.info("Creating player character...")
        
        player_character = {
            'name': 'Ghost',
            'role': 'Team Leader',
            'attributes': CharacterAttributes(
                strength=8.5,
                agility=9.0,
                intelligence=8.0,
                accuracy=9.5,
                demolition=7.0,
                hacking=6.5,
                breach=8.0
            ).__dict__,
            'personality': CharacterPersonality(
                humor_level=0.7,
                wit_level=0.9,
                teamwork=1.0,
                aggression=0.6,
                loyalty=1.0
            ).__dict__,
            'special_abilities': ['Tactical Command', 'Enhanced Perception', 'Combat Instincts'],
            'backstory': 'Former special forces operator with exceptional leadership skills and tactical brilliance.'
        }
        
        # Generate Unity C# script for player character
        unity_script = self._generate_unity_character_script(player_character, is_player=True)
        await self._save_character_assets(player_character, unity_script, 'player_ghost')
        
        self.characters_created += 1
        
        return {
            'agent': 'character_creator',
            'task': 'create_player_character',
            'result': 'success',
            'character_name': player_character['name'],
            'performance': 0.98
        }
    
    async def create_ai_team_members(self):
        """Create AI team members"""
        self.logger.info("Creating AI team members...")
        
        team_members = [
            {
                'callsign': 'Viper',
                'role': 'Demolitions Expert',
                'specialization': 'demolitions',
                'attributes': CharacterAttributes(9.0, 6.5, 7.0, 7.5, 10.0, 5.0, 8.5).__dict__,
                'personality': CharacterPersonality(0.8, 0.7, 0.9, 0.8, 0.9).__dict__,
                'quotes': ["I make things go boom!", "That's not a problem, it's a target."]
            },
            {
                'callsign': 'Spectre', 
                'role': 'Sniper Specialist',
                'specialization': 'sniper',
                'attributes': CharacterAttributes(7.0, 8.5, 8.0, 10.0, 6.0, 7.0, 7.0).__dict__,
                'personality': CharacterPersonality(0.4, 0.6, 0.8, 0.3, 0.95).__dict__,
                'quotes': ["One shot, one kill.", "Patience is a weapon."]
            },
            {
                'callsign': 'Cipher',
                'role': 'Hacking Specialist', 
                'specialization': 'hacker',
                'attributes': CharacterAttributes(5.5, 7.0, 10.0, 6.5, 5.0, 10.0, 7.5).__dict__,
                'personality': CharacterPersonality(0.6, 0.9, 0.7, 0.4, 0.8).__dict__,
                'quotes': ["I speak firewall.", "Your security is my playground."]
            },
            {
                'callsign': 'Titan',
                'role': 'Assault Specialist',
                'specialization': 'assault', 
                'attributes': CharacterAttributes(10.0, 7.0, 6.5, 8.0, 7.5, 5.0, 9.0).__dict__,
                'personality': CharacterPersonality(0.7, 0.5, 0.95, 0.9, 1.0).__dict__,
                'quotes': ["I'll draw their fire!", "Nothing stops the Titan!"]
            }
        ]
        
        creation_tasks = []
        for member in team_members:
            creation_tasks.append(self._create_ai_team_member(member))
        
        results = await asyncio.gather(*creation_tasks)
        
        return {
            'agent': 'character_creator',
            'task': 'create_ai_team_members', 
            'result': 'success',
            'team_members_created': len(team_members),
            'performance': 0.92
        }
    
    async def _create_ai_team_member(self, member_data: Dict):
        """Create individual AI team member"""
        unity_script = self._generate_unity_character_script(member_data, is_player=False)
        await self._save_character_assets(member_data, unity_script, f"ai_{member_data['callsign'].lower()}")
        self.characters_created += 1
    
    def _generate_unity_character_script(self, character_data: Dict, is_player: bool) -> str:
        """Generate Unity C# script for character"""
        character_class = "PlayerCharacter" if is_player else "AICharacter"
        
        script = f"""
using UnityEngine;
using System.Collections;

public class {character_data['callsign'] if not is_player else 'Ghost'}Character : {character_class}
{{
    [Header("Character Attributes")]
    public string callsign = "{character_data['callsign'] if not is_player else 'Ghost'}";
    public string role = "{character_data['role']}";
    
    [Header("Base Stats")]
    public float strength = {character_data['attributes']['strength']}f;
    public float agility = {character_data['attributes']['agility']}f;
    public float intelligence = {character_data['attributes']['intelligence']}f;
    public float accuracy = {character_data['attributes']['accuracy']}f;
    public float demolitionSkill = {character_data['attributes']['demolition']}f;
    public float hackingSkill = {character_data['attributes']['hacking']}f;
    public float breachSkill = {character_data['attributes']['breach']}f;
    
    [Header("Personality")]
    public float humorFactor = {character_data['personality']['humor_level']}f;
    public float witFactor = {character_data['personality']['wit_level']}f;
    public float teamwork = {character_data['personality']['teamwork']}f;
    
    private string[] wittyRemarks = new string[] {{
        {', '.join(f'"{quote}"' for quote in character_data.get('quotes', ['Mission accomplished!']))}
    }};
    
    void Start()
    {{
        InitializeCharacter();
    }}
    
    public override void InitializeCharacter()
    {{
        base.InitializeCharacter();
        Debug.Log($"{character_data['callsign']} reporting for duty. Role: {character_data['role']}");
    }}
    
    public string GetCombatQuote()
    {{
        if (wittyRemarks.Length > 0 && Random.Range(0f, 1f) < humorFactor)
        {{
            return wittyRemarks[Random.Range(0, wittyRemarks.Length)];
        }}
        return "Engaging target!";
    }}
    
    public override void UseSpecialAbility()
    {{
        // Special ability implementation for {character_data['role']}
        Debug.Log("{character_data['callsign']} using special ability!");
    }}
}}
"""
        return script
    
    async def _save_character_system(self, system_design: Dict):
        """Save character system design"""
        output_dir = Path("output/game_design")
        output_dir.mkdir(parents=True, exist_ok=True)
        
        with open(output_dir / "character_system.json", 'w') as f:
            json.dump(system_design, f, indent=2)
    
    async def _save_character_assets(self, character_data: Dict, unity_script: str, filename: str):
        """Save character assets"""
        # Save character data
        chars_dir = Path("output/game_assets/characters")
        chars_dir.mkdir(parents=True, exist_ok=True)
        
        with open(chars_dir / f"{filename}.json", 'w') as f:
            json.dump(character_data, f, indent=2)
        
        # Save Unity script
        scripts_dir = Path("output/unity_scripts/characters")
        scripts_dir.mkdir(parents=True, exist_ok=True)
        
        with open(scripts_dir / f"{filename}.cs", 'w') as f:
            f.write(unity_script)
    
    async def execute_primary_task(self):
        """Execute primary character creation task"""
        return await self.create_player_character()
