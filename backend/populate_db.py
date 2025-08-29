import asyncio
import os
import sys
sys.path.append('/app/backend')

from motor.motor_asyncio import AsyncIOMotorClient
from dotenv import load_dotenv
from pathlib import Path
from models.player import Player, PlayerSkills

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

# Mock data
mock_players = [
    {
        "name": "Manan Shah",
        "position": "DEF",
        "points": 85,
        "photo": "https://drive.google.com/uc?id=19shtJ6sbjhYq9t7Nj38A_Y5dRNWbZIvM&w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 78,
            "shooting": 45,
            "passing": 82,
            "defending": 89,
            "dribbling": 65,
            "physical": 84
        },
        "age": 28,
        "preferredFoot": "Right",
        "nationality": "Brazil"
    },
    {
        "name": "Alex Johnson",
        "position": "ATT",
        "points": 88,
        "photo": "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 91,
            "shooting": 87,
            "passing": 79,
            "defending": 35,
            "dribbling": 88,
            "physical": 75
        },
        "age": 25,
        "preferredFoot": "Left",
        "nationality": "England"
    },
    {
        "name": "David Rodriguez",
        "position": "MID",
        "points": 86,
        "photo": "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 82,
            "shooting": 76,
            "passing": 91,
            "defending": 74,
            "dribbling": 83,
            "physical": 79
        },
        "age": 26,
        "preferredFoot": "Right",
        "nationality": "Spain"
    },
    {
        "name": "James Wilson",
        "position": "DEF",
        "points": 83,
        "photo": "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 75,
            "shooting": 42,
            "passing": 78,
            "defending": 87,
            "dribbling": 62,
            "physical": 88
        },
        "age": 29,
        "preferredFoot": "Left",
        "nationality": "Scotland"
    },
    {
        "name": "Carlos Mendez",
        "position": "ATT",
        "points": 90,
        "photo": "https://images.unsplash.com/photo-1566492031773-4f4e44671d66?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 93,
            "shooting": 89,
            "passing": 81,
            "defending": 28,
            "dribbling": 92,
            "physical": 72
        },
        "age": 24,
        "preferredFoot": "Right",
        "nationality": "Argentina"
    },
    {
        "name": "Robert Brown",
        "position": "MID",
        "points": 84,
        "photo": "https://images.unsplash.com/photo-1570295999919-56ceb5ecca61?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 79,
            "shooting": 72,
            "passing": 87,
            "defending": 78,
            "dribbling": 80,
            "physical": 82
        },
        "age": 27,
        "preferredFoot": "Right",
        "nationality": "USA"
    },
    {
        "name": "Lucas Thompson",
        "position": "DEF",
        "points": 81,
        "photo": "https://images.unsplash.com/photo-1547425260-76bcadfb4f2c?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 73,
            "shooting": 38,
            "passing": 76,
            "defending": 85,
            "dribbling": 58,
            "physical": 86
        },
        "age": 30,
        "preferredFoot": "Right",
        "nationality": "Canada"
    },
    {
        "name": "Francesco Rossi",
        "position": "ATT",
        "points": 87,
        "photo": "https://images.unsplash.com/photo-1492562080023-ab3db95bfbce?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 88,
            "shooting": 85,
            "passing": 77,
            "defending": 32,
            "dribbling": 89,
            "physical": 74
        },
        "age": 26,
        "preferredFoot": "Left",
        "nationality": "Italy"
    },
    {
        "name": "Kevin O'Connor",
        "position": "MID",
        "points": 82,
        "photo": "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 77,
            "shooting": 68,
            "passing": 84,
            "defending": 76,
            "dribbling": 78,
            "physical": 80
        },
        "age": 28,
        "preferredFoot": "Right",
        "nationality": "Ireland"
    },
    {
        "name": "Ahmed Hassan",
        "position": "DEF",
        "points": 84,
        "photo": "https://images.unsplash.com/photo-1507591064344-4c6ce005b128?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 76,
            "shooting": 45,
            "passing": 81,
            "defending": 88,
            "dribbling": 64,
            "physical": 85
        },
        "age": 27,
        "preferredFoot": "Right",
        "nationality": "Egypt"
    },
    {
        "name": "Pierre Dubois",
        "position": "ATT",
        "points": 86,
        "photo": "https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 89,
            "shooting": 83,
            "passing": 75,
            "defending": 30,
            "dribbling": 87,
            "physical": 73
        },
        "age": 25,
        "preferredFoot": "Left",
        "nationality": "France"
    },
    {
        "name": "Viktor Petrov",
        "position": "MID",
        "points": 85,
        "photo": "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 81,
            "shooting": 74,
            "passing": 89,
            "defending": 77,
            "dribbling": 82,
            "physical": 81
        },
        "age": 29,
        "preferredFoot": "Right",
        "nationality": "Russia"
    },
    {
        "name": "Daniel Park",
        "position": "DEF",
        "points": 80,
        "photo": "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 72,
            "shooting": 40,
            "passing": 74,
            "defending": 84,
            "dribbling": 60,
            "physical": 83
        },
        "age": 31,
        "preferredFoot": "Left",
        "nationality": "South Korea"
    },
    {
        "name": "Miguel Santos",
        "position": "ATT",
        "points": 89,
        "photo": "https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 92,
            "shooting": 88,
            "passing": 80,
            "defending": 29,
            "dribbling": 91,
            "physical": 76
        },
        "age": 23,
        "preferredFoot": "Right",
        "nationality": "Portugal"
    },
    {
        "name": "Thomas Mueller",
        "position": "MID",
        "points": 87,
        "photo": "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 83,
            "shooting": 78,
            "passing": 92,
            "defending": 75,
            "dribbling": 85,
            "physical": 82
        },
        "age": 26,
        "preferredFoot": "Right",
        "nationality": "Germany"
    },
    {
        "name": "Hiroshi Tanaka",
        "position": "DEF",
        "points": 82,
        "photo": "https://images.unsplash.com/photo-1552058544-f2b08422138a?w=400&h=600&fit=crop&crop=face",
        "skills": {
            "pace": 74,
            "shooting": 43,
            "passing": 79,
            "defending": 86,
            "dribbling": 63,
            "physical": 84
        },
        "age": 28,
        "preferredFoot": "Right",
        "nationality": "Japan"
    }
]

async def populate_database():
    """Populate database with mock players"""
    
    # Simulate clearing and inserting players in an in-memory list
    print("Cleared existing players (in-memory)")
    inserted_players = []
    in_memory_db = []
    for player_data in mock_players:
        # Create Player object
        player = Player(**player_data)
        # Insert into in-memory db
        in_memory_db.append(player.dict())
        inserted_players.append(player.name)
        print(f"Inserted player: {player.name}")
    print(f"\nSuccessfully inserted {len(inserted_players)} players:")
    for name in inserted_players:
        print(f"  - {name}")
    print(f"\nTotal players in in-memory db: {len(in_memory_db)}")

if __name__ == "__main__":
    import asyncio
    asyncio.run(populate_database())