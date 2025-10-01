"""
Advanced logging system for Ghost: Black Ops AI
"""

import logging
import sys
from datetime import datetime
from pathlib import Path
from typing import Optional

class GameLogger:
    def __init__(self, log_level: str = "INFO", log_file: Optional[str] = None):
        self.log_level = log_level
        self.log_file = log_file or f"logs/ghost_black_ops_{datetime.now().strftime('%Y%m%d_%H%M%S')}.log"
        
        # Create logs directory
        Path("logs").mkdir(exist_ok=True)
        
        self._setup_logger()
    
    def _setup_logger(self):
        """Setup logging configuration"""
        self.logger = logging.getLogger("GhostBlackOps")
        self.logger.setLevel(self._get_log_level())
        
        # Clear any existing handlers
        self.logger.handlers.clear()
        
        # Formatter
        formatter = logging.Formatter(
            '%(asctime)s | %(levelname)-8s | %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S'
        )
        
        # Console handler
        console_handler = logging.StreamHandler(sys.stdout)
        console_handler.setLevel(self._get_log_level())
        console_handler.setFormatter(formatter)
        self.logger.addHandler(console_handler)
        
        # File handler
        file_handler = logging.FileHandler(self.log_file)
        file_handler.setLevel(logging.DEBUG)
        file_handler.setFormatter(formatter)
        self.logger.addHandler(file_handler)
    
    def _get_log_level(self):
        """Convert string log level to logging constant"""
        levels = {
            'DEBUG': logging.DEBUG,
            'INFO': logging.INFO,
            'WARNING': logging.WARNING,
            'ERROR': logging.ERROR,
            'CRITICAL': logging.CRITICAL
        }
        return levels.get(self.log_level.upper(), logging.INFO)
    
    def debug(self, message: str):
        """Log debug message"""
        self.logger.debug(message)
    
    def info(self, message: str):
        """Log info message"""
        self.logger.info(message)
    
    def warning(self, message: str):
        """Log warning message"""
        self.logger.warning(message)
    
    def error(self, message: str):
        """Log error message"""
        self.logger.error(message)
    
    def critical(self, message: str):
        """Log critical message"""
        self.logger.critical(message)
    
    def success(self, message: str):
        """Log success message (custom level)"""
        self.logger.info(f"✅ SUCCESS: {message}")
    
    def system(self, message: str):
        """Log system message (custom level)"""
        self.logger.info(f"🖥️  SYSTEM: {message}")
    
    def agent(self, message: str):
        """Log agent message (custom level)"""
        self.logger.info(f"🤖 AGENT: {message}")
