import pytest
import requests

@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key


# Test to GET all clients
def test_get_all_clients(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/clients", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    clients = response.json()

    assert isinstance(clients, list)
    for client in clients:
        assert "id" in client
        assert "name" in client
        assert "address" in client


# Test to GET a specific client by its ID
def test_get_client_by_id(api_data):
    url, api_key = api_data
    client_id = 1
    response = requests.get(f"{url}/clients/{client_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    client = response.json()

    assert client["id"] == client_id
    assert "name" in client
    assert "address" in client


# Test to ADD a new client and DELETE it afterwards
def test_add_and_delete_client(api_data):
    url, api_key = api_data
    new_client = {
        "id": 9999,
        "name": "Test Client",
        "email": "test@example.com",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the client
    post_response = requests.post(f"{url}/clients", json=new_client, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added client
    delete_response = requests.delete(f"{url}/clients/{new_client['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200


# Test to UPDATE an existing client
def test_update_client(api_data):
    url, api_key = api_data
    client_id = 1

    # Fetch current client data to update
    original_data = requests.get(f"{url}/clients/{client_id}", headers={"API_KEY": api_key}).json()

    updated_client = {
        "id": client_id,
        "name": "Updated Client",
        "email": "updated@example.com",
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated client using PUT request
    put_response = requests.put(f"{url}/clients/{client_id}", json=updated_client, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the name
    get_response = requests.get(f"{url}/clients/{client_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["name"] == "Updated Client"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/clients/{client_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200


# Test to DELETE a client by its ID
def test_delete_client_item(api_data):
    url, api_key = api_data
    new_client = {
        "id": 9999,
        "name": "Test Client",
        "email": "test@example.com",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the client (POST)
    post_response = requests.post(f"{url}/clients", json=new_client, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added client (DELETE)
    delete_response = requests.delete(f"{url}/clients/{new_client['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200


# Test GET with invalid client ID
def test_get_client_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/clients/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404


# Test GET with an empty client ID
def test_get_client_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/clients/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400


# Test GET with a negative client ID
def test_get_client_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/clients/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400
