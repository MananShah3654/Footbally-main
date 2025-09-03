import React, { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import axios from 'axios';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const PlayerCustomizationModal = ({ player, isOpen, onClose, onSave }) => {
  const [formData, setFormData] = useState({
    name: '',
    position: '',
    age: '',
    nationality: '',
    height: '',
    weight: '',
    preferredFoot: 'Right',
    jerseyNumber: '',
    skills: {
      pac: 75,
      sho: 70,
      pas: 80,
      def: 65,
      dri: 75,
      phy: 70
    }
  });
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (player && isOpen) {
      setFormData({
        name: player.name || '',
        position: player.position || 'MID',
        age: player.age || 25,
        nationality: player.nationality || 'India',
        height: player.height || 180,
        weight: player.weight || 75,
        preferredFoot: player.preferredFoot || 'Right',
        jerseyNumber: player.jerseyNumber || 10,
        skills: {
          pac: player.skills?.pac || 75,
          sho: player.skills?.sho || 70,
          pas: player.skills?.pas || 80,
          def: player.skills?.def || 65,
          dri: player.skills?.dri || 75,
          phy: player.skills?.phy || 70
        }
      });
    }
  }, [player, isOpen]);

  const handleInputChange = (field, value) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleSkillChange = (skill, value) => {
    const numValue = Math.min(99, Math.max(1, parseInt(value) || 1));
    setFormData(prev => ({
      ...prev,
      skills: {
        ...prev.skills,
        [skill]: numValue
      }
    }));
  };

  const calculateOverallRating = () => {
    const { pac, sho, pas, def, dri, phy } = formData.skills;
    return Math.round((pac + sho + pas + def + dri + phy) / 6);
  };

  const handleSave = async () => {
    setLoading(true);
    try {
      const updatedPlayer = {
        ...player,
        ...formData,
        averageRating: calculateOverallRating()
      };

      if (player.id) {
        await axios.put(`${API}/players/${player.id}`, formData);
      } else {
        await axios.post(`${API}/players`, formData);
      }

      onSave(updatedPlayer);
      onClose();
    } catch (error) {
      console.error('Error saving player:', error);
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <Card className="w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <CardHeader>
          <CardTitle className="flex items-center justify-between">
            <span>⚙️ Customize Player</span>
            <Button variant="outline" size="sm" onClick={onClose}>✕</Button>
          </CardTitle>
        </CardHeader>
        
        <CardContent className="space-y-6">
          {/* Basic Info */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium mb-1">Name</label>
              <input
                type="text"
                value={formData.name}
                onChange={(e) => handleInputChange('name', e.target.value)}
                className="w-full p-2 border rounded-md"
                placeholder="Player Name"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">Position</label>
              <select
                value={formData.position}
                onChange={(e) => handleInputChange('position', e.target.value)}
                className="w-full p-2 border rounded-md"
              >
                <option value="DEF">Defender (DEF)</option>
                <option value="MID">Midfielder (MID)</option>
                <option value="ATT">Attacker (ATT)</option>
                <option value="GK">Goalkeeper (GK)</option>
              </select>
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">Age</label>
              <input
                type="number"
                value={formData.age}
                onChange={(e) => handleInputChange('age', parseInt(e.target.value))}
                className="w-full p-2 border rounded-md"
                min="16"
                max="45"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">Nationality</label>
              <input
                type="text"
                value={formData.nationality}
                onChange={(e) => handleInputChange('nationality', e.target.value)}
                className="w-full p-2 border rounded-md"
                placeholder="Country"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">Height (cm)</label>
              <input
                type="number"
                value={formData.height}
                onChange={(e) => handleInputChange('height', parseInt(e.target.value))}
                className="w-full p-2 border rounded-md"
                min="150"
                max="220"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">Weight (kg)</label>
              <input
                type="number"
                value={formData.weight}
                onChange={(e) => handleInputChange('weight', parseInt(e.target.value))}
                className="w-full p-2 border rounded-md"
                min="50"
                max="120"
              />
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">Preferred Foot</label>
              <select
                value={formData.preferredFoot}
                onChange={(e) => handleInputChange('preferredFoot', e.target.value)}
                className="w-full p-2 border rounded-md"
              >
                <option value="Right">Right</option>
                <option value="Left">Left</option>
                <option value="Both">Both</option>
              </select>
            </div>
            
            <div>
              <label className="block text-sm font-medium mb-1">Jersey Number</label>
              <input
                type="number"
                value={formData.jerseyNumber}
                onChange={(e) => handleInputChange('jerseyNumber', parseInt(e.target.value))}
                className="w-full p-2 border rounded-md"
                min="1"
                max="99"
              />
            </div>
          </div>

          {/* Skills Section */}
          <div>
            <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
              📊 Player Skills
              <span className="text-sm font-normal text-gray-600">
                (Overall: {calculateOverallRating()})
              </span>
            </h3>
            
            <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
              {Object.entries(formData.skills).map(([skill, value]) => (
                <div key={skill} className="space-y-2">
                  <label className="block text-sm font-medium capitalize">
                    {skill.toUpperCase()}
                  </label>
                  <div className="flex items-center gap-2">
                    <input
                      type="range"
                      min="1"
                      max="99"
                      value={value}
                      onChange={(e) => handleSkillChange(skill, e.target.value)}
                      className="flex-1"
                    />
                    <input
                      type="number"
                      value={value}
                      onChange={(e) => handleSkillChange(skill, e.target.value)}
                      className="w-16 p-1 border rounded text-center text-sm"
                      min="1"
                      max="99"
                    />
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Preview Card */}
          <div>
            <h3 className="text-lg font-semibold mb-4">🎴 Card Preview</h3>
            <div className="flex justify-center">
              <div className="w-64 h-80 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg p-4 text-white shadow-2xl">
                <div className="flex justify-between items-start mb-2">
                  <div className="text-center">
                    <div className="text-2xl font-bold">{calculateOverallRating()}</div>
                    <div className="text-xs">OVR</div>
                  </div>
                  <div className="bg-white bg-opacity-20 px-2 py-1 rounded text-xs">
                    {formData.position}
                  </div>
                </div>
                
                <div className="text-center mb-4">
                  <div className="w-20 h-20 bg-white bg-opacity-20 rounded-full mx-auto mb-2 flex items-center justify-center">
                    👤
                  </div>
                  <div className="relative">
                    <div className="absolute -bottom-1 -right-1 bg-white text-black rounded-full w-6 h-6 flex items-center justify-center text-xs font-bold">
                      {formData.jerseyNumber}
                    </div>
                  </div>
                </div>
                
                <div className="text-center mb-3">
                  <div className="font-bold text-lg">{formData.name || 'Player Name'}</div>
                  <div className="text-xs opacity-90">
                    {formData.nationality} • Age {formData.age}
                  </div>
                </div>
                
                <div className="grid grid-cols-3 gap-1 text-xs">
                  <div className="text-center">
                    <div className="font-bold">{formData.skills.pac}</div>
                    <div className="opacity-75">PAC</div>
                  </div>
                  <div className="text-center">
                    <div className="font-bold">{formData.skills.sho}</div>
                    <div className="opacity-75">SHO</div>
                  </div>
                  <div className="text-center">
                    <div className="font-bold">{formData.skills.pas}</div>
                    <div className="opacity-75">PAS</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-4 pt-4 border-t">
            <Button
              variant="outline"
              onClick={onClose}
              className="flex-1"
              disabled={loading}
            >
              Cancel
            </Button>
            <Button
              onClick={handleSave}
              className="flex-1 bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700"
              disabled={loading}
            >
              {loading ? (
                <div className="flex items-center gap-2">
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  Saving...
                </div>
              ) : (
                '💾 Save Player'
              )}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default PlayerCustomizationModal;