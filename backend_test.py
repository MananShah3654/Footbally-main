import requests
import sys
from datetime import datetime

class FootballyAPITester:
    def __init__(self, base_url="https://team-analytics-4.preview.emergentagent.com"):
        self.base_url = base_url
        self.api_url = f"{base_url}/api"
        self.tests_run = 0
        self.tests_passed = 0

    def run_test(self, name, method, endpoint, expected_status, data=None):
        """Run a single API test"""
        url = f"{self.api_url}/{endpoint}"
        headers = {'Content-Type': 'application/json'}

        self.tests_run += 1
        print(f"\n🔍 Testing {name}...")
        print(f"   URL: {url}")
        
        try:
            if method == 'GET':
                response = requests.get(url, headers=headers, timeout=10)
            elif method == 'POST':
                response = requests.post(url, json=data, headers=headers, timeout=10)
            elif method == 'PUT':
                response = requests.put(url, json=data, headers=headers, timeout=10)
            elif method == 'DELETE':
                response = requests.delete(url, headers=headers, timeout=10)

            success = response.status_code == expected_status
            if success:
                self.tests_passed += 1
                print(f"✅ Passed - Status: {response.status_code}")
                if response.content:
                    try:
                        json_data = response.json()
                        if isinstance(json_data, list):
                            print(f"   Response: List with {len(json_data)} items")
                        elif isinstance(json_data, dict):
                            print(f"   Response keys: {list(json_data.keys())}")
                    except:
                        print(f"   Response length: {len(response.content)} bytes")
            else:
                print(f"❌ Failed - Expected {expected_status}, got {response.status_code}")
                if response.content:
                    print(f"   Error: {response.text[:200]}")

            return success, response.json() if response.content and success else {}

        except requests.exceptions.Timeout:
            print(f"❌ Failed - Request timeout")
            return False, {}
        except requests.exceptions.ConnectionError:
            print(f"❌ Failed - Connection error")
            return False, {}
        except Exception as e:
            print(f"❌ Failed - Error: {str(e)}")
            return False, {}

    def test_health_check(self):
        """Test API health endpoint"""
        return self.run_test("Health Check", "GET", "status/health", 200)

    def test_get_players(self):
        """Test getting all players"""
        return self.run_test("Get All Players", "GET", "players", 200)

    def test_get_player_by_id(self, player_id):
        """Test getting a specific player"""
        return self.run_test(f"Get Player {player_id}", "GET", f"players/{player_id}", 200)

    def test_get_player_statistics(self, player_id):
        """Test getting player statistics"""
        return self.run_test(f"Get Player Statistics {player_id}", "GET", f"players/{player_id}/statistics", 200)

    def test_get_teams(self):
        """Test getting all teams"""
        return self.run_test("Get All Teams", "GET", "teams", 200)

    def test_create_player(self):
        """Test creating a new player"""
        player_data = {
            "name": "Test Player",
            "position": "Midfielder",
            "age": 25,
            "nationality": "Test Country",
            "height": 180,
            "weight": 75,
            "preferredFoot": "Right",
            "jerseyNumber": 99
        }
        return self.run_test("Create Player", "POST", "players", 201, player_data)

def main():
    print("🚀 Starting Footbally API Tests...")
    print("=" * 50)
    
    # Setup
    tester = FootballyAPITester()
    
    # Test basic connectivity
    print("\n📡 Testing API Connectivity...")
    health_success, health_data = tester.test_health_check()
    if not health_success:
        print("❌ API Health check failed - stopping tests")
        return 1

    # Test core endpoints
    print("\n👥 Testing Player Endpoints...")
    players_success, players_data = tester.test_get_players()
    
    if players_success and players_data:
        print(f"   Found {len(players_data)} players")
        
        # Test individual player endpoints with first player
        if len(players_data) > 0:
            first_player = players_data[0]
            player_id = first_player.get('id')
            if player_id:
                tester.test_get_player_by_id(player_id)
                tester.test_get_player_statistics(player_id)
    
    # Test teams endpoint
    print("\n🏆 Testing Team Endpoints...")
    tester.test_get_teams()
    
    # Test player creation (optional)
    print("\n➕ Testing Player Creation...")
    tester.test_create_player()

    # Print final results
    print("\n" + "=" * 50)
    print(f"📊 Final Results: {tester.tests_passed}/{tester.tests_run} tests passed")
    
    if tester.tests_passed == tester.tests_run:
        print("🎉 All tests passed!")
        return 0
    else:
        print(f"⚠️  {tester.tests_run - tester.tests_passed} tests failed")
        return 1

if __name__ == "__main__":
    sys.exit(main())