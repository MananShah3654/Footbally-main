import React, { useState } from 'react';
import { Card, CardContent } from './ui/card';
import { Badge } from './ui/badge';
import { Button } from './ui/button';

const PlayerCard = ({ player, onEdit, onCustomize, showActions = true }) => {
  const [isFlipped, setIsFlipped] = useState(false);

  const getPositionColor = (position) => {
    const pos = position?.toLowerCase() || '';
    if (pos.includes('att') || pos.includes('forward') || pos.includes('striker')) return 'bg-red-500';
    if (pos.includes('mid') || pos.includes('midfielder')) return 'bg-green-500';
    if (pos.includes('def') || pos.includes('defender') || pos.includes('back')) return 'bg-blue-500';
    if (pos.includes('gk') || pos.includes('goalkeeper') || pos.includes('keeper')) return 'bg-yellow-500';
    return 'bg-gray-500';
  };

  const getCardGradient = (rating) => {
    if (rating >= 90) return 'from-yellow-400 via-yellow-500 to-amber-600'; // Gold
    if (rating >= 85) return 'from-gray-300 via-gray-400 to-gray-500'; // Silver
    if (rating >= 80) return 'from-orange-400 via-orange-500 to-orange-600'; // Bronze
    return 'from-gray-400 via-gray-500 to-gray-600'; // Common
  };

  const formatStat = (stat) => stat || 0;

  return (
    <div className="relative group">
      <div 
        className={`w-80 h-96 relative cursor-pointer transition-transform duration-700 ${
          isFlipped ? '[transform:rotateY(180deg)]' : ''
        }`}
        style={{ transformStyle: 'preserve-3d' }}
        onClick={() => setIsFlipped(!isFlipped)}
      >
        {/* Front of Card */}
        <Card className={`absolute inset-0 w-full h-full bg-gradient-to-br ${getCardGradient(player.averageRating || 85)} shadow-2xl border-4 border-white`}
              style={{ backfaceVisibility: 'hidden' }}>
          <CardContent className="p-4 h-full flex flex-col text-white">
            {/* Header */}
            <div className="flex justify-between items-start mb-2">
              <div className="text-center">
                <div className="text-3xl font-bold">{formatStat(player.averageRating || 85)}</div>
                <div className="text-xs font-semibold tracking-wider">OVR</div>
              </div>
              <div className="text-center">
                <Badge className={`${getPositionColor(player.position)} text-white text-xs px-2 py-1`}>
                  {player.position || 'MID'}
                </Badge>
              </div>
            </div>

            {/* Player Image */}
            <div className="flex-1 flex items-center justify-center">
              <div className="relative">
                <img
                  src={player.avatar || player.image || 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face'}
                  alt={player.name}
                  className="w-32 h-32 object-cover rounded-full border-4 border-white shadow-lg"
                />
                <div className="absolute -bottom-2 -right-2 bg-white text-black rounded-full w-8 h-8 flex items-center justify-center text-sm font-bold">
                  {player.jerseyNumber || '10'}
                </div>
              </div>
            </div>

            {/* Player Info */}
            <div className="text-center">
              <h3 className="text-xl font-bold mb-1 text-shadow">{player.name}</h3>
              <div className="flex items-center justify-center gap-2 text-sm opacity-90">
                <span>{player.nationality || 'India'}</span>
                <span>•</span>
                <span>Age {player.age || 25}</span>
              </div>
            </div>

            {/* Skills Preview */}
            <div className="grid grid-cols-3 gap-1 mt-3 text-xs">
              <div className="text-center">
                <div className="font-bold">{formatStat(player.skills?.pac || player.tackles || 75)}</div>
                <div className="opacity-75">PAC</div>
              </div>
              <div className="text-center">
                <div className="font-bold">{formatStat(player.skills?.sho || player.goals * 5 || 70)}</div>
                <div className="opacity-75">SHO</div>
              </div>
              <div className="text-center">
                <div className="font-bold">{formatStat(player.skills?.pas || player.assists * 10 || 80)}</div>
                <div className="opacity-75">PAS</div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Back of Card */}
        <Card className={`absolute inset-0 w-full h-full bg-gradient-to-br ${getCardGradient(player.averageRating || 85)} shadow-2xl border-4 border-white`}
              style={{ backfaceVisibility: 'hidden', transform: 'rotateY(180deg)' }}>
          <CardContent className="p-4 h-full flex flex-col text-white">
            <div className="text-center mb-4">
              <h3 className="text-lg font-bold">{player.name}</h3>
              <p className="text-sm opacity-90">Detailed Stats</p>
            </div>

            {/* Detailed Stats */}
            <div className="flex-1 space-y-3">
              <div className="grid grid-cols-2 gap-3 text-sm">
                <div className="flex justify-between">
                  <span>PAC:</span>
                  <span className="font-bold">{formatStat(player.skills?.pac || 75)}</span>
                </div>
                <div className="flex justify-between">
                  <span>SHO:</span>
                  <span className="font-bold">{formatStat(player.skills?.sho || 70)}</span>
                </div>
                <div className="flex justify-between">
                  <span>PAS:</span>
                  <span className="font-bold">{formatStat(player.skills?.pas || 80)}</span>
                </div>
                <div className="flex justify-between">
                  <span>DEF:</span>
                  <span className="font-bold">{formatStat(player.skills?.def || 65)}</span>
                </div>
                <div className="flex justify-between">
                  <span>DRI:</span>
                  <span className="font-bold">{formatStat(player.skills?.dri || 75)}</span>
                </div>
                <div className="flex justify-between">
                  <span>PHY:</span>
                  <span className="font-bold">{formatStat(player.skills?.phy || 70)}</span>
                </div>
              </div>

              <div className="border-t border-white border-opacity-30 pt-3 space-y-2 text-sm">
                <div className="flex justify-between">
                  <span>Height:</span>
                  <span>{player.height || 180}cm</span>
                </div>
                <div className="flex justify-between">
                  <span>Weight:</span>
                  <span>{player.weight || 75}kg</span>
                </div>
                <div className="flex justify-between">
                  <span>Foot:</span>
                  <span>{player.preferredFoot || 'Right'}</span>
                </div>
                <div className="flex justify-between">
                  <span>Games:</span>
                  <span>{player.gamesPlayed || 0}</span>
                </div>
                <div className="flex justify-between">
                  <span>Goals:</span>
                  <span>{player.goals || 0}</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Action Buttons (Hover) */}
      {showActions && (
        <div className="absolute -bottom-2 left-1/2 transform -translate-x-1/2 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
          <div className="flex gap-2">
            <Button
              size="sm"
              variant="outline"
              className="bg-white/90 hover:bg-white text-black"
              onClick={(e) => {
                e.stopPropagation();
                onCustomize && onCustomize(player);
              }}
            >
              ⚙️ Edit
            </Button>
            <Button
              size="sm"
              variant="outline"
              className="bg-white/90 hover:bg-white text-black"
              onClick={(e) => {
                e.stopPropagation();
                onEdit && onEdit(player);
              }}
            >
              📊 Stats
            </Button>
          </div>
        </div>
      )}
    </div>
  );
};

export default PlayerCard;