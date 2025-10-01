"""
AI Director - Main coordinator for all AI agents and helpers
"""

import asyncio
import json
import time
from typing import Dict, List, Any, Optional
from dataclasses import dataclass
from enum import Enum

from game_agents.character_creator import CharacterCreator
from game_agents.level_designer import LevelDesigner
from game_agents.mission_planner import MissionPlanner
from game_agents.asset_generator import AssetGenerator
from ai_helpers.unity_helper import UnityHelper
from ai_helpers.code_generator import CodeGenerator
from ai_helpers.task_manager import TaskManager
from ai_helpers.performance_optimizer import PerformanceOptimizer

class DevelopmentPhase(Enum):
    DESIGN = "design"
    CREATION = "creation"
    LEVEL_DESIGN = "level_design"
    CODING = "coding"
    INTEGRATION = "integration"

@dataclass
class AgentStatus:
    name: str
    is_active: bool
    tasks_completed: int
    current_task: Optional[str]
    performance: float

class AIDirector:
    def __init__(self, config: Dict, logger):
        self.config = config
        self.logger = logger
        self.agents = {}
        self.helpers = {}
        self.task_manager = None
        self.phase_progress = {}
        self.agent_status = {}
        
    async def initialize_agents(self):
        """Initialize all AI agents and helpers"""
        self.logger.info("Initializing AI Agents and Helpers...")
        
        # Initialize task manager first
        self.task_manager = TaskManager(self.config, self.logger)
        
        # Initialize AI Agents
        self.agents['character_creator'] = CharacterCreator(self.config, self.logger)
        self.agents['level_designer'] = LevelDesigner(self.config, self.logger)
        self.agents['mission_planner'] = MissionPlanner(self.config, self.logger)
        self.agents['asset_generator'] = AssetGenerator(self.config, self.logger)
        
        # Initialize AI Helpers
        self.helpers['unity_helper'] = UnityHelper(self.config, self.logger)
        self.helpers['code_generator'] = CodeGenerator(self.config, self.logger)
        self.helpers['performance_optimizer'] = PerformanceOptimizer(self.config, self.logger)
        
        # Initialize all components
        init_tasks = []
        for agent_name, agent in self.agents.items():
            init_tasks.append(agent.initialize())
            self.agent_status[agent_name] = AgentStatus(
                name=agent_name,
                is_active=False,
                tasks_completed=0,
                current_task=None,
                performance=0.0
            )
        
        for helper_name, helper in self.helpers.items():
            init_tasks.append(helper.initialize())
            
        await asyncio.gather(*init_tasks, return_exceptions=True)
        
        self.logger.success(f"Initialized {len(self.agents)} agents and {len(self.helpers)} helpers")
    
    async def execute_phase(self, phase: str):
        """Execute a specific development phase"""
        phase_enum = DevelopmentPhase(phase)
        self.logger.info(f"Executing development phase: {phase_enum.value}")
        
        phase_methods = {
            DevelopmentPhase.DESIGN: self._run_design_phase,
            DevelopmentPhase.CREATION: self._run_creation_phase,
            DevelopmentPhase.LEVEL_DESIGN: self._run_level_design_phase,
            DevelopmentPhase.CODING: self._run_coding_phase,
            DevelopmentPhase.INTEGRATION: self._run_integration_phase
        }
        
        if phase_enum in phase_methods:
            await phase_methods[phase_enum]()
        else:
            self.logger.error(f"Unknown phase: {phase}")
    
    async def _run_design_phase(self):
        """Run game design phase"""
        self.logger.info("=== GAME DESIGN PHASE ===")
        
        tasks = [
            self.agents['mission_planner'].design_game_narrative(),
            self.agents['character_creator'].design_character_system(),
            self.agents['level_designer'].design_world_structure()
        ]
        
        results = await asyncio.gather(*tasks, return_exceptions=True)
        await self._process_phase_results("design", results)
    
    async def _run_creation_phase(self):
        """Run asset creation phase"""
        self.logger.info("=== ASSET CREATION PHASE ===")
        
        tasks = [
            self.agents['character_creator'].create_player_character(),
            self.agents['character_creator'].create_ai_team_members(),
            self.agents['asset_generator'].generate_weapons(),
            self.agents['asset_generator'].generate_gear(),
            self.agents['asset_generator'].generate_environment_assets()
        ]
        
        results = await asyncio.gather(*tasks, return_exceptions=True)
        await self._process_phase_results("creation", results)
    
    async def _run_level_design_phase(self):
        """Run level design phase"""
        self.logger.info("=== LEVEL DESIGN PHASE ===")
        
        tasks = [
            self.agents['level_designer'].create_main_scenes(),
            self.agents['mission_planner'].create_mission_structure(),
            self.agents['level_designer'].optimize_level_performance()
        ]
        
        results = await asyncio.gather(*tasks, return_exceptions=True)
        await self._process_phase_results("level_design", results)
    
    async def _run_coding_phase(self):
        """Run code generation phase"""
        self.logger.info("=== CODE GENERATION PHASE ===")
        
        tasks = [
            self.helpers['code_generator'].generate_character_systems(),
            self.helpers['code_generator'].generate_ai_behavior(),
            self.helpers['code_generator'].generate_game_mechanics(),
            self.helpers['unity_helper'].setup_unity_project()
        ]
        
        results = await asyncio.gather(*tasks, return_exceptions=True)
        await self._process_phase_results("coding", results)
    
    async def _run_integration_phase(self):
        """Run integration and testing phase"""
        self.logger.info("=== INTEGRATION PHASE ===")
        
        tasks = [
            self.helpers['unity_helper'].integrate_assets(),
            self.helpers['performance_optimizer'].optimize_game_performance(),
            self.task_manager.run_test_suite()
        ]
        
        results = await asyncio.gather(*tasks, return_exceptions=True)
        await self._process_phase_results("integration", results)
    
    async def _process_phase_results(self, phase: str, results: List):
        """Process results from a development phase"""
        successful = 0
        for i, result in enumerate(results):
            if isinstance(result, Exception):
                self.logger.error(f"Task {i} failed: {result}")
            else:
                successful += 1
                self._update_agent_status(result)
        
        self.phase_progress[phase] = successful / len(results) * 100
        self.logger.info(f"Phase {phase} completed: {successful}/{len(results)} tasks successful")
    
    def _update_agent_status(self, task_result: Dict):
        """Update agent status based on task results"""
        if 'agent' in task_result and 'task' in task_result:
            agent_name = task_result['agent']
            if agent_name in self.agent_status:
                self.agent_status[agent_name].tasks_completed += 1
                self.agent_status[agent_name].current_task = None
                if 'performance' in task_result:
                    self.agent_status[agent_name].performance = task_result['performance']
    
    async def run_single_agent(self, agent_name: str):
        """Run a specific agent independently"""
        if agent_name in self.agents:
            self.logger.info(f"Running agent: {agent_name}")
            agent = self.agents[agent_name]
            await agent.execute_primary_task()
        else:
            self.logger.error(f"Unknown agent: {agent_name}")
    
    def get_system_status(self) -> Dict:
        """Get current system status"""
        return {
            'active_agents': len([a for a in self.agent_status.values() if a.is_active]),
            'total_tasks_completed': sum(a.tasks_completed for a in self.agent_status.values()),
            'phase_progress': self.phase_progress,
            'agent_status': {name: status.__dict__ for name, status in self.agent_status.items()}
        }
