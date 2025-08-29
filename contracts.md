# Football Team Shuffler - Backend Integration Contracts

## API Endpoints

### Players Management
- `GET /api/players` - Get all players
- `POST /api/players` - Create new player
- `PUT /api/players/{id}` - Update player
- `DELETE /api/players/{id}` - Delete player
- `GET /api/players/{id}` - Get single player

### Team Shuffling
- `POST /api/shuffle` - Generate shuffled teams with given player IDs

## Data Models

### Player Model
```python
{
  "id": "string",
  "name": "string",
  "position": "DEF|MID|ATT", 
  "points": "number",
  "photo": "string (URL)",
  "skills": {
    "pace": "number",
    "shooting": "number", 
    "passing": "number",
    "defending": "number",
    "dribbling": "number",
    "physical": "number"
  },
  "age": "number",
  "preferredFoot": "Left|Right",
  "nationality": "string",
  "created_at": "datetime",
  "updated_at": "datetime"
}
```

### Team Shuffle Response
```python
{
  "team1": {
    "players": [Player],
    "totalPoints": "number", 
    "formation": "string"
  },
  "team2": {
    "players": [Player],
    "totalPoints": "number",
    "formation": "string" 
  }
}
```

## Frontend Changes Required

### Replace Mock Data Usage
- Remove `mockPlayers` import from App.js
- Replace `shuffleTeams` function with API call to `/api/shuffle`
- Add API calls for CRUD operations in player management

### New Features to Add
1. **Player Management Interface**
   - Add/Edit Player form modal
   - Delete confirmation dialog
   - Player import from JSON functionality

2. **Player Profile Popup**
   - Modal showing detailed player info when name is clicked in teams view
   - Enhanced player card with more statistics

3. **API Integration**
   - Axios service for API calls
   - Loading states for all operations
   - Error handling with toast notifications

## Backend Implementation

### MongoDB Collections
- `players` - Store all player data

### Business Logic
1. **Team Shuffle Algorithm** - Same as frontend but server-side
2. **Player Validation** - Ensure required fields and valid position
3. **Photo URL Validation** - Check if image URLs are accessible

## File Changes

### Frontend Files to Update:
- `src/App.js` - Replace mock data with API calls
- `src/components/PlayerRoster.jsx` - Add CRUD operations
- `src/components/TeamDisplay.jsx` - Add player click handlers
- Create `src/services/api.js` - API service layer
- Create `src/components/PlayerProfileModal.jsx` - Player popup
- Create `src/components/PlayerFormModal.jsx` - Add/Edit player form

### Backend Files to Create:
- `models/player.py` - Player data model
- `routes/players.py` - Player CRUD endpoints  
- `routes/shuffle.py` - Team shuffle endpoint
- `services/shuffle_service.py` - Shuffle algorithm
- Update `server.py` - Include new routes

## Integration Strategy
1. Create backend models and endpoints
2. Test backend with sample data
3. Replace frontend mock usage with API calls
4. Add player management UI
5. Add player profile popup feature
6. Test full application flow