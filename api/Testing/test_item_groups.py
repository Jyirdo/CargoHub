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

# Test to ADD a new item_group and DELETE it afterwards
def test_add_and_delete_item_group(api_data):
    url, api_key = api_data
    new_item_group = {
        "id": 9999,
        "name": "Test Item Group",
        "description": "Description for Test Item Group",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item_group
    post_response = requests.post(f"{url}/item_groups", json=new_item_group, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added item_group
    delete_response = requests.delete(f"{url}/item_groups/{new_item_group['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing item_group
def test_update_item_group(api_data):
    url, api_key = api_data
    item_group_id = 1

    # Fetch current item_group data to update
    original_data = requests.get(f"{url}/item_groups/{item_group_id}", headers={"API_KEY": api_key}).json()

    updated_item_group = {
        "id": item_group_id,
        "name": "Updated Item Group",
        "description": "Updated description",
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated item_group using PUT request
    put_response = requests.put(f"{url}/item_groups/{item_group_id}", json=updated_item_group, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the description
    get_response = requests.get(f"{url}/item_groups/{item_group_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["description"] == "Updated description"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/item_groups/{item_group_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE an inventory item by its ID
def test_delete_item_group_item(api_data):
    url, api_key = api_data
    new_item_group = {
        "id": 9999,
        "name": "P999999",
        "description": "Test Item",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item_group (POST)
    post_response = requests.post(f"{url}/item_groups", json=new_item_group, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added item_group (DELETE)
    delete_response = requests.delete(f"{url}/item_groups/{new_item_group['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid item_group ID
def test_get_item_group_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/item_groups/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty item_group ID
def test_get_item_group_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/item_groups/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative item_group ID
def test_get_item_group_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/item_groups/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400