import React, { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { toast } from 'sonner';
import { Copy, MessageCircle, Calendar, MapPin, Clock, DollarSign } from 'lucide-react';

const WhatsAppExport = ({ teams, isOpen, onClose }) => {
  // Helper to get upcoming Sunday in '31st August, 2025' format
  function getUpcomingSundayFormatted() {
    const today = new Date();
    const dayOfWeek = today.getDay();
    const daysUntilSunday = (7 - dayOfWeek) % 7 || 7;
    const sunday = new Date(today);
    sunday.setDate(today.getDate() + daysUntilSunday);
    const day = sunday.getDate();
    const month = sunday.toLocaleString('en-GB', { month: 'long' });
    const year = sunday.getFullYear();
    // Add ordinal suffix
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
  }

  const [gameDetails, setGameDetails] = useState({
    date: getUpcomingSundayFormatted(),
    time: '7:00 - 8:30 AM',
    venue: 'Savvy Swaraj',
    price: '220₹pp'
  });

  const generateWhatsAppMessage = () => {
    if (!teams) return '';

  const team1Players = teams.team1.players;
  const team2Players = teams.team2.players;
  // Subs from teams.subs if present
  const subs = teams.subs || [];

    let message = `*⚠ SUDAY MORNING ⚠*\n`;
    // Always use upcoming Sunday for WhatsApp export
    message += `${getUpcomingSundayFormatted()}\n`;
    message += `👉 Time:- ${gameDetails.time}\n`;
    message += `👉 Venue:- ${gameDetails.venue}\n`;
    message += `${gameDetails.price}\n\n`;

    // Team 1 (Dark)
    message += `Team 1: Black/Dark\n`;
    team1Players.forEach((player, index) => {
      const checkMark = player.isSubscribed ? '✅️' : '';
      message += ` ${index + 1}.⁠ ⁠${player.name}${checkMark}\n`;
    });

    message += `\nSub:\n`;
    if (subs.length > 0) {
      subs.forEach((sub, index) => {
        message += `${index + 1}. ${sub.name}\n`;
      });
    } else {
      message += `1. TBD\n`;
    }

    message += `\nTeam 2: White/Light\n`;
    team2Players.forEach((player, index) => {
      const checkMark = player.isSubscribed ? '✅️' : '';
      message += `${index + 1}. ${player.name}${checkMark}\n`;
    });

    message += `\nSub:\n`;
    message += `1. TBD\n\n`;

    message += `🔥Game On🔥`;

    return message;
  };

  const handleCopyToClipboard = async () => {
    const message = generateWhatsAppMessage();
    
    try {
      await navigator.clipboard.writeText(message);
      toast.success('Team list copied to clipboard!', {
        description: 'You can now paste it in WhatsApp'
      });
    } catch (error) {
      console.error('Failed to copy:', error);
      toast.error('Failed to copy to clipboard');
    }
  };

  const handleWhatsAppShare = () => {
    const message = generateWhatsAppMessage();
    const encodedMessage = encodeURIComponent(message);
    const whatsappUrl = `https://wa.me/?text=${encodedMessage}`;
    window.open(whatsappUrl, '_blank');
  };

  if (!teams) return null;

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold flex items-center gap-2">
            <MessageCircle className="w-6 h-6 text-green-600" />
            Export for WhatsApp
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-6">
          {/* Game Details Form */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold">Game Details</h3>
            
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="date" className="flex items-center gap-2">
                  <Calendar className="w-4 h-4" />
                  Date
                </Label>
                <Input
                  id="date"
                  value={gameDetails.date}
                  onChange={(e) => setGameDetails(prev => ({ ...prev, date: e.target.value }))}
                  placeholder="17th August, 2025"
                />
              </div>
              
              <div>
                <Label htmlFor="time" className="flex items-center gap-2">
                  <Clock className="w-4 h-4" />
                  Time
                </Label>
                <Input
                  id="time"
                  value={gameDetails.time}
                  onChange={(e) => setGameDetails(prev => ({ ...prev, time: e.target.value }))}
                  placeholder="7:00 - 8:30 AM"
                />
              </div>
              
              <div>
                <Label htmlFor="venue" className="flex items-center gap-2">
                  <MapPin className="w-4 h-4" />
                  Venue
                </Label>
                <Input
                  id="venue"
                  value={gameDetails.venue}
                  onChange={(e) => setGameDetails(prev => ({ ...prev, venue: e.target.value }))}
                  placeholder="Savvy Swaraj"
                />
              </div>
              
              <div>
                <Label htmlFor="price" className="flex items-center gap-2">
                  <DollarSign className="w-4 h-4" />
                  Price
                </Label>
                <Input
                  id="price"
                  value={gameDetails.price}
                  onChange={(e) => setGameDetails(prev => ({ ...prev, price: e.target.value }))}
                  placeholder="220₹pp"
                />
              </div>
            </div>
          </div>

          {/* Prominent Copy Button */}
          <div className="flex justify-end mb-2">
            <Button 
              onClick={handleCopyToClipboard} 
              className="bg-green-600 hover:bg-green-700 text-white flex items-center gap-2"
            >
              <Copy className="w-4 h-4" />
              Copy for WhatsApp
            </Button>
          </div>
          {/* Preview */}
          <div className="space-y-3">
            <h3 className="text-lg font-semibold">Preview</h3>
            <div className="text-sm text-gray-600 mb-1">
              Copy and paste the below message directly into WhatsApp.
            </div>
            <div className="bg-gray-50 p-4 rounded-lg border">
              <Textarea
                value={generateWhatsAppMessage()}
                readOnly
                className="min-h-[300px] font-mono text-sm bg-white"
              />
            </div>
          </div>

          {/* Action Buttons */}
          <div className="flex gap-3 pt-4 border-t">
            <Button onClick={onClose} variant="outline" className="flex-1">
              Close
            </Button>
            <Button 
              onClick={handleWhatsAppShare}
              className="bg-green-600 hover:bg-green-700 text-white flex items-center gap-2"
            >
              <MessageCircle className="w-4 h-4" />
              Share on WhatsApp
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default WhatsAppExport;