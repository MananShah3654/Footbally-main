import React from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Badge } from './ui/badge';
import { Button } from './ui/button';

const PlayerCard = ({ player, onViewDetails, onViewStats }) => {
  const getPositionColor = (position) => {
    const pos = position.toLowerCase();
    if (pos.includes('forward') || pos.includes('striker')) return 'bg-red-500';
    if (pos.includes('midfielder') || pos.includes('mid')) return 'bg-green-500';
    if (pos.includes('defender') || pos.includes('back')) return 'bg-blue-500';
    if (pos.includes('goalkeeper') || pos.includes('keeper')) return 'bg-yellow-500';
    if (pos.includes('winger') || pos.includes('wing')) return 'bg-purple-500';
    return 'bg-gray-500';
  };

  return (
    <Card className="hover:shadow-lg transition-shadow duration-200">
      <CardHeader className="pb-2">
        <div className="flex items-start justify-between">
          <div>
            <CardTitle className="text-lg font-bold">{player.name}</CardTitle>
            <div className="flex items-center gap-2 mt-1">
              <Badge variant="secondary" className={`${getPositionColor(player.position)} text-white`}>
                {player.position}
              </Badge>
              <span className="text-sm text-gray-500">#{player.jerseyNumber}</span>
            </div>
          </div>
          <div className="text-right">
            <div className="text-sm font-medium">{player.nationality}</div>
            <div className="text-xs text-gray-500">{player.age} years old</div>
          </div>
        </div>
      </CardHeader>
      
      <CardContent className="pt-2">
        <div className="grid grid-cols-2 gap-4 text-sm">
          <div>
            <span className="text-gray-500">Height:</span>
            <div className="font-medium">{player.height}cm</div>
          </div>
          <div>
            <span className="text-gray-500">Weight:</span>
            <div className="font-medium">{player.weight}kg</div>
          </div>
          <div>
            <span className="text-gray-500">Foot:</span>
            <div className="font-medium">{player.preferredFoot}</div>
          </div>
          <div>
            <span className="text-gray-500">Team:</span>
            <div className="font-medium text-blue-600">{player.teamId ? 'Team Player' : 'Free Agent'}</div>
          </div>
        </div>
        
        <div className="flex gap-2 mt-4">
          <Button 
            variant="outline" 
            size="sm" 
            onClick={() => onViewDetails(player)}
            className="flex-1"
          >
            View Profile
          </Button>
          <Button 
            variant="default" 
            size="sm" 
            onClick={() => onViewStats(player)}
            className="flex-1"
          >
            View Stats
          </Button>
        </div>
      </CardContent>
    </Card>
  );
};

export default PlayerCard;