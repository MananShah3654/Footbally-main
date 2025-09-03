import React, { useState, useEffect } from 'react';
import axios from 'axios';
import PlayerCard from './PlayerCard';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const HomePage = ({ user, onLogout, onCustomizePlayer }) => {
  const [players, setPlayers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [currentPlayerIndex, setCurrentPlayerIndex] = useState(0);
  const [isShuffling, setIsShuffling] = useState(false);
  const [viewMode, setViewMode] = useState('shuffle'); // 'shuffle', 'all', 'profile'
  const [stats, setStats] = useState({
    totalPlayers: 0,
    avgRating: 0,
    positions: { DEF: 0, MID: 0, ATT: 0 }
  });

  useEffect(() => {
    fetchPlayers();
  }, []);

  const fetchPlayers = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${API}/players`);
      const playersData = response.data;
      setPlayers(playersData);
      
      // Calculate stats
      const totalPlayers = playersData.length;
      const avgRating = playersData.reduce((sum, p) => sum + (p.averageRating || 85), 0) / totalPlayers;
      const positions = playersData.reduce((acc, p) => {
        const pos = p.position || 'MID';
        if (pos.includes('DEF') || pos.includes('Defender')) acc.DEF++;
        else if (pos.includes('ATT') || pos.includes('Forward') || pos.includes('Striker')) acc.ATT++;
        else acc.MID++;
        return acc;
      }, { DEF: 0, MID: 0, ATT: 0 });
      
      setStats({ totalPlayers, avgRating: avgRating.toFixed(1), positions });
    } catch (err) {
      console.error('Error fetching players:', err);
    } finally {
      setLoading(false);
    }
  };

  const shufflePlayer = () => {
    if (players.length === 0) return;
    
    setIsShuffling(true);
    
    // Animate shuffle
    const shuffleCount = 10;
    let count = 0;
    const shuffleInterval = setInterval(() => {
      setCurrentPlayerIndex(Math.floor(Math.random() * players.length));
      count++;
      
      if (count >= shuffleCount) {
        clearInterval(shuffleInterval);
        setIsShuffling(false);
      }
    }, 150);
  };

  const handleCustomizePlayer = (player) => {
    // TODO: Open customization modal
    console.log('Customize player:', player);
  };

  const handleViewPlayerStats = (player) => {
    // TODO: Open detailed stats view
    console.log('View stats for:', player);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-green-400 via-blue-500 to-purple-600 flex items-center justify-center">
        <div className="text-center text-white">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-white mx-auto mb-4"></div>
          <p>Loading your football cards...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-green-400 via-blue-500 to-purple-600">
      {/* Header */}
      <header className="bg-black bg-opacity-20 backdrop-blur-sm border-b border-white border-opacity-20">
        <div className="container mx-auto px-4 py-3">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="text-3xl">⚽</div>
              <div>
                <h1 className="text-xl font-bold text-white">Footbally</h1>
                <p className="text-sm text-white text-opacity-80">Player Card Manager</p>
              </div>
            </div>
            
            <div className="flex items-center gap-4">
              <div className="flex gap-2">
                <Button
                  variant={viewMode === 'shuffle' ? 'default' : 'outline'}
                  size="sm"
                  onClick={() => setViewMode('shuffle')}
                  className="text-xs"
                >
                  🔀 Shuffle
                </Button>
                <Button
                  variant={viewMode === 'all' ? 'default' : 'outline'}
                  size="sm"
                  onClick={() => setViewMode('all')}
                  className="text-xs"
                >
                  🎴 All Cards
                </Button>
                <Button
                  variant={viewMode === 'profile' ? 'default' : 'outline'}
                  size="sm"
                  onClick={() => setViewMode('profile')}
                  className="text-xs"
                >
                  👤 Profile
                </Button>
              </div>
              
              <div className="flex items-center gap-3">
                <img
                  src={user.avatar}
                  alt={user.name}
                  className="w-8 h-8 rounded-full border-2 border-white"
                />
                <span className="text-white font-medium hidden sm:block">{user.name}</span>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={onLogout}
                  className="text-white border-white hover:bg-white hover:text-black"
                >
                  Logout
                </Button>
              </div>
            </div>
          </div>
        </div>
      </header>

      <div className="container mx-auto px-4 py-8">
        {/* Stats Overview */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
          <Card className="bg-white bg-opacity-90 backdrop-blur-sm">
            <CardContent className="p-4 text-center">
              <div className="text-2xl font-bold text-blue-600">{stats.totalPlayers}</div>
              <div className="text-sm text-gray-600">Total Players</div>
            </CardContent>
          </Card>
          
          <Card className="bg-white bg-opacity-90 backdrop-blur-sm">
            <CardContent className="p-4 text-center">
              <div className="text-2xl font-bold text-green-600">{stats.avgRating}</div>
              <div className="text-sm text-gray-600">Avg Rating</div>
            </CardContent>
          </Card>
          
          <Card className="bg-white bg-opacity-90 backdrop-blur-sm">
            <CardContent className="p-4">
              <div className="flex justify-between items-center">
                <div>
                  <div className="text-lg font-bold">Positions</div>
                  <div className="text-xs text-gray-600">Distribution</div>
                </div>
                <div className="text-right text-sm">
                  <div><Badge className="bg-blue-500 text-white text-xs mr-1">DEF</Badge>{stats.positions.DEF}</div>
                  <div><Badge className="bg-green-500 text-white text-xs mr-1">MID</Badge>{stats.positions.MID}</div>
                  <div><Badge className="bg-red-500 text-white text-xs mr-1">ATT</Badge>{stats.positions.ATT}</div>
                </div>
              </div>
            </CardContent>
          </Card>
          
          <Card className="bg-white bg-opacity-90 backdrop-blur-sm">
            <CardContent className="p-4 text-center">
              <div className="text-2xl">🏆</div>
              <div className="text-sm font-medium">FIFA Style</div>
              <div className="text-xs text-gray-600">Card Collection</div>
            </CardContent>
          </Card>
        </div>

        {/* Main Content Area */}
        {viewMode === 'shuffle' && (
          <div className="text-center">
            <Card className="bg-white bg-opacity-90 backdrop-blur-sm p-6 mb-8">
              <CardHeader>
                <CardTitle className="flex items-center justify-center gap-2">
                  <span className="text-2xl">🔀</span>
                  Player Card Shuffler
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 mb-6">
                  Discover players in your collection! Click shuffle to get a random player card.
                </p>
                
                <Button
                  onClick={shufflePlayer}
                  disabled={isShuffling}
                  className="bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white px-8 py-3 rounded-lg font-semibold text-lg mb-8"
                >
                  {isShuffling ? (
                    <div className="flex items-center gap-2">
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
                      Shuffling...
                    </div>
                  ) : (
                    '🔀 Shuffle Player'
                  )}
                </Button>
              </CardContent>
            </Card>

            {/* Current Player Card */}
            {players.length > 0 && (
              <div className="flex justify-center">
                <PlayerCard
                  player={players[currentPlayerIndex]}
                  onCustomize={handleCustomizePlayer}
                  onEdit={handleViewPlayerStats}
                />
              </div>
            )}
          </div>
        )}

        {viewMode === 'all' && (
          <div>
            <Card className="bg-white bg-opacity-90 backdrop-blur-sm p-6 mb-8">
              <CardHeader>
                <CardTitle className="flex items-center justify-center gap-2">
                  <span className="text-2xl">🎴</span>
                  All Player Cards
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 text-center">
                  Browse your complete player collection. Click on any card to flip and see detailed stats.
                </p>
              </CardContent>
            </Card>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8 justify-items-center">
              {players.map((player, index) => (
                <PlayerCard
                  key={player.id || index}
                  player={player}
                  onCustomize={handleCustomizePlayer}
                  onEdit={handleViewPlayerStats}
                />
              ))}
            </div>
          </div>
        )}

        {viewMode === 'profile' && (
          <div className="max-w-2xl mx-auto">
            <Card className="bg-white bg-opacity-90 backdrop-blur-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-3">
                  <img
                    src={user.avatar}
                    alt={user.name}
                    className="w-12 h-12 rounded-full border-2 border-gray-300"
                  />
                  <div>
                    <div className="text-xl">{user.name}</div>
                    <div className="text-sm text-gray-600">{user.email}</div>
                  </div>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-6">
                <div className="grid grid-cols-2 gap-4">
                  <div className="text-center p-4 bg-gray-50 rounded-lg">
                    <div className="text-2xl font-bold text-blue-600">{stats.totalPlayers}</div>
                    <div className="text-sm text-gray-600">Cards Collected</div>
                  </div>
                  <div className="text-center p-4 bg-gray-50 rounded-lg">
                    <div className="text-2xl font-bold text-green-600">{stats.avgRating}</div>
                    <div className="text-sm text-gray-600">Collection Rating</div>
                  </div>
                </div>
                
                <div className="space-y-4">
                  <h3 className="font-semibold text-lg">Collection Stats</h3>
                  <div className="space-y-2">
                    <div className="flex justify-between items-center">
                      <span className="flex items-center gap-2">
                        <Badge className="bg-blue-500 text-white">DEF</Badge>
                        Defenders
                      </span>
                      <span className="font-medium">{stats.positions.DEF}</span>
                    </div>
                    <div className="flex justify-between items-center">
                      <span className="flex items-center gap-2">
                        <Badge className="bg-green-500 text-white">MID</Badge>
                        Midfielders
                      </span>
                      <span className="font-medium">{stats.positions.MID}</span>
                    </div>
                    <div className="flex justify-between items-center">
                      <span className="flex items-center gap-2">
                        <Badge className="bg-red-500 text-white">ATT</Badge>
                        Attackers
                      </span>
                      <span className="font-medium">{stats.positions.ATT}</span>
                    </div>
                  </div>
                </div>
                
                <div className="pt-4 border-t">
                  <p className="text-sm text-gray-600">
                    Member since: {new Date(user.joinDate).toLocaleDateString()}
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>
        )}
      </div>
    </div>
  );
};

export default HomePage;