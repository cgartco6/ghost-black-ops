#!/usr/bin/env python3
"""
Ghost: Black Ops - Main AI Director System
Coordinates AI agents and helpers to develop the complete game
"""

import asyncio
import sys
import os
import platform
from pathlib import Path
from typing import Dict, List, Any
import yaml
import argparse

# Add src to path
sys.path.append(os.path.join(os.path.dirname(__file__), 'src'))

from ai_director import AIDirector
from utils.logger import GameLogger
from utils.system_check import SystemValidator

class GhostBlackOps:
    def __init__(self):
        self.logger = GameLogger()
        self.validator = SystemValidator()
        self.director = None
        self.system_config = None
        
    def load_configuration(self, system_type: str = None):
        """Load system-specific configuration"""
        if not system_type:
            system_type = platform.system().lower()
            
        config_file = f"config/{system_type}_config.yaml"
        
        if not os.path.exists(config_file):
            self.logger.error(f"Configuration file {config_file} not found!")
            # Create default config
            self.create_default_config(system_type)
            
        try:
            with open(config_file, 'r') as f:
                self.system_config = yaml.safe_load(f)
            self.logger.info(f"Loaded configuration for {system_type}")
        except Exception as e:
            self.logger.error(f"Failed to load config: {e}")
            sys.exit(1)
    
    def create_default_config(self, system_type: str):
        """Create default configuration file"""
        config_dir = Path("config")
        config_dir.mkdir(exist_ok=True)
        
        default_config = {
            'system': {
                'type': system_type,
                'max_agents': 8 if system_type == 'windows' else 4,
                'max_memory_gb': 12 if system_type == 'windows' else 6,
                'enable_gpu': True if system_type == 'windows' else False
            },
            'game': {
                'name': 'Ghost: Black Ops',
                'version': '1.0.0',
                'target_platforms': ['windows', 'ubuntu']
            },
            'ai': {
                'model_provider': 'openai',
                'max_concurrent_tasks': 3,
                'enable_learning': True,
                'training_interval': 3600
            },
            'unity': {
                'project_path': './unity_project',
                'assets_path': './unity_project/Assets',
                'scripts_path': './unity_project/Assets/Scripts'
            }
        }
        
        config_file = config_dir / f"{system_type}_config.yaml"
        with open(config_file, 'w') as f:
            yaml.dump(default_config, f, default_flow_style=False)
        
        self.logger.info(f"Created default configuration for {system_type}")
        self.system_config = default_config
    
    async def initialize_system(self):
        """Initialize the complete AI system"""
        self.logger.info("=== Ghost: Black Ops AI System Initialization ===")
        
        # Validate system requirements
        if not self.validator.validate_system():
            self.logger.error("System validation failed!")
            return False
            
        # Load configuration
        self.load_configuration()
        
        # Initialize AI Director
        self.director = AIDirector(self.system_config, self.logger)
        
        # Start AI agents
        await self.director.initialize_agents()
        
        self.logger.success("AI System initialized successfully!")
        return True
    
    async def run_development_cycle(self):
        """Run the complete game development cycle"""
        self.logger.info("Starting game development cycle...")
        
        try:
            # Phase 1: Game Design and Planning
            await self.director.execute_phase("design")
            
            # Phase 2: Character and Asset Creation
            await self.director.execute_phase("creation")
            
            # Phase 3: Level and Mission Design
            await self.director.execute_phase("level_design")
            
            # Phase 4: Code Generation
            await self.director.execute_phase("coding")
            
            # Phase 5: Integration and Testing
            await self.director.execute_phase("integration")
            
            self.logger.success("Game development cycle completed!")
            
        except Exception as e:
            self.logger.error(f"Development cycle failed: {e}")
            return False
            
        return True
    
    def print_banner(self):
        """Print system banner"""
        banner = """
        ╔══════════════════════════════════════════════════════════════╗
        ║                   GHOST: BLACK OPS                           ║
        ║               AI Development System                          ║
        ║                                                              ║
        ║  Creating next-generation tactical gaming experience        ║
        ║  with advanced synthetic intelligence                        ║
        ║                                                              ╕
        ║  System: {}@{}                                    ║
        ║  AI Agents: {}                                              ║
        ║  Memory: {} GB available                                  ║
        ╚══════════════════════════════════════════════════════════════╝
        """.format(
            platform.system(),
            platform.node(),
            self.system_config['system']['max_agents'],
            self.system_config['system']['max_memory_gb']
        )
        print(banner)

async def main():
    """Main application entry point"""
    parser = argparse.ArgumentParser(description='Ghost: Black Ops AI System')
    parser.add_argument('--phase', type=str, help='Specific phase to run')
    parser.add_argument('--agent', type=str, help='Run specific agent')
    args = parser.parse_args()
    
    # Initialize the system
    game_system = GhostBlackOps()
    game_system.print_banner()
    
    if not await game_system.initialize_system():
        sys.exit(1)
    
    # Run specific phase or complete cycle
    if args.phase:
        await game_system.director.execute_phase(args.phase)
    elif args.agent:
        await game_system.director.run_single_agent(args.agent)
    else:
        await game_system.run_development_cycle()

if __name__ == "__main__":
    asyncio.run(main())
