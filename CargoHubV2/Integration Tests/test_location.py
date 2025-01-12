import pytest
import requests

BASE_URL = "http://localhost:5000/api/Location"  

# Fixture voor headers
@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }

# Fixture voor een voorbeeldlocatie
@pytest.fixture
def sample_location():
    return {
        "warehouse_id": 1,
        "code": "LOC123",
        "name": "Test Location",
        "created_at": "2024-01-01T12:00:00Z",
        "updated_at": "2024-01-01T12:00:00Z"
    }

# Test: Get All Locations
def test_get_all_locations(headers):
    url = f"{BASE_URL}"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test: Get Location By ID
def test_get_location_by_id(headers):
    location_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{location_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert "name" in response.json()

# Test: Search Location By Name
def test_search_location_by_name(headers):
    name = "Test Location"
    url = f"{BASE_URL}/Search/Name/{name}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    for location in response.json():
        assert name.lower() in location["name"].lower()

# Test: Search Location By Code
def test_search_location_by_code(headers):
    code = "LOC123"
    url = f"{BASE_URL}/Search/Code/{code}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert response.json()["code"] == code

# Test: Filter Locations By Warehouse ID
def test_filter_locations_by_warehouse(headers):
    warehouse_id = 1  # Vervang met een bestaande warehouse ID
    url = f"{BASE_URL}/Warehouse/{warehouse_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    for location in response.json():
        assert location["warehouse_id"] == warehouse_id

# Test: Add New Location
def test_add_new_location(headers, sample_location):
    url = f"{BASE_URL}/Add"

    response = requests.post(url, json=sample_location, headers=headers)

    assert response.status_code == 201
    if response.status_code == 201:
        assert response.json()["name"] == sample_location["name"]

# Test: Update Existing Location
def test_update_location(headers, sample_location):
    location_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{location_id}"

    sample_location["name"] = "Updated Location Name"
    response = requests.put(url, json=sample_location, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert response.json()["name"] == "Updated Location Name"

# Test: Delete Location By ID
def test_delete_location_by_id(headers):
    location_id = 1  # Vervang met een bestaande ID
    url = f"{BASE_URL}/{location_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]

# Test: Get Total Location Count
def test_get_location_count(headers):
    url = f"{BASE_URL}/Count"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), int)
