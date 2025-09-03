import React from 'react';
import MobileBottomSheet from './MobileBottomSheet';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Button } from './ui/button';
import {
  MapPin,
  Calendar,
  Activity,
  Target,
  Zap,
  Shield,
  Navigation,
  Move,
  Edit,
  Share,
  Heart
} from 'lucide-react';

const MobilePlayerProfile = ({ player, isOpen, onClose, onEdit }) => {
  if (!player) return null;

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

  // Handle different position formats
  const playerPos = player.position?.includes('Forward') ? 'ATT' : 
                   player.position?.includes('Defender') ? 'DEF' : 
                   player.position?.includes('Midfielder') ? 'MID' : 
                   player.position || 'MID';

  const skillIcons = {
    pace: <Zap className="w-4 h-4" />,
    shooting: <Target className="w-4 h-4" />,
    passing: <Navigation className="w-4 h-4" />,
    defending: <Shield className="w-4 h-4" />,
    dribbling: <Move className="w-4 h-4" />,
    physical: <Activity className="w-4 h-4" />
  };

  const getSkillColor = (value) => {
    if (value >= 90) return 'text-green-600';
    if (value >= 80) return 'text-blue-600';
    if (value >= 70) return 'text-yellow-600';
    if (value >= 60) return 'text-orange-600';
    return 'text-red-600';
  };

  const getSkillLevel = (value) => {
    if (value >= 90) return 'Excellent';
    if (value >= 80) return 'Very Good';
    if (value >= 70) return 'Good';
    if (value >= 60) return 'Average';
    return 'Needs Work';
  };

  // Convert API data to match expected structure
  const playerData = {
    ...player,
    position: playerPos,
    photo: player.avatar || player.image || player.photo || 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face',
    points: player.averageRating || player.points || 85,
    skills: {
      pace: player.skills?.pac || Math.floor(Math.random() * 30) + 70,
      shooting: player.skills?.sho || Math.floor(Math.random() * 30) + 70,
      passing: player.skills?.pas || Math.floor(Math.random() * 30) + 70,
      defending: player.skills?.def || Math.floor(Math.random() * 30) + 70,
      dribbling: player.skills?.dri || Math.floor(Math.random() * 30) + 70,
      physical: player.skills?.phy || Math.floor(Math.random() * 30) + 70
    }
  };

  return (
    <MobileBottomSheet
      isOpen={isOpen}
      onClose={onClose}
      title="Player Profile"
      maxHeight="95vh"
    >
      <div className="space-y-6">
        {/* Hero Section */}
        <div className={`relative bg-gradient-to-br ${positionColors[playerData.position]} text-white p-6 mx-6 mt-4 rounded-2xl overflow-hidden`}>
          <div className="flex items-center gap-4">
            <div className="relative">
              <img
                src={playerData.photo}
                alt={playerData.name}
                className="w-20 h-20 object-cover rounded-full border-4 border-white shadow-lg"
                onError={(e) => {
                  e.target.src = 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face';
                }}
              />
              <div className="absolute -bottom-1 -right-1 bg-white text-black text-xs font-bold px-2 py-1 rounded-full">
                {playerData.position}
              </div>
            </div>

            <div className="flex-1">
              <h2 className="text-2xl font-bold mb-1">{playerData.name}</h2>
              <div className="flex items-center gap-3 text-sm opacity-90 mb-3">
                <span className="flex items-center gap-1">
                  <MapPin className="w-3 h-3" />
                  {playerData.nationality}
                </span>
                <span className="flex items-center gap-1">
                  <Calendar className="w-3 h-3" />
                  {playerData.age}y
                </span>
              </div>
              
              <div className="bg-white bg-opacity-20 rounded-full px-4 py-2 inline-block">
                <span className="text-2xl font-bold">{playerData.points}</span>
                <span className="text-sm ml-1 opacity-90">OVR</span>
              </div>
            </div>
          </div>

          {/* Action buttons */}
          <div className="flex gap-2 mt-4">
            <Button
              variant="secondary"
              size="sm"
              className="flex-1 bg-white bg-opacity-20 border-white border-opacity-30 text-white hover:bg-white hover:bg-opacity-30"
              onClick={() => onEdit?.(player)}
            >
              <Edit className="w-4 h-4 mr-2" />
              Edit
            </Button>
            <Button
              variant="secondary"
              size="sm"
              className="bg-white bg-opacity-20 border-white border-opacity-30 text-white hover:bg-white hover:bg-opacity-30"
            >
              <Share className="w-4 h-4" />
            </Button>
            <Button
              variant="secondary"
              size="sm"
              className="bg-white bg-opacity-20 border-white border-opacity-30 text-white hover:bg-white hover:bg-opacity-30"
            >
              <Heart className="w-4 h-4" />
            </Button>
          </div>
        </div>

        {/* Skills Section */}
        <div className="px-6">
          <h3 className="text-lg font-bold mb-4 flex items-center gap-2">
            <Activity className="w-5 h-5 text-blue-500" />
            Skills Breakdown
          </h3>
          
          <div className="space-y-4">
            {Object.entries(playerData.skills).map(([skill, value]) => (
              <div key={skill} className="bg-gray-50 rounded-xl p-4">
                <div className="flex items-center justify-between mb-2">
                  <div className="flex items-center gap-3">
                    <div className="p-2 bg-white rounded-lg shadow-sm">
                      {skillIcons[skill]}
                    </div>
                    <span className="font-semibold capitalize text-gray-900">{skill}</span>
                  </div>
                  <div className="text-right">
                    <span className={`font-bold text-lg ${getSkillColor(value)}`}>
                      {value}
                    </span>
                    <div className="text-xs text-gray-500">
                      {getSkillLevel(value)}
                    </div>
                  </div>
                </div>
                <Progress value={value} className="h-2 bg-gray-200" />
              </div>
            ))}
          </div>
        </div>

        {/* Performance Summary */}
        <div className="px-6">
          <h3 className="text-lg font-bold mb-4">Performance Summary</h3>
          <div className="grid grid-cols-2 gap-3">
            <div className="bg-blue-50 p-4 rounded-xl text-center">
              <div className="text-2xl font-bold text-blue-600 mb-1">
                {Math.max(...Object.values(playerData.skills))}
              </div>
              <div className="text-sm text-gray-600">Best Skill</div>
            </div>
            
            <div className="bg-green-50 p-4 rounded-xl text-center">
              <div className="text-2xl font-bold text-green-600 mb-1">
                {Math.round(Object.values(playerData.skills).reduce((a, b) => a + b) / 6)}
              </div>
              <div className="text-sm text-gray-600">Avg Rating</div>
            </div>
            
            <div className="bg-purple-50 p-4 rounded-xl text-center">
              <div className="text-2xl font-bold text-purple-600 mb-1">
                {playerData.position}
              </div>
              <div className="text-sm text-gray-600">Position</div>
            </div>
            
            <div className="bg-orange-50 p-4 rounded-xl text-center">
              <div className="text-2xl font-bold text-orange-600 mb-1">
                {playerData.preferredFoot}
              </div>
              <div className="text-sm text-gray-600">Foot</div>
            </div>
          </div>
        </div>

        {/* Strengths & Weaknesses */}
        <div className="px-6 pb-6">
          <div className="grid grid-cols-1 gap-6">
            {/* Strengths */}
            <div className="bg-green-50 rounded-xl p-4">
              <h4 className="font-bold text-green-800 mb-3 flex items-center gap-2">
                <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                Strengths
              </h4>
              <div className="space-y-2">
                {Object.entries(playerData.skills)
                  .filter(([_, value]) => value >= 80)
                  .sort(([_, a], [__, b]) => b - a)
                  .slice(0, 3)
                  .map(([skill, value]) => (
                    <div key={skill} className="flex items-center justify-between bg-white rounded-lg p-3">
                      <div className="flex items-center gap-2">
                        {skillIcons[skill]}
                        <span className="capitalize font-medium">{skill}</span>
                      </div>
                      <Badge className="bg-green-100 text-green-700 border-0">
                        {value}
                      </Badge>
                    </div>
                  ))
                }
              </div>
            </div>

            {/* Areas for Improvement */}
            <div className="bg-orange-50 rounded-xl p-4">
              <h4 className="font-bold text-orange-800 mb-3 flex items-center gap-2">
                <div className="w-2 h-2 bg-orange-500 rounded-full"></div>
                Areas for Improvement
              </h4>
              <div className="space-y-2">
                {Object.entries(playerData.skills)
                  .filter(([_, value]) => value < 75)
                  .sort(([_, a], [__, b]) => a - b)
                  .slice(0, 3)
                  .map(([skill, value]) => (
                    <div key={skill} className="flex items-center justify-between bg-white rounded-lg p-3">
                      <div className="flex items-center gap-2">
                        {skillIcons[skill]}
                        <span className="capitalize font-medium">{skill}</span>
                      </div>
                      <Badge className="bg-orange-100 text-orange-700 border-0">
                        {value}
                      </Badge>
                    </div>
                  ))
                }
              </div>
            </div>
          </div>
        </div>
      </div>
    </MobileBottomSheet>
  );
};

export default MobilePlayerProfile;