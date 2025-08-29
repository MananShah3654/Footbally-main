import asyncio
import os
import sys
import random
sys.path.append('/app/backend')

from motor.motor_asyncio import AsyncIOMotorClient
from dotenv import load_dotenv
from pathlib import Path

ROOT_DIR = Path(__file__).parent
load_dotenv(ROOT_DIR / '.env')

async def update_players_subscription():
    """Add subscription status to existing players"""
    
    # Connect to MongoDB
    mongo_url = os.environ['MONGO_URL']
    client = AsyncIOMotorClient(mongo_url)
    db = client[os.environ['DB_NAME']]
    
    try:
        # Get all players
        players = await db.players.find().to_list(1000)
        print(f"Found {len(players)} players to update")
        
        # Update each player with random subscription status (70% subscribed)
        updated_count = 0
        for player in players:
            is_subscribed = random.choice([True, True, True, False])  # 75% chance of being subscribed
            
            await db.players.update_one(
                {"id": player["id"]},
                {"$set": {"isSubscribed": is_subscribed}}
            )
            
            status = "✅" if is_subscribed else "❌"
            print(f"Updated {player['name']}: {status}")
            updated_count += 1
        
        print(f"\nSuccessfully updated {updated_count} players with subscription status")
        
    except Exception as e:
        print(f"Error updating players: {str(e)}")
    
    finally:
        client.close()

if __name__ == "__main__":
    asyncio.run(update_players_subscription())