from fastapi import APIRouter, HTTPException, Depends
from typing import List
from motor.motor_asyncio import AsyncIOMotorDatabase
import sys
import os
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from models.player import Player
from services.shuffle_service import shuffle_teams

def get_database():
    from server import db
    return db

router = APIRouter(prefix="/api", tags=["shuffle"])

@router.post("/shuffle")
async def shuffle_teams_endpoint(db: AsyncIOMotorDatabase = Depends(get_database)):
    """Shuffle all players into two balanced teams"""
    
    # Get all players from database
    players_data = await db.players.find().to_list(1000)
    
    if len(players_data) != 16:
        raise HTTPException(
            status_code=400, 
            detail=f"Exactly 16 players are required for shuffling. Found {len(players_data)} players."
        )
    
    # Convert to Player objects
    players = [Player(**player_data) for player_data in players_data]
    
    try:
        # Shuffle teams
        result = shuffle_teams(players)
        return result
        
    except ValueError as e:
        raise HTTPException(status_code=400, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error shuffling teams: {str(e)}")

@router.post("/shuffle/custom")
async def shuffle_custom_players(player_ids: List[str], db: AsyncIOMotorDatabase = Depends(get_database)):
    """Shuffle specific players into teams by their IDs"""
    
    if len(player_ids) != 16:
        raise HTTPException(
            status_code=400,
            detail=f"Exactly 16 player IDs are required. Received {len(player_ids)}."
        )
    
    # Get players by IDs
    players_data = await db.players.find({"id": {"$in": player_ids}}).to_list(1000)
    
    if len(players_data) != 16:
        missing_ids = set(player_ids) - {p["id"] for p in players_data}
        raise HTTPException(
            status_code=404,
            detail=f"Some players not found. Missing IDs: {list(missing_ids)}"
        )
    
    # Convert to Player objects
    players = [Player(**player_data) for player_data in players_data]
    
    try:
        # Shuffle teams
        result = shuffle_teams(players)
        return result
        
    except ValueError as e:
        raise HTTPException(status_code=400, detail=str(e))
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error shuffling teams: {str(e)}")