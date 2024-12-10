import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all transfers
def test_get_all_transfers(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/transfers", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    transfers = response.json()

    assert isinstance(transfers, list)  # Check if the result is a list
    for transfers in transfers:
        # Ensure that key fields are present in each transfers
        assert "id" in transfers
        assert "reference" in transfers
        assert "transfer_from" in transfers
        assert "transfer_to" in transfers
        assert "transfer_status" in transfers
        assert "created_at" in transfers
        assert "updated_at" in transfers
        assert "items" in transfers

# Test to GET a specific transfer by its ID
def test_get_transfer_by_id(api_data):
    url, api_key = api_data
    transfer_id = 1
    response = requests.get(f"{url}/transfers/{transfer_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    transfer = response.json()

    assert transfer["id"] == transfer_id
    assert "reference" in transfer
    assert "transfer_from" in transfer
    assert "transfer_to" in transfer
    assert "transfer_status" in transfer
    assert "created_at" in transfer
    assert "updated_at" in transfer
    assert "items" in transfer

# Test to ADD a new transfer and DELETE it afterwards
def test_add_and_delete_transfer(api_data):
    url, api_key = api_data
    new_transfer = {
        "id": 9999,
        "reference": "Test Transfer",
        "transfer_from": "Warehouse A",
        "transfer_to": "Warehouse B",
        "transfer_status": "pending",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00",
        "items": [{"item_id": 101, "quantity": 10}]
    }

    # Add the transfer
    post_response = requests.post(f"{url}/transfers", json=new_transfer, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added transfer
    delete_response = requests.delete(f"{url}/transfers/{new_transfer['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing transfer
def test_update_transfer(api_data):
    url, api_key = api_data
    transfer_id = 1

    # Fetch current transfer data to update
    original_data = requests.get(f"{url}/transfers/{transfer_id}", headers={"API_KEY": api_key}).json()

    updated_transfer = {
        "id": transfer_id,
        "reference": "Updated Transfer",
        "transfer_from": original_data["transfer_from"],
        "transfer_to": original_data["transfer_to"],
        "transfer_status": "completed",
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00",
        "items": original_data["items"]
    }

    # Send the updated transfer using PUT request
    put_response = requests.put(f"{url}/transfers/{transfer_id}", json=updated_transfer, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the status
    get_response = requests.get(f"{url}/transfers/{transfer_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["transfer_status"] == "completed"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/transfers/{transfer_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE a transfer by its ID
def test_delete_transfer_item(api_data):
    url, api_key = api_data
    new_transfer = {
        "id": 9999,
        "reference": "Test Transfer",
        "transfer_from": "Warehouse A",
        "transfer_to": "Warehouse B",
        "transfer_status": "pending",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00",
        "items": [{"item_id": 101, "quantity": 10}]
    }

    # Add the transfer (POST)
    post_response = requests.post(f"{url}/transfers", json=new_transfer, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added transfer (DELETE)
    delete_response = requests.delete(f"{url}/transfers/{new_transfer['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid transfer ID
def test_get_transfer_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/transfers/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty transfer ID
def test_get_transfer_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/transfers/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative transfer ID
def test_get_transfer_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/transfers/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400
