import React from 'react';
import { Card } from './ui/card';
import { Badge } from './ui/badge';
import { CheckCircle, ChevronRight, Check } from 'lucide-react';

const MobilePlayerCard = ({ player, isSelected = false, onClick = null, onLongPress = null, onSelectionToggle = null, selectionMode = false }) => {
  const positionColors = {
    DEF: 'from-emerald-400 to-emerald-500',
    MID: 'from-amber-400 to-amber-500',
    ATT: 'from-red-400 to-red-500'
  };

  const positionBg = {
    DEF: 'bg-emerald-500',
    MID: 'bg-amber-500',
    ATT: 'bg-red-500'
  };

  // Handle different data structures from API vs original
  const playerData = {
    name: player.name || 'Unknown Player',
    position: player.position?.includes('Forward') ? 'ATT' : 
              player.position?.includes('Defender') ? 'DEF' : 
              player.position?.includes('Midfielder') ? 'MID' : 
              player.position || 'MID',
    nationality: player.nationality || 'Unknown',
    age: player.age || 25,
    photo: player.avatar || player.image || player.photo || 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face',
    points: player.averageRating || player.points || 85,
    skills: {
      pace: player.skills?.pac || Math.floor(Math.random() * 30) + 70,
      shooting: player.skills?.sho || Math.floor(Math.random() * 30) + 70,
      passing: player.skills?.pas || Math.floor(Math.random() * 30) + 70,
      defending: player.skills?.def || Math.floor(Math.random() * 30) + 70,
      dribbling: player.skills?.dri || Math.floor(Math.random() * 30) + 70,
      physical: player.skills?.phy || Math.floor(Math.random() * 30) + 70
    },
    preferredFoot: player.preferredFoot || 'Right',
    isSubscribed: player.isSubscribed || false
  };

  const handleCardClick = (e) => {
    e.preventDefault();
    if (selectionMode) {
      onSelectionToggle?.(player.id);
    } else {
      onClick?.(player);
    }
  };

  const handleSelectionClick = (e) => {
    e.stopPropagation();
    onSelectionToggle?.(player.id);
  };

  const handleTouchStart = (e) => {
    e.currentTarget.style.transform = 'scale(0.98)';
    e.currentTarget.style.transition = 'transform 0.1s ease';
  };

  const handleTouchEnd = (e) => {
    e.currentTarget.style.transform = 'scale(1)';
  };

  return (
    <Card
      className={`relative overflow-hidden transition-all duration-200 cursor-pointer active:scale-98 ${
        isSelected ? 'ring-2 ring-blue-500 shadow-lg bg-blue-50' : 'shadow-md hover:shadow-lg'
      } mx-2 mb-4`}
      onClick={handleCardClick}
      onTouchStart={handleTouchStart}
      onTouchEnd={handleTouchEnd}
    >
      {/* Mobile-style header gradient */}
      <div className={`h-2 bg-gradient-to-r ${positionColors[playerData.position]}`} />
      
      {/* Selection checkbox - always visible for easy access */}
      <div className="absolute top-4 left-4 z-10">
        <button
          onClick={handleSelectionClick}
          className={`w-8 h-8 rounded-full border-2 flex items-center justify-center transition-all ${
            isSelected 
              ? 'bg-blue-500 border-blue-500 text-white' 
              : 'bg-white border-gray-300 hover:border-blue-400'
          }`}
        >
          {isSelected && <Check className="w-4 h-4" />}
        </button>
      </div>
      
      {/* Main content */}
      <div className="p-4 pl-14"> {/* Extra left padding for checkbox */}
        <div className="flex items-center gap-4">
          {/* Player Avatar */}
          <div className="relative flex-shrink-0">
            <img
              src={playerData.photo}
              alt={playerData.name}
              className="w-16 h-16 object-cover rounded-full border-4 border-white shadow-md"
              onError={(e) => {
                e.target.src = 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face';
              }}
            />
            {/* Position badge */}
            <div className={`absolute -bottom-1 -right-1 ${positionBg[playerData.position]} text-white text-xs font-bold px-2 py-1 rounded-full`}>
              {playerData.position}
            </div>
          </div>

          {/* Player Info */}
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-2 mb-1">
              <h3 className="font-bold text-lg text-gray-900 truncate">{playerData.name}</h3>
              {playerData.isSubscribed && (
                <CheckCircle className="w-4 h-4 text-green-500 flex-shrink-0" />
              )}
            </div>
            
            <div className="flex items-center gap-3 text-sm text-gray-600 mb-2">
              <span className="truncate">{playerData.nationality}</span>
              <span>•</span>
              <span>{playerData.age}y</span>
              <span>•</span>
              <span>{playerData.preferredFoot}</span>
            </div>

            {/* Mobile stats row */}
            <div className="flex items-center gap-4">
              <div className={`px-3 py-1 rounded-full bg-gradient-to-r ${positionColors[playerData.position]} text-white font-bold text-sm`}>
                {playerData.points} OVR
              </div>
              <div className="flex gap-2 text-xs">
                <span className="bg-gray-100 px-2 py-1 rounded">PAC {playerData.skills.pace}</span>
                <span className="bg-gray-100 px-2 py-1 rounded">SHO {playerData.skills.shooting}</span>
              </div>
            </div>
          </div>

          {/* Mobile chevron - only show when not in selection mode */}
          {!selectionMode && <ChevronRight className="w-5 h-5 text-gray-400 flex-shrink-0" />}
        </div>
      </div>

      {/* Touch feedback overlay */}
      <div className="absolute inset-0 bg-black opacity-0 active:opacity-5 transition-opacity duration-75 pointer-events-none" />
    </Card>
  );
};

export default MobilePlayerCard;