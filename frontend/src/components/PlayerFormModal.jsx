import React, { useState, useEffect } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Card } from './ui/card';
import { Slider } from './ui/slider';
import { Switch } from './ui/switch';
import { Save, X, User, Camera, CheckCircle } from 'lucide-react';

const PlayerFormModal = ({ player, isOpen, onClose, onSave }) => {
  const [formData, setFormData] = useState({
    name: '',
    position: 'MID',
    points: 75,
    photo: '',
    age: 25,
    preferredFoot: 'Right',
    nationality: '',
    isSubscribed: false,
    skills: {
      pace: 75,
      shooting: 75,
      passing: 75,
      defending: 75,
      dribbling: 75,
      physical: 75
    }
  });
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Reset form when modal opens/closes or player changes
  useEffect(() => {
    if (player) {
      setFormData({
        ...player,
        isSubscribed: player.isSubscribed || false,
        skills: {
          pace: player.skills?.pace || 75,
          shooting: player.skills?.shooting || 75,
          passing: player.skills?.passing || 75,
          defending: player.skills?.defending || 75,
          dribbling: player.skills?.dribbling || 75,
          physical: player.skills?.physical || 75
        }
      });
    } else {
      setFormData({
        name: '',
        position: 'MID',
        points: 75,
        photo: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=400&h=600&fit=crop&crop=face',
        age: 25,
        preferredFoot: 'Right',
        nationality: '',
        isSubscribed: false,
        skills: {
          pace: 75,
          shooting: 75,
          passing: 75,
          defending: 75,
          dribbling: 75,
          physical: 75
        }
      });
    }
  }, [player, isOpen]);

  // Calculate overall points based on skills and position
  useEffect(() => {
    const { skills, position } = formData;
    let overall = 0;

    // Position-based skill weighting
    if (position === 'DEF') {
      overall = Math.round(
        (skills.defending * 0.3) +
        (skills.physical * 0.25) +
        (skills.passing * 0.2) +
        (skills.pace * 0.15) +
        (skills.dribbling * 0.05) +
        (skills.shooting * 0.05)
      );
    } else if (position === 'MID') {
      overall = Math.round(
        (skills.passing * 0.3) +
        (skills.dribbling * 0.2) +
        (skills.defending * 0.15) +
        (skills.pace * 0.15) +
        (skills.shooting * 0.1) +
        (skills.physical * 0.1)
      );
    } else { // ATT
      overall = Math.round(
        (skills.shooting * 0.3) +
        (skills.dribbling * 0.25) +
        (skills.pace * 0.2) +
        (skills.passing * 0.15) +
        (skills.physical * 0.05) +
        (skills.defending * 0.05)
      );
    }

    setFormData(prev => ({ ...prev, points: Math.min(99, Math.max(1, overall)) }));
  }, [formData.skills, formData.position]);

  const handleSkillChange = (skill, newValue) => {
    setFormData(prev => ({
      ...prev,
      skills: {
        ...prev.skills,
        [skill]: newValue[0]
      }
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSubmitting(true);
    
    try {
      await onSave(formData);
    } catch (error) {
      console.error('Error saving player:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  const positionColors = {
    DEF: 'from-emerald-500 to-emerald-600',
    MID: 'from-amber-500 to-amber-600',
    ATT: 'from-red-500 to-red-600'
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold flex items-center gap-2">
            <User className="w-6 h-6" />
            {player ? 'Edit Player' : 'Create New Player'}
          </DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Left Column - Basic Info */}
            <Card className="p-6 space-y-4">
              <h3 className="text-lg font-semibold">Basic Information</h3>
              
              <div>
                <Label htmlFor="name" className="flex items-center gap-2">
                  <User className="w-4 h-4" />
                  Player Name
                </Label>
                <Input
                  id="name"
                  value={formData.name}
                  onChange={(e) => setFormData(prev => ({ ...prev, name: e.target.value }))}
                  placeholder="Enter player name"
                  required
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="position">Position</Label>
                  <Select value={formData.position} onValueChange={(value) => setFormData(prev => ({ ...prev, position: value }))}>
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="DEF">Defender (DEF)</SelectItem>
                      <SelectItem value="MID">Midfielder (MID)</SelectItem>
                      <SelectItem value="ATT">Attacker (ATT)</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label htmlFor="age">Age</Label>
                  <Input
                    id="age"
                    type="number"
                    min="16"
                    max="45"
                    value={formData.age}
                    onChange={(e) => setFormData(prev => ({ ...prev, age: parseInt(e.target.value) || 25 }))}
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="nationality">Nationality</Label>
                  <Input
                    id="nationality"
                    value={formData.nationality}
                    onChange={(e) => setFormData(prev => ({ ...prev, nationality: e.target.value }))}
                    placeholder="Country"
                  />
                </div>

                <div>
                  <Label htmlFor="preferredFoot">Preferred Foot</Label>
                  <Select value={formData.preferredFoot} onValueChange={(value) => setFormData(prev => ({ ...prev, preferredFoot: value }))}>
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="Right">Right</SelectItem>
                      <SelectItem value="Left">Left</SelectItem>
                      <SelectItem value="Both">Both</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              </div>

              <div className="flex items-center justify-between">
                <Label htmlFor="subscribed" className="flex items-center gap-2">
                  <CheckCircle className="w-4 h-4" />
                  Subscribed Member
                </Label>
                <Switch
                  id="subscribed"
                  checked={formData.isSubscribed}
                  onCheckedChange={(checked) => setFormData(prev => ({ ...prev, isSubscribed: checked }))}
                />
              </div>

              <div>
                <Label htmlFor="photo" className="flex items-center gap-2">
                  <Camera className="w-4 h-4" />
                  Photo URL
                </Label>
                <Input
                  id="photo"
                  value={formData.photo}
                  onChange={(e) => setFormData(prev => ({ ...prev, photo: e.target.value }))}
                  placeholder="https://example.com/player-photo.jpg"
                />
              </div>
            </Card>

            {/* Right Column - Skills */}
            <Card className="p-6 space-y-4">
              <div className="flex items-center justify-between">
                <h3 className="text-lg font-semibold">Skills</h3>
                <div className={`px-3 py-1 rounded-full bg-gradient-to-r ${positionColors[formData.position]} text-white font-bold`}>
                  OVR: {formData.points}
                </div>
              </div>

              <div className="space-y-4">
                {Object.entries(formData.skills).map(([skill, value]) => (
                  <div key={skill} className="space-y-2">
                    <div className="flex justify-between items-center">
                      <Label className="capitalize font-medium">{skill}</Label>
                      <span className="font-bold text-lg w-8 text-center">{value}</span>
                    </div>
                    <Slider
                      value={[value]}
                      onValueChange={(newValue) => handleSkillChange(skill, newValue)}
                      max={99}
                      min={1}
                      step={1}
                      className="w-full"
                    />
                  </div>
                ))}
              </div>
            </Card>
          </div>

          {/* Submit Buttons */}
          <div className="flex justify-end gap-3 pt-4 border-t">
            <Button type="button" variant="outline" onClick={onClose}>
              <X className="w-4 h-4 mr-2" />
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              <Save className="w-4 h-4 mr-2" />
              {isSubmitting ? 'Saving...' : player ? 'Update Player' : 'Create Player'}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
};

export default PlayerFormModal;