import { useState, useEffect } from "react";
import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import MobileHeader from "./components/MobileHeader";
import MobilePlayerRoster from "./components/MobilePlayerRoster";
import PlayerRoster from "./components/PlayerRoster";
import MobilePlayerProfile from "./components/MobilePlayerProfile";
import PlayerFormModal from "./components/PlayerFormModal";

function App() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [editingPlayer, setEditingPlayer] = useState(null);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [viewingPlayer, setViewingPlayer] = useState(null);
  const [isProfileModalOpen, setIsProfileModalOpen] = useState(false);
  const [currentView, setCurrentView] = useState('mobile'); // 'mobile', 'roster', 'list'

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

  const handleViewChange = (view) => {
    setCurrentView(view);
  };

  const getViewTitle = () => {
    switch (currentView) {
      case 'mobile': return 'Squad';
      case 'roster': return 'Player Roster';
      case 'list': return 'Player List';
      default: return 'Footbally';
    }
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
            {/* Mobile Header with Navigation */}
            <MobileHeader 
              user={user} 
              onLogout={handleLogout}
              title={getViewTitle()}
              showSearch={true}
              currentView={currentView}
              onViewChange={handleViewChange}
            />

            <Routes>
              <Route 
                path="/" 
                element={
                  <div className={currentView === 'roster' ? 'container mx-auto p-4' : ''}>
                    {/* Mobile View */}
                    {currentView === 'mobile' && (
                      <MobilePlayerRoster 
                        onPlayerClick={handlePlayerClick}
                        onEditPlayer={handleEditPlayer}
                        onDeletePlayer={handleDeletePlayer}
                      />
                    )}
                    
                    {/* Full Roster View */}
                    {currentView === 'roster' && (
                      <PlayerRoster 
                        onPlayerClick={handlePlayerClick}
                        onEditPlayer={handleEditPlayer}
                        onDeletePlayer={handleDeletePlayer}
                      />
                    )}
                    
                    {/* List View */}
                    {currentView === 'list' && (
                      <div className="p-4">
                        <div className="bg-white rounded-lg shadow">
                          <div className="p-6">
                            <h2 className="text-xl font-bold mb-4">Player List View</h2>
                            <div className="space-y-4">
                              {/* Simple list view - could be implemented later */}
                              <div className="text-center py-8 text-gray-500">
                                <p>List view coming soon...</p>
                                <p className="text-sm mt-2">Use Mobile View or Full Roster for now</p>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    )}
                  </div>
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