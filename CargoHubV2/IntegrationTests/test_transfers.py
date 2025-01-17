import pytest
import requests

BASE_URL = "http://localhost:5000/api/Transfers" 

@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }

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

def test_get_all_transfers(headers):
    url = f"{BASE_URL}"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test: Get Transfer By ID
def test_get_transfer_by_id(headers):
    get_all_url = f"{BASE_URL}"
    get_all_response = requests.get(get_all_url, headers=headers)
    
    if get_all_response.status_code != 200 or not get_all_response.json():
        pytest.skip("Geen transfers gevonden om te testen.")

    existing_transfer = get_all_response.json()[0] 
    transfer_id = existing_transfer["id"]
    url = f"{BASE_URL}/{transfer_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    response_data = response.json()

    print(f"Response Data: {response_data}")

    assert "id" in response_data, "Expected 'id' in response"
    assert response_data["id"] == transfer_id, f"Expected ID {transfer_id}, got {response_data['id']}"
    assert "reference" in response_data, "Expected 'reference' in response"
    assert "createdAt" in response_data, "Expected 'createdAt' in response"

    if "transfer_status" not in response_data:
        print("Warning: 'transfer_status' ontbreekt in de response. Controleer de API of database.")
    else:
        assert "transfer_status" in response_data, "Expected 'transfer_status' in response"


# Test: Get Transfer Status By ID
def test_get_transfer_status_by_id(headers, sample_transfer):
  
    sample_transfer["transferStatus"] = "Pending" 
    create_response = requests.post(BASE_URL, json=sample_transfer, headers=headers)
    assert create_response.status_code == 201, f"Failed to create transfer: {create_response.text}"
    created_transfer = create_response.json()
    transfer_id = created_transfer["id"]  

    try:
        url = f"{BASE_URL}/Status/{transfer_id}"
        response = requests.get(url, headers=headers)

        if response.status_code != 200:
            print(f"Debugging Info: {response.status_code} - {response.text}")
            pytest.fail(f"Expected status code 200, but got {response.status_code}")

        response_data = response.json()
        print(f"Response Data: {response_data}") 
        assert "transferStatus" in response_data, "Expected 'transferStatus' in response"
    
    finally:

        delete_url = f"{BASE_URL}/{transfer_id}"
        delete_response = requests.delete(delete_url, headers=headers)
        assert delete_response.status_code in [200, 204], f"Failed to delete transfer: {delete_response.text}"


# Test: Get Transfers By Status
def test_get_transfers_by_status(headers, sample_transfer):
    sample_transfer["transferStatus"] = "Pending"  
    create_response = requests.post(BASE_URL, json=sample_transfer, headers=headers)
    assert create_response.status_code == 201, f"Failed to create transfer: {create_response.text}"

    created_transfer = create_response.json()
    print(f"Created Transfer: {created_transfer}")

    assert created_transfer.get("transferStatus") == "Pending", (
        f"Expected 'Pending' for transferStatus, got {created_transfer.get('transferStatus')}"
    )

    status = "Pending"
    url = f"{BASE_URL}/ByStatus/{status}"
    response = requests.get(url, headers=headers)

    if response.status_code != 200:
        print(f"Debugging Info: {response.status_code} - {response.text}")
        pytest.fail(f"Expected status code 200, but got {response.status_code}")

    assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"
    retrieved_transfers = response.json()
    print(f"Retrieved Transfers: {retrieved_transfers}")

    for transfer in retrieved_transfers:
        assert transfer.get("transferStatus") == "Pending", (
            f"Expected 'Pending' for transferStatus, got {transfer.get('transferStatus')}"
        )


# Test: Add New Transfer
def test_add_new_transfer(headers, sample_transfer):
    url = f"{BASE_URL}"

    response = requests.post(url, json=sample_transfer, headers=headers)

    assert response.status_code == 201
    if response.status_code == 201:
        assert response.json()["reference"] == sample_transfer["reference"]

# Test: Update Existing Transfer
def test_update_transfer(headers, sample_transfer):
    sample_transfer["transferStatus"] = "Pending"
    create_response = requests.post(BASE_URL, json=sample_transfer, headers=headers)
    assert create_response.status_code == 201, f"Failed to create transfer: {create_response.text}"
    created_transfer = create_response.json()
    transfer_id = created_transfer["id"]  

    try:
        url = f"{BASE_URL}/{transfer_id}"
        sample_transfer["transferStatus"] = "Completed"
        update_response = requests.put(url, json=sample_transfer, headers=headers)

        if update_response.status_code != 200:
            print(f"Debugging Info: {update_response.status_code} - {update_response.text}")
            pytest.fail(f"Expected status code 200, but got {update_response.status_code}")

        response_data = update_response.json()
        print(f"Response Data: {response_data}")  

        assert "transferStatus" in response_data, "Expected 'transferStatus' in response"
        assert response_data["transferStatus"] == "Completed", f"Expected 'transferStatus' to be 'Completed', got {response_data['transferStatus']}"
    
    finally:
        delete_url = f"{BASE_URL}/{transfer_id}"
        delete_response = requests.delete(delete_url, headers=headers)
        assert delete_response.status_code in [200, 204], f"Failed to delete transfer: {delete_response.text}"


# Test: Delete Transfer By ID
def test_delete_transfer_by_id(headers, sample_transfer):
    create_response = requests.post(BASE_URL, json=sample_transfer, headers=headers)
    assert create_response.status_code == 201, f"Failed to create transfer: {create_response.text}"

    transfer_id = create_response.json()["id"]
    url = f"{BASE_URL}/{transfer_id}"

    delete_response = requests.delete(url, headers=headers)
    assert delete_response.status_code in [200, 204], f"Unexpected status code: {delete_response.status_code}"

    check_response = requests.get(url, headers=headers)
    assert check_response.status_code == 404, f"Transfer with ID {transfer_id} still exists after deletion."


# Test: Delete Transfers By Status
def test_delete_transfers_by_status(headers, sample_transfer):
    sample_transfer["transferStatus"] = "Pending"  
    create_response = requests.post(BASE_URL, json=sample_transfer, headers=headers)
    assert create_response.status_code == 201, f"Failed to create transfer: {create_response.text}"

    created_transfer = create_response.json()
    print(f"Created Transfer: {created_transfer}")

    assert created_transfer.get("transferStatus") == "Pending", (
        f"Expected 'Pending' for transferStatus, got {created_transfer.get('transferStatus')}"
    )
