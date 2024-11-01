import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key


# Test to GET all itemitem_lines
def test_get_all_item_lines(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/item_lines", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    item_lines = response.json()

    assert isinstance(item_lines, list)  # Check if the result is a list
    for item_lines in item_lines:
        # Ensure that key fields are present in each item_lines
        assert "id" in item_lines
        assert "name" in item_lines
        assert "description" in item_lines
        assert "created_at" in item_lines
        assert "updated_at" in item_lines