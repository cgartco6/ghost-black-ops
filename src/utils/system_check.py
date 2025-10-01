"""
System validation for Ghost: Black Ops AI requirements
"""

import platform
import psutil
import sys
from pathlib import Path

class SystemValidator:
    def __init__(self):
        self.system_info = self._gather_system_info()
    
    def _gather_system_info(self) -> dict:
        """Gather comprehensive system information"""
        return {
            'platform': platform.system(),
            'platform_version': platform.version(),
            'architecture': platform.architecture()[0],
            'processor': platform.processor(),
            'memory_gb': round(psutil.virtual_memory().total / (1024**3), 1),
            'available_memory_gb': round(psutil.virtual_memory().available / (1024**3), 1),
            'disk_space_gb': round(psutil.disk_usage('/').free / (1024**3), 1),
            'python_version': platform.python_version()
        }
    
    def validate_system(self) -> bool:
        """Validate if system meets requirements"""
        requirements_met = True
        
        print("=== System Validation ===")
        print(f"Platform: {self.system_info['platform']} {self.system_info['platform_version']}")
        print(f"Architecture: {self.system_info['architecture']}")
        print(f"Processor: {self.system_info['processor']}")
        print(f"Memory: {self.system_info['memory_gb']} GB")
        print(f"Available Memory: {self.system_info['available_memory_gb']} GB")
        print(f"Disk Space: {self.system_info['disk_space_gb']} GB free")
        print(f"Python: {self.system_info['python_version']}")
        print()
        
        # Check memory requirements
        if self.system_info['available_memory_gb'] < 4:
            print("❌ WARNING: Less than 4GB RAM available. Performance may be affected.")
            requirements_met = False
        else:
            print("✅ Memory: OK")
        
        # Check disk space
        if self.system_info['disk_space_gb'] < 10:
            print("❌ ERROR: Less than 10GB disk space available.")
            requirements_met = False
        else:
            print("✅ Disk Space: OK")
        
        # Check Python version
        python_version = tuple(map(int, self.system_info['python_version'].split('.')))
        if python_version < (3, 8):
            print("❌ ERROR: Python 3.8 or higher required.")
            requirements_met = False
        else:
            print("✅ Python Version: OK")
        
        # Platform-specific checks
        if self.system_info['platform'] == 'Windows':
            if self.system_info['memory_gb'] < 8:
                print("⚠️  RECOMMENDATION: 8GB+ RAM recommended for Windows")
            print("✅ Windows Platform: Supported")
        
        elif self.system_info['platform'] == 'Linux':
            if self.system_info['memory_gb'] < 4:
                print("⚠️  RECOMMENDATION: 4GB+ RAM recommended for Linux")
            print("✅ Linux Platform: Supported")
        
        else:
            print("⚠️  WARNING: Unsupported platform detected")
            requirements_met = False
        
        print("\n" + "="*50)
        
        if requirements_met:
            print("✅ System validation PASSED")
        else:
            print("❌ System validation FAILED")
        
        return requirements_met
    
    def get_recommendations(self) -> list:
        """Get system optimization recommendations"""
        recommendations = []
        
        if self.system_info['available_memory_gb'] < 8:
            recommendations.append("Close unnecessary applications to free up memory")
        
        if self.system_info['disk_space_gb'] < 20:
            recommendations.append("Free up disk space for better performance")
        
        if self.system_info['platform'] == 'Windows':
            recommendations.append("Run as Administrator for full system access")
        
        return recommendations
