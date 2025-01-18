import pytest
import requests

BASE_URL = "http://localhost:5000/api/Warehouses"  

@pytest.fixture
def headers():
    return {
        "API_KEY": "cargohub123",  
        "Content-Type": "application/json"
    }

@pytest.fixture
def sample_warehouse():
    return {
        "code": "WH123",
        "name": "Test Warehouse",
        "address": "123 Warehouse St",
        "zip": "12345",
        "city": "Warehouse City",
        "province": "Warehouse Province",
        "country": "Warehouse Country",
        "contact": {
            "name": "Warehouse Contact",
            "phone": "123-456-7890",
            "email": "contact@warehouse.com"
        },
        "created_at": "2024-01-01T12:00:00Z",
        "updated_at": "2024-01-01T12:00:00Z"
    }


def test_get_all_warehouses(headers):
    url = f"{BASE_URL}"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

def test_get_warehouse_by_id(headers):
    
    get_all_url = f"{BASE_URL}"
    get_all_response = requests.get(get_all_url, headers=headers)
    
    if get_all_response.status_code != 200 or not get_all_response.json():
        pytest.skip("Geen warehouses gevonden om te testen.")

   
    existing_warehouse = get_all_response.json()[0] 
    warehouse_id = existing_warehouse["id"]
    url = f"{BASE_URL}/{warehouse_id}"

   
    response = requests.get(url, headers=headers)

    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    response_data = response.json()
    assert "name" in response_data, "Expected 'name' in response"
    assert response_data["id"] == warehouse_id, f"Expected ID {warehouse_id}, got {response_data['id']}"


# Test: Get Warehouses By City
def test_get_warehouses_by_city(headers):
    city = "Warehouse City" 
    url = f"{BASE_URL}/ByCity/{city}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    for warehouse in response.json():
        assert warehouse["city"] == city

# Test: Add New Warehouse
def test_add_new_warehouse(headers, sample_warehouse):
    url = f"{BASE_URL}"

    response = requests.post(url, json=sample_warehouse, headers=headers)

    assert response.status_code == 201, f"Expected 201, got {response.status_code}"
    if response.status_code == 201:
        response_data = response.json()
        assert response_data["name"] == sample_warehouse["name"], \
            f"Expected name {sample_warehouse['name']}, got {response_data['name']}"

# Test: Update Existing Warehouse
def test_update_warehouse(headers, sample_warehouse):
    
    # First, create a new warehouse
    create_response = requests.post(BASE_URL, json=sample_warehouse, headers=headers)
    assert create_response.status_code == 201, f"Failed to create warehouse: {create_response.text}"

    warehouse_id = create_response.json()["id"]
    url = f"{BASE_URL}/{warehouse_id}"

    # Update the warehouse
    sample_warehouse["name"] = "Updated Warehouse Name"
    response = requests.put(url, json=sample_warehouse, headers=headers)

    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    response_data = response.json()
    assert "name" in response_data, "Expected 'name' in response"
    assert response_data["name"] == "Updated Warehouse Name", \
        f"Expected name 'Updated Warehouse Name', got {response_data['name']}"


# Test: Delete Warehouse By ID
def test_delete_warehouse_by_id(headers, sample_warehouse):
    create_response = requests.post(BASE_URL, json=sample_warehouse, headers=headers)
    assert create_response.status_code == 201, f"Failed to create warehouse: {create_response.text}"

    warehouse_id = create_response.json()["id"]
    url = f"{BASE_URL}/{warehouse_id}"

    delete_response = requests.delete(url, headers=headers)
    assert delete_response.status_code in [200, 204], f"Unexpected status code: {delete_response.status_code}"

    check_response = requests.get(url, headers=headers)
    assert check_response.status_code == 404, f"Warehouse with ID {warehouse_id} still exists after deletion."
