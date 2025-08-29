#!/usr/bin/env python3
"""
Comprehensive Backend API Tests for Football Team Shuffler
Tests all CRUD operations, shuffle functionality, and error handling
"""

import requests
import json
import sys
import os
from typing import Dict, List, Any

# Get backend URL from frontend .env file
def get_backend_url():
    try:
        with open('/app/frontend/.env', 'r') as f:
            for line in f:
                if line.startswith('REACT_APP_BACKEND_URL='):
                    return line.split('=', 1)[1].strip()
    except FileNotFoundError:
        pass
    return "http://localhost:8001"

BASE_URL = get_backend_url()
API_URL = f"{BASE_URL}/api"

class FootballAPITester:
    def __init__(self):
        self.base_url = API_URL
        self.test_results = []
        self.created_player_ids = []
        
    def log_test(self, test_name: str, success: bool, message: str = "", details: Any = None):
        """Log test results"""
        status = "✅ PASS" if success else "❌ FAIL"
        result = {
            "test": test_name,
            "status": status,
            "message": message,
            "details": details
        }
        self.test_results.append(result)
        print(f"{status}: {test_name}")
        if message:
            print(f"   {message}")
        if details and not success:
            print(f"   Details: {details}")
        print()

    def test_health_check(self):
        """Test basic API health"""
        try:
            response = requests.get(f"{self.base_url}/health", timeout=10)
            if response.status_code == 200:
                data = response.json()
                if data.get("status") == "healthy":
                    self.log_test("Health Check", True, "API and database are healthy")
                else:
                    self.log_test("Health Check", False, f"Unhealthy status: {data}")
            else:
                self.log_test("Health Check", False, f"HTTP {response.status_code}: {response.text}")
        except Exception as e:
            self.log_test("Health Check", False, f"Connection error: {str(e)}")

    def test_get_all_players(self):
        """Test GET /api/players/ - should return players list"""
        try:
            response = requests.get(f"{self.base_url}/players/", timeout=10)
            if response.status_code == 200:
                players = response.json()
                if isinstance(players, list):
                    self.log_test("GET All Players", True, f"Retrieved {len(players)} players")
                    return players
                else:
                    self.log_test("GET All Players", False, "Response is not a list")
                    return []
            else:
                self.log_test("GET All Players", False, f"HTTP {response.status_code}: {response.text}")
                return []
        except Exception as e:
            self.log_test("GET All Players", False, f"Request error: {str(e)}")
            return []

    def test_get_single_player(self, player_id: str):
        """Test GET /api/players/{id}/ for a specific player"""
        try:
            response = requests.get(f"{self.base_url}/players/{player_id}", timeout=10)
            if response.status_code == 200:
                player = response.json()
                if player.get("id") == player_id:
                    self.log_test("GET Single Player", True, f"Retrieved player: {player.get('name')}")
                    return player
                else:
                    self.log_test("GET Single Player", False, "Player ID mismatch in response")
            elif response.status_code == 404:
                self.log_test("GET Single Player", False, "Player not found (404)")
            else:
                self.log_test("GET Single Player", False, f"HTTP {response.status_code}: {response.text}")
        except Exception as e:
            self.log_test("GET Single Player", False, f"Request error: {str(e)}")
        return None

    def test_create_player(self):
        """Test POST /api/players/ to create a new player"""
        test_player = {
            "name": "Lionel Messi",
            "position": "ATT",
            "points": 95,
            "photo": "https://example.com/messi.jpg",
            "skills": {
                "pace": 85,
                "shooting": 95,
                "passing": 90,
                "defending": 35,
                "dribbling": 99,
                "physical": 75
            },
            "age": 36,
            "preferredFoot": "Left",
            "nationality": "Argentina"
        }
        
        try:
            response = requests.post(
                f"{self.base_url}/players/", 
                json=test_player,
                timeout=10
            )
            if response.status_code == 200:
                created_player = response.json()
                if created_player.get("name") == test_player["name"]:
                    self.created_player_ids.append(created_player["id"])
                    self.log_test("POST Create Player", True, f"Created player: {created_player['name']}")
                    return created_player
                else:
                    self.log_test("POST Create Player", False, "Created player data mismatch")
            else:
                self.log_test("POST Create Player", False, f"HTTP {response.status_code}: {response.text}")
        except Exception as e:
            self.log_test("POST Create Player", False, f"Request error: {str(e)}")
        return None

    def test_update_player(self, player_id: str):
        """Test PUT /api/players/{id}/ to update a player"""
        update_data = {
            "points": 96,
            "age": 37
        }
        
        try:
            response = requests.put(
                f"{self.base_url}/players/{player_id}",
                json=update_data,
                timeout=10
            )
            if response.status_code == 200:
                updated_player = response.json()
                if updated_player.get("points") == 96:
                    self.log_test("PUT Update Player", True, f"Updated player points to {updated_player['points']}")
                    return updated_player
                else:
                    self.log_test("PUT Update Player", False, "Update data not reflected")
            elif response.status_code == 404:
                self.log_test("PUT Update Player", False, "Player not found (404)")
            else:
                self.log_test("PUT Update Player", False, f"HTTP {response.status_code}: {response.text}")
        except Exception as e:
            self.log_test("PUT Update Player", False, f"Request error: {str(e)}")
        return None

    def test_delete_player(self, player_id: str):
        """Test DELETE /api/players/{id}/ to delete a player"""
        try:
            response = requests.delete(f"{self.base_url}/players/{player_id}", timeout=10)
            if response.status_code == 200:
                result = response.json()
                if "deleted successfully" in result.get("message", "").lower():
                    self.log_test("DELETE Player", True, "Player deleted successfully")
                    return True
                else:
                    self.log_test("DELETE Player", False, f"Unexpected response: {result}")
            elif response.status_code == 404:
                self.log_test("DELETE Player", False, "Player not found (404)")
            else:
                self.log_test("DELETE Player", False, f"HTTP {response.status_code}: {response.text}")
        except Exception as e:
            self.log_test("DELETE Player", False, f"Request error: {str(e)}")
        return False

    def test_shuffle_teams(self):
        """Test POST /api/shuffle/ - should return balanced teams"""
        try:
            response = requests.post(f"{self.base_url}/shuffle", timeout=10)
            if response.status_code == 200:
                result = response.json()
                
                # Validate response structure
                if "team1" in result and "team2" in result:
                    team1 = result["team1"]
                    team2 = result["team2"]
                    
                    # Check team sizes
                    team1_size = len(team1.get("players", []))
                    team2_size = len(team2.get("players", []))
                    
                    if team1_size == 8 and team2_size == 8:
                        # Check position constraints
                        team1_positions = self.count_positions(team1["players"])
                        team2_positions = self.count_positions(team2["players"])
                        
                        constraints_met = (
                            team1_positions["DEF"] >= 2 and team1_positions["ATT"] >= 2 and team1_positions["MID"] >= 1 and
                            team2_positions["DEF"] >= 2 and team2_positions["ATT"] >= 2 and team2_positions["MID"] >= 1
                        )
                        
                        if constraints_met:
                            team1_points = team1.get("totalPoints", 0)
                            team2_points = team2.get("totalPoints", 0)
                            self.log_test("POST Shuffle Teams", True, 
                                        f"Teams created - Team1: {team1_points} pts, Team2: {team2_points} pts")
                            return result
                        else:
                            self.log_test("POST Shuffle Teams", False, "Position constraints not met")
                    else:
                        self.log_test("POST Shuffle Teams", False, f"Invalid team sizes: {team1_size}, {team2_size}")
                else:
                    self.log_test("POST Shuffle Teams", False, "Missing team1 or team2 in response")
            elif response.status_code == 400:
                error_msg = response.json().get("detail", "Bad request")
                self.log_test("POST Shuffle Teams", False, f"Bad request: {error_msg}")
            else:
                self.log_test("POST Shuffle Teams", False, f"HTTP {response.status_code}: {response.text}")
        except Exception as e:
            self.log_test("POST Shuffle Teams", False, f"Request error: {str(e)}")
        return None

    def count_positions(self, players: List[Dict]) -> Dict[str, int]:
        """Count players by position"""
        positions = {"DEF": 0, "MID": 0, "ATT": 0}
        for player in players:
            pos = player.get("position", "")
            if pos in positions:
                positions[pos] += 1
        return positions

    def test_error_scenarios(self):
        """Test various error scenarios"""
        
        # Test getting non-existent player
        try:
            response = requests.get(f"{self.base_url}/players/non-existent-id", timeout=10)
            if response.status_code == 404:
                self.log_test("Error: Get Non-existent Player", True, "Correctly returned 404")
            else:
                self.log_test("Error: Get Non-existent Player", False, f"Expected 404, got {response.status_code}")
        except Exception as e:
            self.log_test("Error: Get Non-existent Player", False, f"Request error: {str(e)}")

        # Test updating non-existent player
        try:
            response = requests.put(
                f"{self.base_url}/players/non-existent-id",
                json={"points": 50},
                timeout=10
            )
            if response.status_code == 404:
                self.log_test("Error: Update Non-existent Player", True, "Correctly returned 404")
            else:
                self.log_test("Error: Update Non-existent Player", False, f"Expected 404, got {response.status_code}")
        except Exception as e:
            self.log_test("Error: Update Non-existent Player", False, f"Request error: {str(e)}")

        # Test deleting non-existent player
        try:
            response = requests.delete(f"{self.base_url}/players/non-existent-id", timeout=10)
            if response.status_code == 404:
                self.log_test("Error: Delete Non-existent Player", True, "Correctly returned 404")
            else:
                self.log_test("Error: Delete Non-existent Player", False, f"Expected 404, got {response.status_code}")
        except Exception as e:
            self.log_test("Error: Delete Non-existent Player", False, f"Request error: {str(e)}")

        # Test creating player with invalid data
        invalid_player = {
            "name": "",  # Invalid: empty name
            "position": "INVALID",  # Invalid position
            "points": 150,  # Invalid: too high
            "photo": "invalid-url",
            "skills": {
                "pace": 0,  # Invalid: too low
                "shooting": 100,  # Invalid: too high
                "passing": 50,
                "defending": 50,
                "dribbling": 50,
                "physical": 50
            },
            "age": 10,  # Invalid: too young
            "preferredFoot": "Both",  # Invalid option
            "nationality": ""  # Invalid: empty
        }
        
        try:
            response = requests.post(
                f"{self.base_url}/players/",
                json=invalid_player,
                timeout=10
            )
            if response.status_code == 422:  # Validation error
                self.log_test("Error: Create Invalid Player", True, "Correctly returned validation error")
            else:
                self.log_test("Error: Create Invalid Player", False, f"Expected 422, got {response.status_code}")
        except Exception as e:
            self.log_test("Error: Create Invalid Player", False, f"Request error: {str(e)}")

    def run_all_tests(self):
        """Run all backend API tests"""
        print("=" * 60)
        print("FOOTBALL TEAM SHUFFLER - BACKEND API TESTS")
        print("=" * 60)
        print(f"Testing API at: {self.base_url}")
        print()

        # 1. Health check
        self.test_health_check()

        # 2. Get all players
        players = self.test_get_all_players()
        
        # 3. Test single player retrieval (if players exist)
        if players:
            first_player = players[0]
            self.test_get_single_player(first_player["id"])

        # 4. Test player creation
        created_player = self.test_create_player()

        # 5. Test player update (if we created one)
        if created_player:
            self.test_update_player(created_player["id"])

        # 6. Test shuffle functionality
        self.test_shuffle_teams()

        # 7. Test error scenarios
        self.test_error_scenarios()

        # 8. Clean up - delete created player
        if created_player:
            self.test_delete_player(created_player["id"])

        # Print summary
        self.print_summary()

    def print_summary(self):
        """Print test summary"""
        print("=" * 60)
        print("TEST SUMMARY")
        print("=" * 60)
        
        passed = sum(1 for result in self.test_results if "✅ PASS" in result["status"])
        failed = sum(1 for result in self.test_results if "❌ FAIL" in result["status"])
        total = len(self.test_results)
        
        print(f"Total Tests: {total}")
        print(f"Passed: {passed}")
        print(f"Failed: {failed}")
        print(f"Success Rate: {(passed/total*100):.1f}%" if total > 0 else "0%")
        print()
        
        if failed > 0:
            print("FAILED TESTS:")
            for result in self.test_results:
                if "❌ FAIL" in result["status"]:
                    print(f"  - {result['test']}: {result['message']}")
        else:
            print("🎉 ALL TESTS PASSED!")
        
        print("=" * 60)

if __name__ == "__main__":
    tester = FootballAPITester()
    tester.run_all_tests()