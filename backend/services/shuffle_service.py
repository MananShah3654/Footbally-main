import random
import sys
import os
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from typing import List, Dict, Any
from models.player import Player

def shuffle_teams(players: List[Player]) -> Dict[str, Any]:
    """
    Shuffle players into two balanced teams with position constraints:
    - Each team must have exactly 2 defenders, 2 attackers, 1 midfielder
    - Remaining 3 players can be any position
    - Teams should be balanced by total points
    """
    if len(players) != 16:
        raise ValueError("Exactly 16 players are required for team shuffle")
    
    # Separate players by position
    defenders = [p for p in players if p.position == 'DEF']
    attackers = [p for p in players if p.position == 'ATT']
    midfielders = [p for p in players if p.position == 'MID']
    
    # Validate we have enough players in each position
    if len(defenders) < 4:
        raise ValueError("At least 4 defenders are required")
    if len(attackers) < 4:
        raise ValueError("At least 4 attackers are required") 
    if len(midfielders) < 2:
        raise ValueError("At least 2 midfielders are required")
    
    # Shuffle each position array
    random.shuffle(defenders)
    random.shuffle(attackers) 
    random.shuffle(midfielders)
    
    # Assign mandatory positions to teams
    team1 = [
        defenders[0], defenders[1],  # 2 defenders
        attackers[0], attackers[1],  # 2 attackers
        midfielders[0]  # 1 midfielder
    ]
    
    team2 = [
        defenders[2], defenders[3],  # 2 defenders
        attackers[2], attackers[3],  # 2 attackers
        midfielders[1]  # 1 midfielder
    ]
    
    # Collect remaining players
    remaining = []
    remaining.extend(defenders[4:])
    remaining.extend(attackers[4:])
    remaining.extend(midfielders[2:])
    
    # Shuffle remaining players
    random.shuffle(remaining)
    
    # Balance teams by total points using remaining players
    team1_points = sum(p.points for p in team1)
    team2_points = sum(p.points for p in team2)
    
    # Distribute remaining 6 players (3 to each team) to balance points
    for player in remaining:
        if len(team1) < 8:
            if team1_points <= team2_points:
                team1.append(player)
                team1_points += player.points
            else:
                team2.append(player)
                team2_points += player.points
        else:
            team2.append(player)
            team2_points += player.points
    
    return {
        "team1": {
            "players": [player.dict() for player in team1],
            "totalPoints": team1_points,
            "formation": get_formation(team1)
        },
        "team2": {
            "players": [player.dict() for player in team2],
            "totalPoints": team2_points,
            "formation": get_formation(team2)
        }
    }

def get_formation(team: List[Player]) -> str:
    """Get team formation as string like '4-3-3'"""
    def_count = len([p for p in team if p.position == 'DEF'])
    mid_count = len([p for p in team if p.position == 'MID']) 
    att_count = len([p for p in team if p.position == 'ATT'])
    return f"{def_count}-{mid_count}-{att_count}"