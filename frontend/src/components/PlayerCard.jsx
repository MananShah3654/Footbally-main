import React from 'react';
import { Card } from './ui/card';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { CheckCircle } from 'lucide-react';

const PlayerCard = ({ player, isSelected = false, onClick = null }) => {
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
    <Card 
      className={`relative overflow-hidden transition-all duration-300 hover:scale-105 hover:shadow-xl cursor-pointer group ${
        isSelected ? 'ring-2 ring-blue-500 shadow-lg' : ''
      }`}
      onClick={onClick}
    >
      {/* Header with gradient */}
      <div className={`h-3 bg-gradient-to-r ${positionColors[player.position]}`} />
      
      {/* Card content */}
      <div className="p-4 space-y-4">
        {/* Player photo and basic info */}
        <div className="flex items-start gap-3">
          <div className="relative">
            <img 
              src={player.photo} 
              alt={player.name}
              className="w-16 h-20 object-cover rounded-lg shadow-md"
              onError={(e) => {
                e.target.src = 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face';
                
              }}
            />
            <div className="absolute -top-1 -right-1">
              <Badge className={`text-xs px-2 py-1 ${positionBg[player.position]} border-0`}>
                {player.position}
              </Badge>
            </div>
          </div>
          
          <div className="flex-1 space-y-1">
            <div className="flex items-center gap-2">
              <h3 className="font-bold text-lg leading-tight">{player.name}</h3>
              {player.isSubscribed && (
                <CheckCircle className="w-4 h-4 text-green-600" />
              )}
            </div>
            <p className="text-sm text-muted-foreground">{player.nationality} • Age {player.age}</p>
            
            {/* Overall rating */}
            <div className="flex items-center gap-2">
              <div className={`px-3 py-1 rounded-full bg-gradient-to-r ${positionColors[player.position]} text-white font-bold text-sm`}>
                {player.points}
              </div>
              <span className="text-xs text-muted-foreground">OVR</span>
            </div>
          </div>
        </div>

        {/* Skills */}
        <div className="space-y-2">
          <h4 className="text-xs font-semibold text-muted-foreground uppercase tracking-wide">Skills</h4>
          <div className="grid grid-cols-2 gap-2 text-xs">
            <div className="flex justify-between">
              <span>PAC</span>
              <span className="font-semibold">{player.skills.pace}</span>
            </div>
            <div className="flex justify-between">
              <span>SHO</span>
              <span className="font-semibold">{player.skills.shooting}</span>
            </div>
            <div className="flex justify-between">
              <span>PAS</span>
              <span className="font-semibold">{player.skills.passing}</span>
            </div>
            <div className="flex justify-between">
              <span>DEF</span>
              <span className="font-semibold">{player.skills.defending}</span>
            </div>
            <div className="flex justify-between">
              <span>DRI</span>
              <span className="font-semibold">{player.skills.dribbling}</span>
            </div>
            <div className="flex justify-between">
              <span>PHY</span>
              <span className="font-semibold">{player.skills.physical}</span>
            </div>
          </div>
        </div>

        {/* Preferred foot */}
        <div className="flex items-center justify-between text-xs text-muted-foreground">
          <span>Preferred Foot</span>
          <Badge variant="outline" className="text-xs">
            {player.preferredFoot}
          </Badge>
        </div>
      </div>

      {/* Hover effect overlay */}
      <div className="absolute inset-0 bg-gradient-to-br from-transparent to-black/5 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
    </Card>
  );
};

export default PlayerCard;