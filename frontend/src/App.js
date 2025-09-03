import { useState, useEffect } from "react";
import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import MobileHeader from "./components/MobileHeader";
import MobilePlayerRoster from "./components/MobilePlayerRoster";
import MobilePlayerProfile from "./components/MobilePlayerProfile";
import PlayerFormModal from "./components/PlayerFormModal";

function App() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [editingPlayer, setEditingPlayer] = useState(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [viewingPlayer, setViewingPlayer] = useState(null);
  const [isProfileModalOpen, setIsProfileModalOpen] = useState(false);

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
    setViewingPlayer(player);
    setIsProfileModalOpen(true);
  };

  const handleEditPlayer = (player) => {
    setEditingPlayer(player);
    setIsEditModalOpen(true);
    // Close profile modal if open
    setIsProfileModalOpen(false);
  };

  const handleDeletePlayer = (playerId) => {
    if (window.confirm('Are you sure you want to delete this player?')) {
      // TODO: Call API to delete player
      console.log('Delete player:', playerId);
    }
  };

  const handleSavePlayer = async (playerData) => {
    try {
      // TODO: Call API to save player
      console.log('Player saved:', playerData);
      setIsEditModalOpen(false);
      setEditingPlayer(null);
      // Refresh the player list
      window.location.reload();
    } catch (error) {
      console.error('Error saving player:', error);
    }
  };

  const handleCloseEditModal = () => {
    setIsEditModalOpen(false);
    setEditingPlayer(null);
  };

  const handleCloseProfileModal = () => {
    setIsProfileModalOpen(false);
    setViewingPlayer(null);
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
            {/* Mobile Header */}
            <MobileHeader 
              user={user} 
              onLogout={handleLogout}
              title="Squad"
              showSearch={true}
            />

            <Routes>
              <Route 
                path="/" 
                element={
                  <MobilePlayerRoster 
                    onPlayerClick={handlePlayerClick}
                    onEditPlayer={handleEditPlayer}
                    onDeletePlayer={handleDeletePlayer}
                  />
                } 
              />
            </Routes>
            
            {/* Mobile Player Profile */}
            <MobilePlayerProfile
              player={viewingPlayer}
              isOpen={isProfileModalOpen}
              onClose={handleCloseProfileModal}
              onEdit={handleEditPlayer}
            />

            {/* Edit Modal (desktop style for complex forms) */}
            <PlayerFormModal
              player={editingPlayer}
              isOpen={isEditModalOpen}
              onClose={handleCloseEditModal}
              onSave={handleSavePlayer}
            />
          </>
        )}
      </BrowserRouter>
    </div>
  );
}

export default App;