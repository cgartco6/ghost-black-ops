#!/usr/bin/env python3
"""
Setup script for Ghost: Black Ops AI System
"""

import os
import sys
import subprocess
import platform
from pathlib import Path

def run_command(command: str, check: bool = True) -> bool:
    """Run a shell command"""
    try:
        print(f"Running: {command}")
        result = subprocess.run(command, shell=True, check=check, capture_output=True, text=True)
        if result.returncode == 0:
            print(f"✅ Command successful: {command}")
            return True
        else:
            print(f"❌ Command failed: {command}")
            print(f"Error: {result.stderr}")
            return False
    except subprocess.CalledProcessError as e:
        print(f"❌ Command failed with error: {e}")
        return False

def install_requirements():
    """Install Python requirements"""
    print("Installing Python requirements...")
    
    if run_command("pip install -r requirements.txt"):
        print("✅ Requirements installed successfully")
        return True
    else:
        print("❌ Failed to install requirements")
        return False

def setup_directories():
    """Create necessary directories"""
    directories = [
        "output/game_assets/characters",
        "output/game_assets/weapons", 
        "output/game_assets/gear",
        "output/game_assets/missions",
        "output/game_assets/scenes",
        "output/game_assets/environment",
        "output/unity_scripts/characters",
        "output/unity_scripts/weapons",
        "output/unity_scripts/gear", 
        "output/unity_scripts/missions",
        "output/unity_scripts/ai",
        "output/unity_scripts/core",
        "output/unity_scripts/systems",
        "output/unity_scripts/optimization",
        "output/game_design",
        "logs",
        "config",
        "unity_project/Assets/Scripts/Characters",
        "unity_project/Assets/Scripts/Weapons",
        "unity_project/Assets/Scripts/Missions",
        "unity_project/Assets/Scripts/AI",
        "unity_project/Assets/Scripts/UI",
        "unity_project/Assets/Scripts/Managers"
    ]
    
    for directory in directories:
        Path(directory).mkdir(parents=True, exist_ok=True)
        print(f"✅ Created directory: {directory}")

def check_system():
    """Check system compatibility"""
    system = platform.system().lower()
    python_version = platform.python_version()
    
    print(f"Detected system: {system}")
    print(f"Python version: {python_version}")
    
    if system not in ['windows', 'linux']:
        print("❌ Unsupported operating system")
        return False
    
    # Check Python version
    version_tuple = tuple(map(int, python_version.split('.')))
    if version_tuple < (3, 8):
        print("❌ Python 3.8 or higher required")
        return False
    
    print("✅ System check passed")
    return True

def main():
    """Main setup function"""
    print("=== Ghost: Black Ops AI System Setup ===")
    
    if not check_system():
        sys.exit(1)
    
    print("\n1. Creating directory structure...")
    setup_directories()
    
    print("\n2. Installing requirements...")
    if not install_requirements():
        print("⚠️  Some requirements may not have installed correctly")
    
    print("\n3. Generating configuration files...")
    # Configuration files will be auto-generated on first run
    
    print("\n🎉 Setup completed successfully!")
    print("\nNext steps:")
    print("1. Run: python main.py")
    print("2. The system will auto-generate configuration files")
    print("3. AI agents will start creating your game!")
    
    # Create a quick start script
    quick_start = """#!/bin/bash
echo "Starting Ghost: Black Ops AI System..."
python main.py
"""
    
    with open("start.sh", "w") as f:
        f.write(quick_start)
    
    with open("start.bat", "w") as f:
        f.write("@echo off\npython main.py\npause")
    
    os.chmod("start.sh", 0o755)
    
    print("\nQuick start scripts created:")
    print("- start.sh (Linux/Ubuntu)")
    print("- start.bat (Windows)")

if __name__ == "__main__":
    main()
