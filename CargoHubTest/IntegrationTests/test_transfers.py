import pytest
import requests

BASE_URL = "http://localhost:5000/api/Transfers" 

# Fixture voor headers
@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }

# Fixture voor een voorbeeldtransfer
@pytest.fixture
def sample_transfer():
    return {
        "reference": "REF123",
        "transfer_from": 1,
        "transfer_to": 2,
        "transfer_status": "Pending",
        "created_at": "2024-01-01T12:00:00Z",
        "updated_at": "2024-01-01T12:00:00Z",
        "items": []
    }

# Test: Get All Transfers
def test_get_all_transfers(headers):
    url = f"{BASE_URL}"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test: Get Transfer By ID
def test_get_transfer_by_id(headers):
    transfer_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{transfer_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert "transfer_status" in response.json()

# Test: Get Transfer Status By ID
def test_get_transfer_status_by_id(headers):
    transfer_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/Status/{transfer_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert "transferStatus" in response.json()

# Test: Get Transfers By Status
def test_get_transfers_by_status(headers):
    status = "Pending"  # Gebruik een bestaande status
    url = f"{BASE_URL}/ByStatus/{status}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    for transfer in response.json():
        assert transfer["transfer_status"] == status

# Test: Add New Transfer
def test_add_new_transfer(headers, sample_transfer):
    url = f"{BASE_URL}"

    response = requests.post(url, json=sample_transfer, headers=headers)

    assert response.status_code == 201
    if response.status_code == 201:
        assert response.json()["reference"] == sample_transfer["reference"]

# Test: Update Existing Transfer
def test_update_transfer(headers, sample_transfer):
    transfer_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{transfer_id}"

    sample_transfer["transfer_status"] = "Completed"
    response = requests.put(url, json=sample_transfer, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert response.json()["transfer_status"] == "Completed"

# Test: Delete Transfer By ID
def test_delete_transfer_by_id(headers):
    transfer_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{transfer_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]

# Test: Delete Transfers By Status
def test_delete_transfers_by_status(headers):
    status = "Pending"  # Gebruik een bestaande status
    url = f"{BASE_URL}/ByStatus/{status}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]
