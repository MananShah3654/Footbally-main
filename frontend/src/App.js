import React, { useState, useEffect } from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Toaster } from './components/ui/sonner';
import { toast } from 'sonner';
// import { playersAPI, shuffleAPI } from './services/api';
import { mockPlayers, shuffleTeams } from './mock';
import PlayerRoster from './components/PlayerRoster';
import TeamDisplay from './components/TeamDisplay';
import PlayerProfileModal from './components/PlayerProfileModal';
import PlayerFormModal from './components/PlayerFormModal';
import WhatsAppExport from './components/WhatsAppExport';
import { Card } from './components/ui/card';
import { Button } from './components/ui/button';
import { Badge } from './components/ui/badge';
import { 
  Trophy, 
  Users, 
  Shuffle, 
  Target, 
  Clock,
  ArrowLeft,
  Plus,
  Upload,
  MessageCircle,
  Copy,
  MapPin
} from 'lucide-react';
import './App.css';

const Home = () => {
  // Copy WhatsApp message directly from main screen (Teams view)
  const handleCopyWhatsApp = async () => {
    if (!teams) return;
    const team1Players = teams.team1.players;
    const team2Players = teams.team2.players;
    let message = `⚠ SUNDAY MORNING ⚠\n`;
    message += `${new Date().toLocaleDateString('en-GB', { day: 'numeric', month: 'long', year: 'numeric' })}\n`;
    message += `👉 Time:- 7:00 - 8:30 AM\n`;
    message += `👉 Venue:- Savvy Swaraj\n`;
    message += `220₹pp\n\n`;
    message += `Team 1: Black/Dark\n`;
    team1Players.forEach((player, index) => {
      const checkMark = player.isSubscribed ? '✅️' : '';
      message += ` ${index + 1}.⁠ ⁠${player.name}${checkMark}\n`;
    });
    message += `\nSub:\n1. TBD\n`;
    message += `\nTeam 2: White/Light\n`;
    team2Players.forEach((player, index) => {
      const checkMark = player.isSubscribed ? '✅️' : '';
      message += `${index + 9}. ${player.name}${checkMark}\n`;
    });
    message += `\nSub:\n1. TBD\n\n`;
    message += `🔥Game On🔥`;
    try {
      await navigator.clipboard.writeText(message);
      toast.success('Team list copied to clipboard!', {
        description: 'You can now paste it in WhatsApp'
      });
    } catch (error) {
      toast.error('Failed to copy to clipboard');
    }
  };
  const [players, setPlayers] = useState([]);
  const [selectedPlayers, setSelectedPlayers] = useState([]); // IDs of selected players
  const [teams, setTeams] = useState(null);
  const [isShuffling, setIsShuffling] = useState(false);
  const [shuffleHistory, setShuffleHistory] = useState([]);
  const [loading, setLoading] = useState(true);
  
  // Modal states
  const [selectedPlayer, setSelectedPlayer] = useState(null);
  const [showPlayerProfile, setShowPlayerProfile] = useState(false);
  const [showPlayerForm, setShowPlayerForm] = useState(false);
  const [editingPlayer, setEditingPlayer] = useState(null);

  // Load players on mount

  // Toggle this flag to switch between API and mock data
  const USE_MOCK = true;

  useEffect(() => {
    if (USE_MOCK) {
      setPlayers(mockPlayers);
      setLoading(false);
    } else {
      loadPlayers();
    }
  }, []);

  // Uncomment to use API
  /*
  const loadPlayers = async () => {
    try {
      setLoading(true);
      const playersData = await playersAPI.getAll();
      setPlayers(playersData);
    } catch (error) {
      console.error('Error loading players:', error);
      toast.error('Failed to load players');
    } finally {
      setLoading(false);
    }
  };
  */

  const handleShuffle = async () => {
    if (selectedPlayers.length < 16) {
      toast.error(`Select at least 16 players to shuffle. Currently selected: ${selectedPlayers.length}`);
      return;
    }
    setIsShuffling(true);
    try {
      await new Promise(resolve => setTimeout(resolve, 1500));
      // Use only the first 16 for teams, rest as subs
      const selected = players.filter(p => selectedPlayers.includes(p.id));
      const mainPlayers = selected.slice(0, 16);
      const subs = selected.slice(16);
      const newTeams = shuffleTeams(mainPlayers);
      newTeams.subs = subs;
      setTeams(newTeams);
      setShuffleHistory(prev => [...prev, { 
        id: Date.now(), 
        teams: newTeams, 
        timestamp: new Date() 
      }].slice(-5));
      toast.success('Teams shuffled successfully!', {
        description: `Team 1: ${newTeams.team1.totalPoints} pts | Team 2: ${newTeams.team2.totalPoints} pts`
      });
    } catch (error) {
      console.error('Error shuffling teams:', error);
      toast.error(error.message || 'Failed to shuffle teams');
    } finally {
      setIsShuffling(false);
    }
  };

  // Toggle player selection
  const toggleSelectPlayer = (playerId) => {
    setSelectedPlayers(prev =>
      prev.includes(playerId)
        ? prev.filter(id => id !== playerId)
        : prev.length < 16 ? [...prev, playerId] : prev // max 16
    );
  };

  const handleBackToRoster = () => {
    setTeams(null);
  };

  const handlePlayerClick = (player) => {
    setSelectedPlayer(player);
    setShowPlayerProfile(true);
  };

  const handleAddPlayer = () => {
    setEditingPlayer(null);
    setShowPlayerForm(true);
  };

  const handleEditPlayer = (player) => {
    setEditingPlayer(player);
    setShowPlayerForm(true);
  };

  const handleDeletePlayer = (playerId) => {
    if (!window.confirm('Are you sure you want to delete this player?')) {
      return;
    }
    setPlayers(prev => prev.filter(p => p.id !== playerId));
    toast.success('Player deleted successfully');
    // Clear teams if we no longer have 16 players
    if (players.length - 1 < 16) {
      setTeams(null);
    }
  };

  const handleSavePlayer = async (playerData) => {
    try {
      if (editingPlayer) {
        // Edit existing player
        setPlayers(prev => prev.map(p => p.id === editingPlayer.id ? { ...p, ...playerData } : p));
        toast.success('Player updated successfully');
      } else {
        // Add new player (generate unique id)
        const newId = Math.max(0, ...players.map(p => Number(p.id) || 0)) + 1;
        setPlayers(prev => [...prev, { ...playerData, id: newId }]);
        toast.success('Player added successfully');
      }
    } catch (error) {
      console.error('Error saving player:', error);
      throw error;
    }
  };

  const handleImportPlayers = async () => {
    // This would typically open a file picker or show import modal
    // For now, we'll show a simple confirmation
    toast.info('Import feature would open file picker for JSON import');
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-indigo-50/40 flex items-center justify-center">
        <div className="text-center space-y-4">
          <div className="animate-spin rounded-full h-12 w-12 border-4 border-blue-600 border-t-transparent mx-auto"></div>
          <p className="text-muted-foreground">Loading players...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-indigo-50/40">
      {/* Header */}
      <header className="border-b bg-white/80 backdrop-blur-sm sticky top-0 z-10 shadow-sm">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-xl text-white shadow-lg">
                <Trophy className="w-6 h-6" />
              </div>
              <div>
                <h1 className="text-2xl font-bold bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                  Football Team Shuffler
                </h1>
                <p className="text-sm text-muted-foreground">
                  Create balanced teams for your Sunday matches • {players.length} players
                </p>
              </div>
            </div>

            <div className="flex items-center gap-2">
              {teams && (
                <Button 
                  variant="outline" 
                  onClick={handleBackToRoster}
                  className="flex items-center gap-2"
                >
                  <ArrowLeft className="w-4 h-4" />
                  Back to Roster
                </Button>
              )}
              
              {!teams && (
                <>
                  <Button
                    variant="outline"
                    onClick={handleImportPlayers}
                    className="flex items-center gap-2"
                  >
                    <Upload className="w-4 h-4" />
                    Import
                  </Button>
                  <Button
                    onClick={handleAddPlayer}
                    className="flex items-center gap-2 bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700"
                  >
                    <Plus className="w-4 h-4" />
                    Add Player
                  </Button>
                </>
              )}
            </div>
          </div>
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        {!teams ? (
          /* Player Roster View */
          <PlayerRoster 
            players={players}
            selectedPlayers={selectedPlayers}
            toggleSelectPlayer={toggleSelectPlayer}
            onShuffle={handleShuffle}
            isShuffling={isShuffling}
            onPlayerClick={handlePlayerClick}
            onEditPlayer={handleEditPlayer}
            onDeletePlayer={handleDeletePlayer}
          />
        ) : (
          /* Teams View */
          <div className="space-y-8">
            {/* Match Header */}
            <Card className="p-6 bg-gradient-to-r from-white to-gray-50/50 border-0 shadow-lg">
              <div className="text-center space-y-4">
                <div className="inline-flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-green-500 to-emerald-600 text-white rounded-full shadow-lg">
                  <Clock className="w-5 h-5" />
                  <span className="font-bold">Match Ready!</span>
                </div>
                
                <h2 className="text-3xl font-bold text-gray-800">
                  Sunday Football Match
                </h2>
                
                {/* Team comparison */}
                {/* Date & Venue Row */}
                <div className="flex flex-col md:flex-row items-center justify-center gap-4 mt-4 mb-2">
                  <div className="flex items-center gap-2 text-base text-gray-700 font-medium">
                    <Clock className="w-4 h-4 text-blue-600" />
                    {(() => {
                      // Get upcoming Sunday in '31st August, 2025' format
                      const today = new Date();
                      const dayOfWeek = today.getDay();
                      const daysUntilSunday = (7 - dayOfWeek) % 7 || 7;
                      const sunday = new Date(today);
                      sunday.setDate(today.getDate() + daysUntilSunday);
                      const day = sunday.getDate();
                      const month = sunday.toLocaleString('en-GB', { month: 'long' });
                      const year = sunday.getFullYear();
                      function ordinal(n) {
                        if (n > 3 && n < 21) return n + 'th';
                        switch (n % 10) {
                          case 1: return n + 'st';
                          case 2: return n + 'nd';
                          case 3: return n + 'rd';
                          default: return n + 'th';
                        }
                      }
                      return `${ordinal(day)} ${month}, ${year}`;
                    })()} (upcoming Sunday)
                  </div>
                  <div className="flex items-center gap-2 text-base text-gray-700 font-medium">
                    <MapPin className="w-4 h-4 text-green-600" />
                    Savvy Swaraj
                  </div>
                </div>
                <div className="flex items-center justify-center gap-8 mt-6">
                  <div className="text-center">
                    <div className="text-2xl font-bold text-blue-600">
                      Team A
                    </div>
                    <div className="text-sm text-muted-foreground">
                      {teams.team1.totalPoints} points
                    </div>
                  </div>
                  
                  <div className="text-4xl font-bold text-muted-foreground">
                    VS
                  </div>
                  
                  <div className="text-center">
                    <div className="text-2xl font-bold text-orange-600">
                      Team B
                    </div>
                    <div className="text-sm text-muted-foreground">
                      {teams.team2.totalPoints} points
                    </div>
                  </div>
                </div>

                {/* Point difference indicator */}
                <div className="flex items-center justify-center gap-2">
                  <Target className="w-4 h-4 text-muted-foreground" />
                  <span className="text-sm text-muted-foreground">
                    Point difference: {Math.abs(teams.team1.totalPoints - teams.team2.totalPoints)}
                  </span>
                  {Math.abs(teams.team1.totalPoints - teams.team2.totalPoints) <= 5 && (
                    <Badge variant="secondary" className="bg-green-100 text-green-700 border-0">
                      Well Balanced!
                    </Badge>
                  )}
                </div>

                <div className="flex flex-col md:flex-row justify-center gap-4 pt-4">
                  <Button 
                    onClick={handleShuffle} 
                    disabled={isShuffling}
                    variant="outline"
                    className="flex items-center gap-2"
                  >
                    <Shuffle className="w-4 h-4" />
                    Shuffle Again
                  </Button>
                  <Button 
                    onClick={handleCopyWhatsApp}
                    className="bg-green-600 hover:bg-green-700 text-white flex items-center gap-2"
                  >
                    <Copy className="w-4 h-4" />
                    Copy for WhatsApp
                  </Button>
                </div>
              </div>
            </Card>

            {/* Teams Display */}
            <div className="grid grid-cols-1 xl:grid-cols-2 gap-8">
              <TeamDisplay 
                team={teams.team1} 
                teamName="Team A" 
                teamColor={1}
                onPlayerClick={handlePlayerClick}
              />
              <TeamDisplay 
                team={teams.team2} 
                teamName="Team B" 
                teamColor={2}
                onPlayerClick={handlePlayerClick}
              />
            </div>

            {/* Match Statistics */}
            <Card className="p-6">
              <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
                <Target className="w-5 h-5" />
                Match Statistics
              </h3>
              
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="text-center space-y-2">
                  <div className="text-2xl font-bold text-blue-600">
                    {teams.team1.players.length}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    Players per team
                  </div>
                </div>
                
                <div className="text-center space-y-2">
                  <div className="text-2xl font-bold text-green-600">
                    {Math.abs(teams.team1.totalPoints - teams.team2.totalPoints)}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    Point difference
                  </div>
                </div>
                
                <div className="text-center space-y-2">
                  <div className="text-2xl font-bold text-purple-600">
                    {Math.round((teams.team1.totalPoints + teams.team2.totalPoints) / 2)}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    Average team rating
                  </div>
                </div>
              </div>
            </Card>
          </div>
        )}

        {/* Shuffle History */}
        {shuffleHistory.length > 0 && !teams && (
          <Card className="p-6 mt-8">
            <h3 className="text-lg font-semibold mb-4 flex items-center gap-2">
              <Clock className="w-5 h-5" />
              Recent Shuffles
            </h3>
            <div className="space-y-2">
              {shuffleHistory.slice(-3).reverse().map((shuffle, index) => (
                <div 
                  key={shuffle.id} 
                  className="flex items-center justify-between p-3 bg-gray-50 rounded-lg cursor-pointer hover:bg-gray-100 transition-colors"
                  onClick={() => setTeams(shuffle.teams)}
                >
                  <span className="text-sm">
                    {shuffle.timestamp.toLocaleTimeString()} - 
                    Team 1: {shuffle.teams.team1.totalPoints} pts | 
                    Team 2: {shuffle.teams.team2.totalPoints} pts
                  </span>
                  <Button variant="ghost" size="sm">
                    View
                  </Button>
                </div>
              ))}
            </div>
          </Card>
        )}
      </main>

      {/* Modals */}
      <PlayerProfileModal
        player={selectedPlayer}
        isOpen={showPlayerProfile}
        onClose={() => setShowPlayerProfile(false)}
      />
      
      <PlayerFormModal
        player={editingPlayer}
        isOpen={showPlayerForm}
        onClose={() => setShowPlayerForm(false)}
        onSave={handleSavePlayer}
      />

      <Toaster richColors position="top-right" />
    </div>
  );
};

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Home />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;