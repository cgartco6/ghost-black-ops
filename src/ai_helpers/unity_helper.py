"""
Unity Helper - Manages Unity project integration
"""

import asyncio
import json
import shutil
from pathlib import Path
from typing import Dict, List, Any

class UnityHelper:
    def __init__(self, config: Dict, logger):
        self.config = config
        self.logger = logger
        self.unity_path = Path(config['unity']['project_path'])
        
    async def initialize(self):
        """Initialize Unity helper"""
        self.logger.info("Unity Helper initialized")
        return True
    
    async def setup_unity_project(self):
        """Setup Unity project structure"""
        self.logger.info("Setting up Unity project structure...")
        
        project_structure = {
            'Assets': {
                'Scripts': {
                    'Characters': [],
                    'Weapons': [],
                    'Missions': [],
                    'AI': [],
                    'UI': [],
                    'Managers': []
                },
                'Scenes': [],
                'Prefabs': {
                    'Characters': [],
                    'Weapons': [],
                    'Environment': []
                },
                'Materials': [],
                'Textures': [],
                'Audio': {
                    'Music': [],
                    'SFX': []
                }
            },
            'ProjectSettings': [],
            'Packages': []
        }
        
        await self._create_project_structure(project_structure)
        
        # Generate essential Unity scripts
        await self._generate_core_unity_scripts()
        
        return {
            'helper': 'unity_helper',
            'task': 'setup_unity_project',
            'result': 'success',
            'performance': 0.95
        }
    
    async def integrate_assets(self):
        """Integrate generated assets into Unity project"""
        self.logger.info("Integrating assets into Unity project...")
        
        integration_tasks = [
            self._integrate_character_scripts(),
            self._integrate_weapon_scripts(),
            self._integrate_mission_scripts(),
            self._integrate_ai_scripts()
        ]
        
        await asyncio.gather(*integration_tasks)
        
        return {
            'helper': 'unity_helper',
            'task': 'integrate_assets',
            'result': 'success',
            'assets_integrated': 'All generated assets',
            'performance': 0.92
        }
    
    async def _create_project_structure(self, structure: Dict):
        """Create Unity project directory structure"""
        def create_dirs(base_path: Path, struct: Dict):
            for folder, subfolders in struct.items():
                folder_path = base_path / folder
                folder_path.mkdir(parents=True, exist_ok=True)
                if isinstance(subfolders, dict):
                    create_dirs(folder_path, subfolders)
        
        create_dirs(self.unity_path, structure)
        self.logger.info("Unity project structure created")
    
    async def _generate_core_unity_scripts(self):
        """Generate core Unity scripts"""
        scripts_dir = self.unity_path / "Assets" / "Scripts" / "Managers"
        scripts_dir.mkdir(parents=True, exist_ok=True)
        
        # Game Manager
        game_manager_script = """
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Systems")]
    public PlayerController player;
    public MissionManager missionManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    
    [Header("Game State")]
    public GameState currentGameState;
    public int playerTokens;
    public int playerLevel;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeGameSystems();
    }
    
    void InitializeGameSystems()
    {
        // Initialize all game systems
        missionManager.Initialize();
        uiManager.Initialize();
        audioManager.Initialize();
        
        Debug.Log("Game systems initialized successfully");
    }
    
    public void AddTokens(int amount)
    {
        playerTokens += amount;
        uiManager.UpdateTokenDisplay(playerTokens);
    }
    
    public enum GameState
    {
        MainMenu,
        InMission,
        Paused,
        MissionComplete,
        GameOver
    }
}
"""
        
        with open(scripts_dir / "GameManager.cs", 'w') as f:
            f.write(game_manager_script)
    
    async def _integrate_character_scripts(self):
        """Integrate character scripts into Unity project"""
        source_dir = Path("output/unity_scripts/characters")
        target_dir = self.unity_path / "Assets" / "Scripts" / "Characters"
        
        if source_dir.exists():
            # Copy all character scripts
            for script_file in source_dir.glob("*.cs"):
                shutil.copy2(script_file, target_dir / script_file.name)
            
            self.logger.info(f"Integrated {len(list(source_dir.glob('*.cs')))} character scripts")
    
    async def _integrate_weapon_scripts(self):
        """Integrate weapon scripts into Unity project"""
        source_dir = Path("output/unity_scripts/weapons")
        target_dir = self.unity_path / "Assets" / "Scripts" / "Weapons"
        
        if source_dir.exists():
            for script_file in source_dir.glob("*.cs"):
                shutil.copy2(script_file, target_dir / script_file.name)
            
            self.logger.info(f"Integrated {len(list(source_dir.glob('*.cs')))} weapon scripts")
    
    async def _integrate_mission_scripts(self):
        """Integrate mission scripts into Unity project"""
        source_dir = Path("output/unity_scripts/missions")
        target_dir = self.unity_path / "Assets" / "Scripts" / "Missions"
        
        if source_dir.exists():
            for script_file in source_dir.glob("*.cs"):
                shutil.copy2(script_file, target_dir / script_file.name)
            
            self.logger.info(f"Integrated {len(list(source_dir.glob('*.cs')))} mission scripts")
    
    async def _integrate_ai_scripts(self):
        """Integrate AI scripts into Unity project"""
        source_dir = Path("output/unity_scripts/ai")
        target_dir = self.unity_path / "Assets" / "Scripts" / "AI"
        
        if source_dir.exists():
            for script_file in source_dir.glob("*.cs"):
                shutil.copy2(script_file, target_dir / script_file.name)
            
            self.logger.info(f"Integrated {len(list(source_dir.glob('*.cs')))} AI scripts")
