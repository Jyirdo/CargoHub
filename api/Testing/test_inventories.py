import pytest
import requests


# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key


# Test to GET all inventories
def test_get_all_inventories(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/inventories", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    inventories = response.json()

    assert isinstance(inventories, list)  # Check if the result is a list
    for inventory in inventories:
        # Ensure that key fields are present in each inventory
        assert "id" in inventory
        assert "item_id" in inventory
        assert "description" in inventory
        assert "total_on_hand" in inventory


# Test to GET a specific inventory by its ID
def test_get_inventory_by_id(api_data):
    url, api_key = api_data
    inventory_id = 1  # Adjust this ID based on what exists in your database
    response = requests.get(f"{url}/inventories/{inventory_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    inventory = response.json()

    assert inventory["id"] == inventory_id  # Ensure ID matches
    assert "item_id" in inventory  # Check necessary fields
    assert "description" in inventory


# Test to ADD a new inventory and DELETE it afterwards
def test_add_and_delete_inventory(api_data):
    url, api_key = api_data
    new_inventory = {
        "id": 9999,
        "item_id": "P999999",
        "description": "Test Item",
        "total_on_hand": 100,
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the inventory (POST request)
    post_response = requests.post(f"{url}/inventories", json=new_inventory, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added inventory (DELETE request)
    delete_response = requests.delete(f"{url}/inventories/{new_inventory['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200


# Test to UPDATE an existing inventory
def test_update_inventory(api_data):
    url, api_key = api_data
    inventory_id = 1  # Adjust this ID based on your data

    # Fetch current inventory data to update
    original_data = requests.get(f"{url}/inventories/{inventory_id}", headers={"API_KEY": api_key}).json()

    updated_inventory = {
        "id": inventory_id,
        "item_id": "P000001",
        "description": "Updated Item",
        "total_on_hand": 300,
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated inventory using PUT request
    put_response = requests.put(f"{url}/inventories/{inventory_id}", json=updated_inventory, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the description
    get_response = requests.get(f"{url}/inventories/{inventory_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["description"] == "Updated Item"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/inventories/{inventory_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200


# Test to DELETE an inventory item by its ID
def test_delete_inventory_item(api_data):
    url, api_key = api_data
    new_inventory = {
        "id": 9999,
        "item_id": "P999999",
        "description": "Test Item",
        "total_on_hand": 100,
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the inventory (POST)
    post_response = requests.post(f"{url}/inventories", json=new_inventory, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added inventory (DELETE)
    delete_response = requests.delete(f"{url}/inventories/{new_inventory['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200


# Test GET with invalid inventory ID
def test_get_inventory_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/inventories/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404


# Test GET with an empty inventory ID
def test_get_inventory_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/inventories/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400


# Test GET with a negative inventory ID
def test_get_inventory_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/inventories/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400
