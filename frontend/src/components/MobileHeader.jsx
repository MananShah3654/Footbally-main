import React from 'react';
import { Search, Menu, Bell, User } from 'lucide-react';
import { Button } from './ui/button';

const MobileHeader = ({ user, onLogout, title = "Footbally", showSearch = false, onSearchToggle = null }) => {
  return (
    <div className="sticky top-0 z-50 bg-white border-b border-gray-200 shadow-sm">
      {/* Status bar spacer for mobile */}
      <div className="h-safe-top bg-white"></div>
      
      {/* Main header */}
      <div className="flex items-center justify-between px-4 py-3">
        {/* Left side */}
        <div className="flex items-center gap-3">
          <div className="text-2xl">⚽</div>
          <div>
            <h1 className="text-lg font-bold text-gray-900">{title}</h1>
            <p className="text-xs text-gray-500">FIFA-Style Cards</p>
          </div>
        </div>

        {/* Right side */}
        <div className="flex items-center gap-2">
          {showSearch && (
            <Button
              variant="ghost"
              size="sm"
              className="p-2 h-auto"
              onClick={onSearchToggle}
            >
              <Search className="w-5 h-5 text-gray-600" />
            </Button>
          )}
          
          <Button
            variant="ghost"
            size="sm" 
            className="p-2 h-auto"
          >
            <Bell className="w-5 h-5 text-gray-600" />
          </Button>

          {/* User avatar */}
          <div className="relative">
            <button
              onClick={onLogout}
              className="flex items-center gap-2 p-1 rounded-full hover:bg-gray-100 transition-colors"
            >
              <img
                src={user.avatar}
                alt={user.name}
                className="w-8 h-8 rounded-full border-2 border-gray-200"
              />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default MobileHeader;