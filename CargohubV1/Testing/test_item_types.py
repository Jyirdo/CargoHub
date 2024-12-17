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

# Test to GET a specific item_type by its ID
def test_get_item_type_by_id(api_data):
    url, api_key = api_data
    item_type_id = 1
    response = requests.get(f"{url}/item_types/{item_type_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    item_type = response.json()

    assert item_type["id"] == item_type_id
    assert "name" in item_type
    assert "description" in item_type
    assert "created_at" in item_type
    assert "updated_at" in item_type

# Test to ADD a new item_type and DELETE it afterwards
def test_add_and_delete_item_type(api_data):
    url, api_key = api_data
    new_item_type = {
        "id": 9999,
        "name": "Test Item Type",
        "description": "Description for Test Item Type",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item_type
    post_response = requests.post(f"{url}/item_types", json=new_item_type, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added item_type
    delete_response = requests.delete(f"{url}/item_types/{new_item_type['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing item_type
def test_update_item_type(api_data):
    url, api_key = api_data
    item_type_id = 1

    # Fetch current item_type data to update
    original_data = requests.get(f"{url}/item_types/{item_type_id}", headers={"API_KEY": api_key}).json()

    updated_item_type = {
        "id": item_type_id,
        "name": "Updated Item Type",
        "description": "Updated description",
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated item_type using PUT request
    put_response = requests.put(f"{url}/item_types/{item_type_id}", json=updated_item_type, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the name
    get_response = requests.get(f"{url}/item_types/{item_type_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["name"] == "Updated Item Type"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/item_types/{item_type_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE an item_type by its ID
def test_delete_item_type(api_data):
    url, api_key = api_data
    new_item_type = {
        "id": 9999,
        "name": "Test Item Type",
        "description": "Description for Test Item Type",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item_type (POST)
    post_response = requests.post(f"{url}/item_types", json=new_item_type, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added item_type (DELETE)
    delete_response = requests.delete(f"{url}/item_types/{new_item_type['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid item_type ID
def test_get_item_type_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/item_types/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty item_type ID
def test_get_item_type_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/item_types/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative item_type ID
def test_get_item_type_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/item_types/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400