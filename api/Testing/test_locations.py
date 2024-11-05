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

# Test to GET a specific location by its ID
def test_get_location_by_id(api_data):
    url, api_key = api_data
    location_id = 1
    response = requests.get(f"{url}/locations/{location_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    location = response.json()

    assert location["id"] == location_id
    assert "warehouse_id" in location
    assert "name" in location
    assert "created_at" in location
    assert "updated_at" in location

# Test to ADD a new location and DELETE it afterwards
def test_add_and_delete_location(api_data):
    url, api_key = api_data
    new_location = {
        "id": 9999,
        "warehouse_id": 1,
        "name": "Test Location",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the location
    post_response = requests.post(f"{url}/locations", json=new_location, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added location
    delete_response = requests.delete(f"{url}/locations/{new_location['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing location
def test_update_location(api_data):
    url, api_key = api_data
    location_id = 1

    # Fetch current location data to update
    original_data = requests.get(f"{url}/locations/{location_id}", headers={"API_KEY": api_key}).json()

    updated_location = {
        "id": location_id,
        "warehouse_id": original_data["warehouse_id"],
        "name": "Updated Location",
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated location using PUT request
    put_response = requests.put(f"{url}/locations/{location_id}", json=updated_location, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the name
    get_response = requests.get(f"{url}/locations/{location_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["name"] == "Updated Location"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/locations/{location_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE a location by its ID
def test_delete_location_item(api_data):
    url, api_key = api_data
    new_location = {
        "id": 9999,
        "warehouse_id": 1,
        "name": "Test Location",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the location (POST)
    post_response = requests.post(f"{url}/locations", json=new_location, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added location (DELETE)
    delete_response = requests.delete(f"{url}/locations/{new_location['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid location ID
def test_get_location_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/locations/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty location ID
def test_get_location_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/locations/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative location ID
def test_get_location_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/locations/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400