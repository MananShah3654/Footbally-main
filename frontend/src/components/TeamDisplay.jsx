import React from 'react';
import { Card } from './ui/card';
import { Badge } from './ui/badge';
import { Button } from './ui/button';
import { Trophy, Users, Target, Eye } from 'lucide-react';

const TeamDisplay = ({ team, teamName, teamColor, onPlayerClick }) => {
  const teamGradients = {
    1: 'from-blue-500 via-blue-600 to-indigo-600',
    2: 'from-orange-500 via-red-500 to-pink-600'
  };

  const teamBorders = {
    1: 'border-blue-200',
    2: 'border-orange-200'
  };

  const positionColors = {
    DEF: 'from-emerald-500 to-emerald-600',
    MID: 'from-amber-500 to-amber-600', 
    ATT: 'from-red-500 to-red-600'
  };

  const positionBg = {
    DEF: 'bg-emerald-100 text-emerald-800',
    MID: 'bg-amber-100 text-amber-800',
    ATT: 'bg-red-100 text-red-800'
  };

  return (
    <Card className={`p-6 ${teamBorders[teamColor]} border-2 bg-gradient-to-br from-white to-gray-50/50`}>
      {/* Team Header */}
      <div className="mb-6">
        <div className={`inline-flex items-center gap-2 px-4 py-2 rounded-full bg-gradient-to-r ${teamGradients[teamColor]} text-white shadow-lg`}>
          <Trophy className="w-5 h-5" />
          <span className="font-bold text-lg">{teamName}</span>
        </div>
        
        {/* Team Stats */}
        <div className="mt-4 flex flex-wrap gap-4">
          <div className="flex items-center gap-2 text-sm">
            <Target className="w-4 h-4 text-muted-foreground" />
            <span className="text-muted-foreground">Total Points:</span>
            <Badge variant="secondary" className="font-bold">
              {team.totalPoints}
            </Badge>
          </div>
          <div className="flex items-center gap-2 text-sm">
            <Users className="w-4 h-4 text-muted-foreground" />
            <span className="text-muted-foreground">Formation:</span>
            <Badge variant="outline" className="font-mono">
              {team.formation}
            </Badge>
          </div>
        </div>
      </div>

      {/* Players List - Compact for teams view */}
      <div className="space-y-3">
        {team.players.map((player) => {
          const isCaptain = team.captain && player.id === team.captain.id;
          return (
            <Card 
              key={player.id} 
              className="p-3 hover:shadow-md transition-all duration-200 cursor-pointer group"
            >
              {/* Position gradient header */}
              <div className={`h-1 bg-gradient-to-r ${positionColors[player.position]} rounded-t-lg -m-3 mb-3`} />
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3 flex-1">
                  {/* Player Photo */}
                  <img 
                    src={player.photo} 
                    alt={player.name}
                    className="w-12 h-16 object-cover rounded-md shadow-sm"
                    onError={(e) => {
                      e.target.src = 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face';
                    }}
                  />
                  {/* Player Info */}
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2">
                      <h4 
                        className="font-semibold text-sm truncate hover:text-blue-600 transition-colors cursor-pointer"
                        onClick={() => onPlayerClick?.(player)}
                      >
                        {player.name}
                        {isCaptain && (
                          <span title="Captain" className="ml-1 text-xs font-bold text-yellow-600 align-middle">C</span>
                        )}
                      </h4>
                      <Badge className={`text-xs ${positionBg[player.position]} border-0`}>
                        {player.position}
                      </Badge>
                    </div>
                    <div className="text-xs text-muted-foreground mt-1">
                      {player.nationality} • Age {player.age} • {player.preferredFoot}
                    </div>
                    {/* Key Skills */}
                    <div className="flex gap-2 mt-1 text-xs">
                      <span className="text-muted-foreground">PAC {player.skills.pace}</span>
                      <span className="text-muted-foreground">SHO {player.skills.shooting}</span>
                      <span className="text-muted-foreground">PAS {player.skills.passing}</span>
                    </div>
                  </div>
                </div>
                {/* Player Rating & Actions */}
                <div className="flex items-center gap-2">
                  <div className={`px-2 py-1 rounded-lg bg-gradient-to-r ${positionColors[player.position]} text-white font-bold text-sm`}>
                    {player.points}
                  </div>
                  <Button 
                    size="sm" 
                    variant="ghost" 
                    onClick={() => onPlayerClick?.(player)}
                    className="opacity-0 group-hover:opacity-100 transition-opacity h-8 w-8 p-0"
                  >
                    <Eye className="w-4 h-4" />
                  </Button>
                </div>
              </div>
            </Card>
          );
        })}
      </div>

      {/* Position Breakdown */}
      <div className="mt-6 pt-4 border-t border-gray-200">
        <h4 className="text-sm font-semibold text-muted-foreground mb-3">Position Breakdown</h4>
        <div className="flex flex-wrap gap-2">
          {['DEF', 'MID', 'ATT'].map(position => {
            const count = team.players.filter(p => p.position === position).length;
            const positionColors = {
              DEF: 'bg-emerald-100 text-emerald-700',
              MID: 'bg-amber-100 text-amber-700', 
              ATT: 'bg-red-100 text-red-700'
            };
            
            return (
              <Badge key={position} className={`${positionColors[position]} border-0`}>
                {position}: {count}
              </Badge>
            );
          })}
        </div>
      </div>
    </Card>
  );
};

export default TeamDisplay;