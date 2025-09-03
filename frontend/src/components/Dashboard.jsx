import React, { useState, useEffect } from 'react';
import axios from 'axios';
import PlayerCard from './PlayerCard';
import StatsCard from './StatsCard';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API = `${BACKEND_URL}/api`;

const Dashboard = () => {
  const [players, setPlayers] = useState([]);
  const [teams, setTeams] = useState([]);
  const [statistics, setStatistics] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedPlayer, setSelectedPlayer] = useState(null);
  const [playerStats, setPlayerStats] = useState([]);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [playersRes, teamsRes, statsRes] = await Promise.all([
        axios.get(`${API}/players`),
        axios.get(`${API}/teams`),
        axios.get(`${API}/statistics`)
      ]);
      
      setPlayers(playersRes.data);
      setTeams(teamsRes.data);
      setStatistics(statsRes.data);
    } catch (err) {
      setError('Failed to fetch data');
      console.error('Error fetching data:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleViewPlayerDetails = (player) => {
    setSelectedPlayer(player);
  };

  const handleViewPlayerStats = async (player) => {
    try {
      const response = await axios.get(`${API}/players/${player.id}/statistics`);
      setPlayerStats(response.data);
      setSelectedPlayer(player);
    } catch (err) {
      console.error('Error fetching player statistics:', err);
    }
  };

  const calculateOverallStats = () => {
    const totalPlayers = players.length;
    const totalTeams = teams.length;
    const totalGoals = statistics.reduce((sum, stat) => sum + stat.goals, 0);
    const totalGames = statistics.reduce((sum, stat) => sum + stat.gamesPlayed, 0);
    const avgRating = statistics.length > 0 
      ? (statistics.reduce((sum, stat) => sum + stat.averageRating, 0) / statistics.length).toFixed(1)
      : 0;

    return { totalPlayers, totalTeams, totalGoals, totalGames, avgRating };
  };

  const getTopScorers = () => {
    return statistics
      .sort((a, b) => b.goals - a.goals)
      .slice(0, 5)
      .map(stat => ({
        ...stat,
        player: players.find(p => p.id === stat.playerId)
      }))
      .filter(item => item.player);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-4 text-gray-600">Loading football data...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="text-center">
          <p className="text-red-600 text-lg">{error}</p>
          <Button onClick={fetchData} className="mt-4">Try Again</Button>
        </div>
      </div>
    );
  }

  const overallStats = calculateOverallStats();
  const topScorers = getTopScorers();

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">⚽ Footbally</h1>
          <p className="text-gray-600">Professional Football Player Statistics Platform</p>
        </div>

        {/* Overall Statistics */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6 mb-8">
          <StatsCard
            title="Total Players"
            value={overallStats.totalPlayers}
            icon="👥"
            subtitle="Active players"
          />
          <StatsCard
            title="Teams"
            value={overallStats.totalTeams}
            icon="🏆"
            subtitle="Registered teams"
          />
          <StatsCard
            title="Total Goals"
            value={overallStats.totalGoals}
            icon="⚽"
            subtitle="This season"
          />
          <StatsCard
            title="Total Games"
            value={overallStats.totalGames}
            icon="🏟️"
            subtitle="Matches played"
          />
          <StatsCard
            title="Avg Rating"
            value={overallStats.avgRating}
            icon="⭐"
            subtitle="Player performance"
          />
        </div>

        {/* Top Scorers */}
        <Card className="mb-8">
          <CardHeader>
            <CardTitle>🏅 Top Scorers</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {topScorers.map((item, index) => (
                <div key={item.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                  <div className="flex items-center gap-3">
                    <span className={`w-8 h-8 rounded-full flex items-center justify-center text-white font-bold ${
                      index === 0 ? 'bg-yellow-500' : index === 1 ? 'bg-gray-400' : index === 2 ? 'bg-orange-600' : 'bg-blue-500'
                    }`}>
                      {index + 1}
                    </span>
                    <div>
                      <h4 className="font-semibold">{item.player.name}</h4>
                      <p className="text-sm text-gray-600">{item.player.position}</p>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="text-2xl font-bold text-green-600">{item.goals}</div>
                    <div className="text-xs text-gray-500">{item.gamesPlayed} games</div>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Players Grid */}
        <div className="mb-8">
          <h2 className="text-2xl font-bold mb-4">👨‍💼 All Players</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            {players.map(player => (
              <PlayerCard
                key={player.id}
                player={player}
                onViewDetails={handleViewPlayerDetails}
                onViewStats={handleViewPlayerStats}
              />
            ))}
          </div>
        </div>

        {/* Player Details Modal/Section */}
        {selectedPlayer && (
          <Card className="mb-8">
            <CardHeader>
              <CardTitle>📊 {selectedPlayer.name} - Detailed Statistics</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Player Info */}
                <div>
                  <h3 className="text-lg font-semibold mb-4">Player Information</h3>
                  <div className="space-y-2">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Position:</span>
                      <span className="font-medium">{selectedPlayer.position}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Age:</span>
                      <span className="font-medium">{selectedPlayer.age} years</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Nationality:</span>
                      <span className="font-medium">{selectedPlayer.nationality}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Height:</span>
                      <span className="font-medium">{selectedPlayer.height}cm</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Weight:</span>
                      <span className="font-medium">{selectedPlayer.weight}kg</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Preferred Foot:</span>
                      <span className="font-medium">{selectedPlayer.preferredFoot}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Jersey Number:</span>
                      <span className="font-medium">#{selectedPlayer.jerseyNumber}</span>
                    </div>
                  </div>
                </div>

                {/* Statistics */}
                <div>
                  <h3 className="text-lg font-semibold mb-4">Season Statistics</h3>
                  {playerStats.length > 0 ? (
                    <div className="space-y-4">
                      {playerStats.map(stat => (
                        <div key={stat.id} className="border rounded-lg p-4">
                          <h4 className="font-medium mb-3">Season {stat.season}</h4>
                          <div className="grid grid-cols-2 gap-4 text-sm">
                            <div className="space-y-2">
                              <div className="flex justify-between">
                                <span>Games Played:</span>
                                <span className="font-medium">{stat.gamesPlayed}</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Goals:</span>
                                <span className="font-medium text-green-600">{stat.goals}</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Assists:</span>
                                <span className="font-medium text-blue-600">{stat.assists}</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Minutes:</span>
                                <span className="font-medium">{stat.minutesPlayed}</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Rating:</span>
                                <span className="font-medium text-yellow-600">{stat.averageRating}⭐</span>
                              </div>
                            </div>
                            <div className="space-y-2">
                              <div className="flex justify-between">
                                <span>Tackles:</span>
                                <span className="font-medium">{stat.tackles}</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Pass Accuracy:</span>
                                <span className="font-medium">{stat.passAccuracy.toFixed(1)}%</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Shot Accuracy:</span>
                                <span className="font-medium">{stat.shotAccuracy.toFixed(1)}%</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Win Rate:</span>
                                <span className="font-medium text-green-600">{stat.winPercentage.toFixed(1)}%</span>
                              </div>
                              <div className="flex justify-between">
                                <span>Cards:</span>
                                <span className="font-medium">
                                  <span className="text-yellow-500">Y:{stat.yellowCards}</span>
                                  {' '}
                                  <span className="text-red-500">R:{stat.redCards}</span>
                                </span>
                              </div>
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="text-gray-500">No statistics available for this player.</p>
                  )}
                </div>
              </div>
              
              <div className="mt-6">
                <Button variant="outline" onClick={() => setSelectedPlayer(null)}>
                  Close Details
                </Button>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Teams Section */}
        <div className="mb-8">
          <h2 className="text-2xl font-bold mb-4">🏆 Teams</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {teams.map(team => (
              <Card key={team.id} className="hover:shadow-lg transition-shadow">
                <CardHeader>
                  <CardTitle className="flex items-center justify-between">
                    <span>{team.name}</span>
                    <span className="text-sm font-normal text-gray-500">{team.shortName}</span>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">League:</span>
                      <span className="font-medium">{team.league}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">City:</span>
                      <span className="font-medium">{team.city}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Stadium:</span>
                      <span className="font-medium">{team.stadium}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Manager:</span>
                      <span className="font-medium">{team.manager}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Founded:</span>
                      <span className="font-medium">{team.founded}</span>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;