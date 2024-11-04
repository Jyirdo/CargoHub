import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all item_types
def test_get_all_item_types(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/item_types", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    item_types = response.json()

    assert isinstance(item_types, list)  # Check if the result is a list
    for item_types in item_types:
        # Ensure that key fields are present in each item_types
        assert "id" in item_types
        assert "name" in item_types
        assert "description" in item_types
        assert "created_at" in item_types
        assert "updated_at" in item_types