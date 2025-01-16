import pytest
import requests

BASE_URL = "http://localhost:5000/api/Warehouses"  

# Fixture voor headers
@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }

# Fixture voor een voorbeeldmagazijn
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

# Test: Get All Warehouses
def test_get_all_warehouses(headers):
    url = f"{BASE_URL}"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test: Get Warehouse By ID
def test_get_warehouse_by_id(headers):
    warehouse_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{warehouse_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert "name" in response.json()

# Test: Get Warehouses By City
def test_get_warehouses_by_city(headers):
    city = "Warehouse City"  # Gebruik een bestaande stad
    url = f"{BASE_URL}/ByCity/{city}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    for warehouse in response.json():
        assert warehouse["city"] == city

# Test: Add New Warehouse
def test_add_new_warehouse(headers, sample_warehouse):
    url = f"{BASE_URL}"

    response = requests.post(url, json=sample_warehouse, headers=headers)

    assert response.status_code == 201
    if response.status_code == 201:
        assert response.json()["name"] == sample_warehouse["name"]

# Test: Update Existing Warehouse
def test_update_warehouse(headers, sample_warehouse):
    warehouse_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{warehouse_id}"

    sample_warehouse["name"] = "Updated Warehouse Name"
    response = requests.put(url, json=sample_warehouse, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert response.json()["name"] == "Updated Warehouse Name"

# Test: Delete Warehouse By ID
def test_delete_warehouse_by_id(headers):
    warehouse_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{warehouse_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]
