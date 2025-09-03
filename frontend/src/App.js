import { useState, useEffect } from "react";
import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import PlayerRoster from "./components/PlayerRoster";
import PlayerCustomizationModal from "./components/PlayerCustomizationModal";

function App() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [customizingPlayer, setCustomizingPlayer] = useState(null);
  const [isCustomizationOpen, setIsCustomizationOpen] = useState(false);
  const [selectedPlayer, setSelectedPlayer] = useState(null);

  useEffect(() => {
    // Check if user is already logged in
    const savedUser = localStorage.getItem('footbally_user');
    if (savedUser) {
      try {
        setUser(JSON.parse(savedUser));
      } catch (error) {
        console.error('Error parsing saved user:', error);
        localStorage.removeItem('footbally_user');
      }
    }
    setLoading(false);
  }, []);

  const handleLogin = (userData) => {
    setUser(userData);
  };

  const handleLogout = () => {
    localStorage.removeItem('footbally_user');
    setUser(null);
  };

  const handlePlayerClick = (player) => {
    setSelectedPlayer(player);
    // Could open profile modal here
  };

  const handleEditPlayer = (player) => {
    setCustomizingPlayer(player);
    setIsCustomizationOpen(true);
  };

  const handleDeletePlayer = (playerId) => {
    if (window.confirm('Are you sure you want to delete this player?')) {
      // TODO: Call API to delete player
      console.log('Delete player:', playerId);
    }
  };

  const handleSavePlayer = (updatedPlayer) => {
    console.log('Player saved:', updatedPlayer);
    setIsCustomizationOpen(false);
    setCustomizingPlayer(null);
  };

  const handleCloseCustomization = () => {
    setIsCustomizationOpen(false);
    setCustomizingPlayer(null);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-green-400 via-blue-500 to-purple-600 flex items-center justify-center">
        <div className="text-center text-white">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-white mx-auto mb-4"></div>
          <p>Loading Footbally...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="App min-h-screen bg-gray-50">
      <BrowserRouter>
        {!user ? (
          <LoginPage onLogin={handleLogin} />
        ) : (
          <>
            {/* Header */}
            <header className="bg-white shadow-sm border-b">
              <div className="container mx-auto px-4 py-4">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className="text-2xl">⚽</div>
                    <div>
                      <h1 className="text-xl font-bold text-gray-900">Footbally</h1>
                      <p className="text-sm text-gray-600">FIFA-Style Player Cards</p>
                    </div>
                  </div>
                  
                  <div className="flex items-center gap-3">
                    <img
                      src={user.avatar}
                      alt={user.name}
                      className="w-8 h-8 rounded-full border-2 border-gray-300"
                    />
                    <span className="font-medium hidden sm:block">{user.name}</span>
                    <button
                      onClick={handleLogout}
                      className="text-gray-600 hover:text-gray-900 text-sm font-medium px-3 py-1 rounded border hover:border-gray-300"
                    >
                      Logout
                    </button>
                  </div>
                </div>
              </div>
            </header>

            <Routes>
              <Route 
                path="/" 
                element={
                  <div className="container mx-auto p-4">
                    <PlayerRoster 
                      onPlayerClick={handlePlayerClick}
                      onEditPlayer={handleEditPlayer}
                      onDeletePlayer={handleDeletePlayer}
                    />
                  </div>
                } 
              />
            </Routes>
            
            <PlayerCustomizationModal
              player={customizingPlayer}
              isOpen={isCustomizationOpen}
              onClose={handleCloseCustomization}
              onSave={handleSavePlayer}
            />
          </>
        )}
      </BrowserRouter>
    </div>
  );
}

export default App;