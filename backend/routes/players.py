from fastapi import APIRouter, HTTPException, Depends
from typing import List
from motor.motor_asyncio import AsyncIOMotorDatabase
import sys
import os
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from models.player import Player, PlayerCreate, PlayerUpdate
import json
from datetime import datetime

def get_database():
    from server import db
    return db

router = APIRouter(prefix="/api/players", tags=["players"])

@router.get("/", response_model=List[Player])
async def get_all_players(db: AsyncIOMotorDatabase = Depends(get_database)):
    """Get all players"""
    players = await db.players.find().to_list(1000)
    return [Player(**player) for player in players]

@router.get("/{player_id}", response_model=Player)
async def get_player(player_id: str, db: AsyncIOMotorDatabase = Depends(get_database)):
    """Get a single player by ID"""
    player = await db.players.find_one({"id": player_id})
    if not player:
        raise HTTPException(status_code=404, detail="Player not found")
    return Player(**player)

@router.post("/", response_model=Player)
async def create_player(player_data: PlayerCreate, db: AsyncIOMotorDatabase = Depends(get_database)):
    """Create a new player"""
    # Check if player name already exists
    existing = await db.players.find_one({"name": player_data.name})
    if existing:
        raise HTTPException(status_code=400, detail="Player name already exists")
    
    # Create player object
    player = Player(**player_data.dict())
    
    # Insert into database
    await db.players.insert_one(player.dict())
    
    return player

@router.put("/{player_id}", response_model=Player)
async def update_player(
    player_id: str, 
    player_data: PlayerUpdate, 
    db: AsyncIOMotorDatabase = Depends(get_database)
):
    """Update a player"""
    # Check if player exists
    existing = await db.players.find_one({"id": player_id})
    if not existing:
        raise HTTPException(status_code=404, detail="Player not found")
    
    # Prepare update data (only include non-None fields)
    update_data = {k: v for k, v in player_data.dict().items() if v is not None}
    update_data["updated_at"] = datetime.utcnow()
    
    # Check name uniqueness if name is being updated
    if "name" in update_data:
        name_exists = await db.players.find_one({
            "name": update_data["name"],
            "id": {"$ne": player_id}
        })
        if name_exists:
            raise HTTPException(status_code=400, detail="Player name already exists")
    
    # Update player
    await db.players.update_one(
        {"id": player_id},
        {"$set": update_data}
    )
    
    # Return updated player
    updated_player = await db.players.find_one({"id": player_id})
    return Player(**updated_player)

@router.delete("/{player_id}")
async def delete_player(player_id: str, db: AsyncIOMotorDatabase = Depends(get_database)):
    """Delete a player"""
    result = await db.players.delete_one({"id": player_id})
    if result.deleted_count == 0:
        raise HTTPException(status_code=404, detail="Player not found")
    
    return {"message": "Player deleted successfully"}

@router.post("/import")
async def import_players(players_data: List[PlayerCreate], db: AsyncIOMotorDatabase = Depends(get_database)):
    """Import multiple players from JSON data"""
    created_players = []
    errors = []
    
    for i, player_data in enumerate(players_data):
        try:
            # Check if player name already exists
            existing = await db.players.find_one({"name": player_data.name})
            if existing:
                errors.append(f"Row {i+1}: Player '{player_data.name}' already exists")
                continue
            
            # Create and insert player
            player = Player(**player_data.dict())
            await db.players.insert_one(player.dict())
            created_players.append(player)
            
        except Exception as e:
            errors.append(f"Row {i+1}: {str(e)}")
    
    return {
        "created": len(created_players),
        "errors": errors,
        "players": [p.dict() for p in created_players]
    }