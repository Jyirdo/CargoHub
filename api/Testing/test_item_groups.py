import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key


# Test to GET all itemitem_groups
def test_get_all_item_groups(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/item_groups", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    item_groups = response.json()

    assert isinstance(item_groups, list)  # Check if the result is a list
    for item_groups in item_groups:
        # Ensure that key fields are present in each item_groups
        assert "id" in item_groups
        assert "name" in item_groups
        assert "description" in item_groups
        assert "created_at" in item_groups
        assert "updated_at" in item_groups