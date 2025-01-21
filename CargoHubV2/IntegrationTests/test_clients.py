import pytest
import requests
import time
BASE_URL = "http://localhost:5000/api/Clients" 


@pytest.fixture
def headers():
    return {
        "API_KEY": "cargohub123", 
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_client():
    return {
        "name": "Test Client",
        "address": "123 Test Street",
        "city": "Testville",
        "zipCode": "12345",
        "province": "Test Province",
        "country": "Testland",
        "contactName": "Test Contact",
        "contactPhone": "123-456-7890",
        "contactEmail": "testclient@testt.com"
    }

# Test GetAllClients


@pytest.mark.asyncio
def test_get_all_clients(headers):
    url = f"{BASE_URL}/byAmount/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test GetClientById


def test_get_client_by_id(headers):
    client_id = 1  
    url = f"{BASE_URL}/{client_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert "address" in response.json()

# Test GetClientByEmail

def test_get_client_by_email(headers):
    email = "robertcharles@example.net"
    url = f"{BASE_URL}/Email/{email}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        assert response.json()["contactEmail"] == email

# Test CreateClient

def test_create_duplicate_client(headers, sample_client):
    url = f"{BASE_URL}/Add"

    requests.post(url, json=sample_client, headers=headers)
    time.sleep(1)
    response = requests.post(url, json=sample_client, headers=headers)

    assert "Client already exists" in response.text

# Test UpdateClient

@pytest.mark.asyncio
def test_update_client(headers, sample_client):
    client_id = 9824  
    url = f"{BASE_URL}/Update/{client_id}"

    sample_client["Name"] = "Updated Test Name"
    response = requests.put(url, json=sample_client, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        assert response.json()["name"] == "Updated Test Name"

# Test RemoveClientById

@pytest.mark.asyncio
def test_remove_client_by_id(headers):
    client_id = 9823  
    url = f"{BASE_URL}/Delete/{client_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]

# Test RemoveClientByEmail

@pytest.mark.asyncio
def test_remove_client_by_email(headers):
    email = "testclient@testt.com"  
    url = f"{BASE_URL}/Delete/Email/{email}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]
