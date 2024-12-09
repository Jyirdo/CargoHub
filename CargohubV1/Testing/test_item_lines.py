import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all item_lines
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

# Test to GET a specific item_line by its ID
def test_get_item_line_by_id(api_data):
    url, api_key = api_data
    item_line_id = 1
    response = requests.get(f"{url}/item_lines/{item_line_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    item_line = response.json()

    assert item_line["id"] == item_line_id
    assert "name" in item_line
    assert "description" in item_line
    assert "created_at" in item_line
    assert "updated_at" in item_line

# Test to ADD a new item_line and DELETE it afterwards
def test_add_and_delete_item_line(api_data):
    url, api_key = api_data
    new_item_line = {
        "id": 9999,
        "name": "Test Item Line",
        "description": "Description for Test Item Line",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item_line
    post_response = requests.post(f"{url}/item_lines", json=new_item_line, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added item_line
    delete_response = requests.delete(f"{url}/item_lines/{new_item_line['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing item_line
def test_update_item_line(api_data):
    url, api_key = api_data
    item_line_id = 1

    # Fetch current item_line data to update
    original_data = requests.get(f"{url}/item_lines/{item_line_id}", headers={"API_KEY": api_key}).json()

    updated_item_line = {
        "id": item_line_id,
        "name": "Updated Item Line",
        "description": "Updated description",
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated item_line using PUT request
    put_response = requests.put(f"{url}/item_lines/{item_line_id}", json=updated_item_line, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the name
    get_response = requests.get(f"{url}/item_lines/{item_line_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["name"] == "Updated Item Line"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/item_lines/{item_line_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE an item_line by its ID
def test_delete_item_line(api_data):
    url, api_key = api_data
    new_item_line = {
        "id": 9999,
        "name": "Test Item Line",
        "description": "Description for Test Item Line",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item_line (POST)
    post_response = requests.post(f"{url}/item_lines", json=new_item_line, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added item_line (DELETE)
    delete_response = requests.delete(f"{url}/item_lines/{new_item_line['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid item_line ID
def test_get_item_line_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/item_lines/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty item_line ID
def test_get_item_line_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/item_lines/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative item_line ID
def test_get_item_line_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/item_lines/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400