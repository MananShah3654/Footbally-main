import { useState, useEffect } from "react";
import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoginPage from "./components/LoginPage";
import HomePage from "./components/HomePage";
import PlayerCustomizationModal from "./components/PlayerCustomizationModal";

function App() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [customizingPlayer, setCustomizingPlayer] = useState(null);
  const [isCustomizationOpen, setIsCustomizationOpen] = useState(false);

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

  const handleCustomizePlayer = (player) => {
    setCustomizingPlayer(player);
    setIsCustomizationOpen(true);
  };

  const handleSavePlayer = (updatedPlayer) => {
    // In a real app, this would update the players list
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
    <div className="App">
      <BrowserRouter>
        {!user ? (
          <LoginPage onLogin={handleLogin} />
        ) : (
          <>
            <Routes>
              <Route 
                path="/" 
                element={
                  <HomePage 
                    user={user} 
                    onLogout={handleLogout}
                    onCustomizePlayer={handleCustomizePlayer}
                  />
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