import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from './ui/dropdown-menu';
import PlayerCard from './PlayerCard';
import { Search, Users, Filter, Shuffle, MoreHorizontal, Edit, Trash2, Eye } from 'lucide-react';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const PlayerRoster = ({
  onPlayerClick,
  onEditPlayer,
  onDeletePlayer
}) => {
  const [players, setPlayers] = useState([]);
  const [selectedPlayers, setSelectedPlayers] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [positionFilter, setPositionFilter] = useState('ALL');
  const [isShuffling, setIsShuffling] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchPlayers();
  }, []);

  const fetchPlayers = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API}/players`);
      setPlayers(response.data);
    } catch (error) {
      console.error('Error fetching players:', error);
    } finally {
      setLoading(false);
    }
  };

  const toggleSelectPlayer = (playerId) => {
    setSelectedPlayers(prev => 
      prev.includes(playerId) 
        ? prev.filter(id => id !== playerId)
        : [...prev, playerId]
    );
  };

  const handleShuffle = () => {
    if (selectedPlayers.length < 2) {
      alert('Please select at least 2 players to shuffle teams');
      return;
    }
    
    setIsShuffling(true);
    setTimeout(() => {
      // Shuffle logic here - for now just simulate
      console.log('Shuffling teams with players:', selectedPlayers);
      setIsShuffling(false);
    }, 2000);
  };

  const filteredPlayers = players.filter(player => {
    const matchesSearch = player.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         (player.nationality || '').toLowerCase().includes(searchTerm.toLowerCase());
    
    const playerPos = player.position?.includes('Forward') ? 'ATT' : 
                     player.position?.includes('Defender') ? 'DEF' : 
                     player.position?.includes('Midfielder') ? 'MID' : 'MID';
    const matchesPosition = positionFilter === 'ALL' || playerPos === positionFilter;
    
    return matchesSearch && matchesPosition;
  });

  const positionCounts = {
    DEF: players.filter(p => p.position?.includes('Defender')).length,
    MID: players.filter(p => p.position?.includes('Midfielder')).length,
    ATT: players.filter(p => p.position?.includes('Forward')).length
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p>Loading players...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <Card className="p-6 bg-gradient-to-r from-slate-50 to-gray-50 border-0 shadow-md">
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div className="space-y-1">
            <h2 className="text-2xl font-bold flex items-center gap-2">
              <Users className="w-6 h-6 text-blue-600" />
              Player Roster
            </h2>
            <p className="text-muted-foreground">
              Manage your {players.length} players and create balanced teams
            </p>
          </div>
          
          <div className="flex flex-col gap-2 items-end">
            <div className="flex gap-2 mb-1">
              <Button
                size="sm"
                variant="outline"
                onClick={() => {
                  const allIds = filteredPlayers.slice(0, 16).map(p => p.id);
                  allIds.forEach(id => {
                    if (!selectedPlayers.includes(id)) toggleSelectPlayer(id);
                  });
                }}
                disabled={filteredPlayers.length === 0 || selectedPlayers.length === 16}
              >
                Select All
              </Button>
              <Button
                size="sm"
                variant="outline"
                onClick={() => {
                  filteredPlayers.forEach(p => {
                    if (selectedPlayers.includes(p.id)) toggleSelectPlayer(p.id);
                  });
                }}
                disabled={selectedPlayers.length === 0}
              >
                Select None
              </Button>
            </div>
            
            <Button
              onClick={handleShuffle}
              disabled={isShuffling}
              size="lg"
              className="bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white shadow-lg hover:shadow-xl transition-all duration-300"
            >
              {isShuffling ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-2 border-white border-t-transparent mr-2" />
                  Shuffling...
                </>
              ) : (
                <>
                  <Shuffle className="w-4 h-4 mr-2" />
                  Shuffle Teams
                </>
              )}
            </Button>
          </div>
        </div>
      </Card>

      {/* Filters and Stats */}
      <Card className="p-4">
        <div className="flex flex-col md:flex-row gap-4">
          {/* Search */}
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground w-4 h-4" />
            <Input
              placeholder="Search players by name or nationality..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>

          {/* Position Filter */}
          <div className="flex items-center gap-2">
            <Filter className="w-4 h-4 text-muted-foreground" />
            <div className="flex gap-1">
              {['ALL', 'DEF', 'MID', 'ATT'].map(pos => (
                <Button
                  key={pos}
                  variant={positionFilter === pos ? 'default' : 'outline'}
                  size="sm"
                  onClick={() => setPositionFilter(pos)}
                  className="min-w-[60px]"
                >
                  {pos}
                  {pos !== 'ALL' && (
                    <Badge variant="secondary" className="ml-1 text-xs">
                      {positionCounts[pos]}
                    </Badge>
                  )}
                </Button>
              ))}
            </div>
          </div>
        </div>

        {/* Quick Stats */}
        <div className="mt-4 pt-4 border-t flex flex-wrap gap-4">
          <div className="text-sm">
            <span className="text-muted-foreground">Total Players:</span>
            <Badge variant="secondary" className="ml-2">{players.length}</Badge>
          </div>
          <div className="text-sm">
            <span className="text-muted-foreground">Average Rating:</span>
            <Badge variant="secondary" className="ml-2">
              {players.length > 0 ? Math.round(
                players.reduce((sum, p) => sum + (p.averageRating || 85), 0) / players.length
              ) : 0}
            </Badge>
          </div>
          <div className="text-sm">
            <span className="text-muted-foreground">Filtered:</span>
            <Badge variant="secondary" className="ml-2">{filteredPlayers.length}</Badge>
          </div>
        </div>
      </Card>

      {/* Players Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 2xl:grid-cols-5 gap-4">
        {filteredPlayers.map((player) => (
          <div key={player.id} className="transform transition-all duration-300 group">
            <div className="relative">
              {/* Selection Checkbox */}
              <input
                type="checkbox"
                checked={selectedPlayers.includes(player.id)}
                onChange={() => toggleSelectPlayer(player.id)}
                className="absolute top-2 left-2 z-10 w-5 h-5 accent-blue-600 rounded border-gray-300 shadow focus:ring-2 focus:ring-blue-500"
                title={selectedPlayers.includes(player.id) ? 'Deselect' : 'Select for shuffle'}
              />
              
              <div onClick={() => onPlayerClick?.(player)}>
                <PlayerCard 
                  player={player} 
                  isSelected={selectedPlayers.includes(player.id)}
                />
              </div>

              {/* Action Menu */}
              <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity">
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button
                      variant="secondary"
                      size="sm"
                      className="h-8 w-8 p-0 bg-white/90 hover:bg-white shadow-md"
                    >
                      <MoreHorizontal className="w-4 h-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem onClick={() => onPlayerClick?.(player)}>
                      <Eye className="w-4 h-4 mr-2" />
                      View Profile
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={() => onEditPlayer?.(player)}>
                      <Edit className="w-4 h-4 mr-2" />
                      Edit Player
                    </DropdownMenuItem>
                    <DropdownMenuItem
                      onClick={() => onDeletePlayer?.(player.id)}
                      className="text-red-600 focus:text-red-600"
                    >
                      <Trash2 className="w-4 h-4 mr-2" />
                      Delete Player
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* No results message */}
      {filteredPlayers.length === 0 && (
        <Card className="p-8 text-center">
          <div className="space-y-2">
            <Users className="w-12 h-12 text-muted-foreground mx-auto" />
            <h3 className="text-lg font-semibold">No players found</h3>
            <p className="text-muted-foreground">
              Try adjusting your search or filter criteria
            </p>
          </div>
        </Card>
      )}
    </div>
  );
};

export default PlayerRoster;