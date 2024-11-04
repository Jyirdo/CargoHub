import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all locations
def test_get_all_locations(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/locations", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    locations = response.json()

    assert isinstance(locations, list)  # Check if the result is a list
    for locations in locations:
        # Ensure that key fields are present in each locations
        assert "id" in locations
        assert "warehouse_id" in locations
        assert "name" in locations
        assert "created_at" in locations
        assert "updated_at" in locations