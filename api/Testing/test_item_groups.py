import pytest
import requests

@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key


# Test to GET all item_groups
def test_get_all_item_groups(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/item_groups", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    item_groups = response.json()

    assert isinstance(item_groups, list)
    for item_groups in item_groups:
        assert "id" in item_groups
        assert "name" in item_groups
        assert "description" in item_groups
        assert "created_at" in item_groups
        assert "updated_at" in item_groups

# Test to GET a specific item_group by its ID
def test_get_item_group_by_id(api_data):
    url, api_key = api_data
    item_group_id = 1
    response = requests.get(f"{url}/item_groups/{item_group_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    item_group = response.json()

    assert item_group["id"] == item_group_id
    assert "name" in item_group
    assert "description" in item_group
    assert "created_at" in item_group
    assert "updated_at" in item_group