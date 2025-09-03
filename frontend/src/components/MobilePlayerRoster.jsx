import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Card } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Input } from './ui/input';
import MobilePlayerCard from './MobilePlayerCard';
import MobileBottomSheet from './MobileBottomSheet';
import { Search, Users, Filter, Shuffle, Plus, Settings } from 'lucide-react';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const MobilePlayerRoster = ({ onPlayerClick, onEditPlayer, onDeletePlayer }) => {
  const [players, setPlayers] = useState([]);
  const [selectedPlayers, setSelectedPlayers] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [positionFilter, setPositionFilter] = useState('ALL');
  const [isShuffling, setIsShuffling] = useState(false);
  const [loading, setLoading] = useState(true);
  const [showSearch, setShowSearch] = useState(false);
  const [showFilters, setShowFilters] = useState(false);

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

  const handlePlayerClick = (player) => {
    if (selectedPlayers.length === 0) {
      onPlayerClick?.(player);
    }
  };

  const handleShuffle = () => {
    if (selectedPlayers.length < 2) {
      alert('Please select at least 2 players to shuffle teams');
      return;
    }
    
    setIsShuffling(true);
    setTimeout(() => {
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
      <div className="flex items-center justify-center min-h-screen bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500 mx-auto mb-4"></div>
          <p className="text-gray-600">Loading players...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 pb-20">
      {/* Mobile stats header */}
      <div className="bg-white border-b border-gray-200 px-4 py-4">
        <div className="flex items-center justify-between mb-4">
          <div>
            <h2 className="text-xl font-bold text-gray-900">Squad</h2>
            <p className="text-sm text-gray-600">{players.length} players available</p>
          </div>
          
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setShowSearch(!showSearch)}
              className="p-2 h-auto"
            >
              <Search className="w-4 h-4" />
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setShowFilters(true)}
              className="p-2 h-auto"
            >
              <Filter className="w-4 h-4" />
            </Button>
          </div>
        </div>

        {/* Search bar */}
        {showSearch && (
          <div className="mb-4">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-4 h-4" />
              <Input
                placeholder="Search players..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pl-10 rounded-full border-gray-300"
              />
            </div>
          </div>
        )}

        {/* Quick stats and selection controls */}
        <div className="flex items-center justify-between">
          <div className="flex gap-4 text-sm">
            <div className="flex items-center gap-1">
              <div className="w-3 h-3 bg-emerald-500 rounded"></div>
              <span>DEF {positionCounts.DEF}</span>
            </div>
            <div className="flex items-center gap-1">
              <div className="w-3 h-3 bg-amber-500 rounded"></div>
              <span>MID {positionCounts.MID}</span>
            </div>
            <div className="flex items-center gap-1">
              <div className="w-3 h-3 bg-red-500 rounded"></div>
              <span>ATT {positionCounts.ATT}</span>
            </div>
          </div>
          
          {/* Selection mode toggle */}
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => {
                if (selectedPlayers.length === 0) {
                  // Select first 2 players to demonstrate
                  const firstTwoPlayers = filteredPlayers.slice(0, 2).map(p => p.id);
                  setSelectedPlayers(firstTwoPlayers);
                } else {
                  setSelectedPlayers([]);
                }
              }}
              className="text-xs"
            >
              {selectedPlayers.length > 0 ? 'Clear All' : 'Select'}
            </Button>
          </div>
        </div>
      </div>

      {/* Players list */}
      <div className="px-2 py-4">
        {filteredPlayers.map((player) => (
          <MobilePlayerCard
            key={player.id}
            player={player}
            isSelected={selectedPlayers.includes(player.id)}
            onClick={handlePlayerClick}
            onSelectionToggle={toggleSelectPlayer}
            selectionMode={selectedPlayers.length > 0}
          />
        ))}
      </div>

      {/* No results */}
      {filteredPlayers.length === 0 && (
        <div className="text-center py-12">
          <Users className="w-16 h-16 text-gray-300 mx-auto mb-4" />
          <h3 className="text-lg font-semibold text-gray-600 mb-2">No players found</h3>
          <p className="text-gray-500">Try adjusting your search or filters</p>
        </div>
      )}

      {/* Floating Shuffle Button - Always visible for demo */}
      <div className="fixed bottom-24 right-6 z-40">
        <Button
          onClick={handleShuffle}
          disabled={isShuffling}
          className={`w-16 h-16 rounded-full shadow-2xl transition-all ${
            selectedPlayers.length >= 2 
              ? 'bg-blue-500 hover:bg-blue-600 scale-110' 
              : 'bg-gray-400'
          }`}
        >
          {isShuffling ? (
            <div className="animate-spin rounded-full h-6 w-6 border-2 border-white border-t-transparent" />
          ) : (
            <div className="flex flex-col items-center">
              <Shuffle className="w-5 h-5 mb-1" />
              <span className="text-xs">Shuffle</span>
            </div>
          )}
        </Button>
      </div>

      {/* Selection counter badge */}
      {selectedPlayers.length > 0 && (
        <div className="fixed bottom-32 right-2 z-30">
          <div className="bg-blue-500 text-white px-3 py-1 rounded-full text-sm font-bold shadow-lg">
            {selectedPlayers.length} selected
          </div>
        </div>
      )}

      {/* Selection bar */}
      {selectedPlayers.length > 0 && (
        <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 p-4 shadow-lg">
          <div className="flex items-center justify-between">
            <span className="text-sm font-medium">
              {selectedPlayers.length} player{selectedPlayers.length > 1 ? 's' : ''} selected
            </span>
            <div className="flex gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setSelectedPlayers([])}
              >
                Clear
              </Button>
              <Button
                size="sm"
                onClick={handleShuffle}
                disabled={selectedPlayers.length < 2}
              >
                Shuffle Teams
              </Button>
            </div>
          </div>
        </div>
      )}

      {/* Filters Bottom Sheet */}
      <MobileBottomSheet
        isOpen={showFilters}
        onClose={() => setShowFilters(false)}
        title="Filter Players"
      >
        <div className="p-6 space-y-6">
          <div>
            <h3 className="font-semibold mb-3">Position</h3>
            <div className="grid grid-cols-2 gap-3">
              {['ALL', 'DEF', 'MID', 'ATT'].map(pos => (
                <Button
                  key={pos}
                  variant={positionFilter === pos ? 'default' : 'outline'}
                  className="justify-start"
                  onClick={() => setPositionFilter(pos)}
                >
                  {pos}
                  {pos !== 'ALL' && (
                    <Badge variant="secondary" className="ml-2">
                      {positionCounts[pos]}
                    </Badge>
                  )}
                </Button>
              ))}
            </div>
          </div>

          <div className="pt-4">
            <Button
              className="w-full"
              onClick={() => setShowFilters(false)}
            >
              Apply Filters
            </Button>
          </div>
        </div>
      </MobileBottomSheet>
    </div>
  );
};

export default MobilePlayerRoster;