import React from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { Badge } from './ui/badge';
import { Progress } from './ui/progress';
import { Card } from './ui/card';
import { Separator } from './ui/separator';
import {
  User,
  MapPin,
  Calendar,
  Activity,
  Target,
  Zap,
  Shield,
  Navigation,
  Move
} from 'lucide-react';

const PlayerProfileModal = ({ player, isOpen, onClose }) => {
  if (!player) return null;

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
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold">Player Profile</DialogTitle>
        </DialogHeader>

        <div className="space-y-6">
          {/* Header Card */}
          <Card className="p-6 bg-gradient-to-br from-slate-50 to-gray-50 border-0">
            {/* Position gradient header */}
            <div className={`h-2 bg-gradient-to-r ${positionColors[playerData.position]} rounded-t-lg -m-6 mb-4`} />
            
            <div className="flex items-start gap-6">
              {/* Player Photo */}
              <div className="relative flex-shrink-0">
                <img
                  src={playerData.photo}
                  alt={playerData.name}
                  className="w-24 h-32 object-cover rounded-xl shadow-lg"
                  onError={(e) => {
                    e.target.src = 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face';
                  }}
                />
                <div className="absolute -top-2 -right-2">
                  <Badge className={`${positionBg[playerData.position]} border-0 font-bold`}>
                    {playerData.position}
                  </Badge>
                </div>
              </div>

              {/* Basic Info */}
              <div className="flex-1 space-y-3">
                <div>
                  <h3 className="text-3xl font-bold text-gray-800">{playerData.name}</h3>
                  <div className="flex items-center gap-4 mt-2 text-muted-foreground">
                    <div className="flex items-center gap-1">
                      <MapPin className="w-4 h-4" />
                      <span>{playerData.nationality}</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <Calendar className="w-4 h-4" />
                      <span>{playerData.age} years old</span>
                    </div>
                    <div className="flex items-center gap-1">
                      <User className="w-4 h-4" />
                      <span>{playerData.preferredFoot} footed</span>
                    </div>
                  </div>
                </div>

                {/* Overall Rating */}
                <div className="flex items-center gap-4">
                  <div className={`px-6 py-3 rounded-2xl bg-gradient-to-r ${positionColors[playerData.position]} text-white shadow-lg`}>
                    <div className="text-center">
                      <div className="text-2xl font-bold">{playerData.points}</div>
                      <div className="text-xs opacity-90">OVERALL</div>
                    </div>
                  </div>
                  <div className="text-sm text-muted-foreground">
                    <div className="font-semibold text-gray-700">{getSkillLevel(playerData.points)}</div>
                    <div>Overall Rating</div>
                  </div>
                </div>
              </div>
            </div>
          </Card>

          {/* Skills Breakdown */}
          <Card className="p-6">
            <h4 className="text-lg font-semibold mb-4 flex items-center gap-2">
              <Activity className="w-5 h-5" />
              Skills Breakdown
            </h4>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {Object.entries(playerData.skills).map(([skill, value]) => (
                <div key={skill} className="space-y-2">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      {skillIcons[skill]}
                      <span className="font-medium capitalize">{skill}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <span className={`font-bold ${getSkillColor(value)}`}>
                        {value}
                      </span>
                      <span className="text-xs text-muted-foreground">
                        {getSkillLevel(value)}
                      </span>
                    </div>
                  </div>
                  <Progress value={value} className="h-2" />
                </div>
              ))}
            </div>
          </Card>

          {/* Stats Summary */}
          <Card className="p-6">
            <h4 className="text-lg font-semibold mb-4">Performance Summary</h4>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div className="text-center p-3 bg-blue-50 rounded-lg">
                <div className="text-2xl font-bold text-blue-600">
                  {Math.max(...Object.values(playerData.skills))}
                </div>
                <div className="text-sm text-muted-foreground">Best Skill</div>
              </div>
              <div className="text-center p-3 bg-green-50 rounded-lg">
                <div className="text-2xl font-bold text-green-600">
                  {Math.round(Object.values(playerData.skills).reduce((a, b) => a + b) / 6)}
                </div>
                <div className="text-sm text-muted-foreground">Avg Rating</div>
              </div>
              <div className="text-center p-3 bg-purple-50 rounded-lg">
                <div className="text-2xl font-bold text-purple-600">
                  {playerData.position}
                </div>
                <div className="text-sm text-muted-foreground">Position</div>
              </div>
              <div className="text-center p-3 bg-orange-50 rounded-lg">
                <div className="text-2xl font-bold text-orange-600">
                  {playerData.preferredFoot}
                </div>
                <div className="text-sm text-muted-foreground">Foot</div>
              </div>
            </div>
          </Card>

          {/* Strengths & Weaknesses */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Strengths */}
            <Card className="p-6">
              <h4 className="text-lg font-semibold mb-3 text-green-700">Strengths</h4>
              <div className="space-y-2">
                {Object.entries(playerData.skills)
                  .filter(([_, value]) => value >= 80)
                  .sort(([_, a], [__, b]) => b - a)
                  .slice(0, 3)
                  .map(([skill, value]) => (
                    <div key={skill} className="flex items-center justify-between p-2 bg-green-50 rounded-lg">
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
            </Card>

            {/* Areas for Improvement */}
            <Card className="p-6">
              <h4 className="text-lg font-semibold mb-3 text-orange-700">Areas for Improvement</h4>
              <div className="space-y-2">
                {Object.entries(playerData.skills)
                  .filter(([_, value]) => value < 70)
                  .sort(([_, a], [__, b]) => a - b)
                  .slice(0, 3)
                  .map(([skill, value]) => (
                    <div key={skill} className="flex items-center justify-between p-2 bg-orange-50 rounded-lg">
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
            </Card>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default PlayerProfileModal;