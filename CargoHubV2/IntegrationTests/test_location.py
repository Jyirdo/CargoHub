import pytest
import requests

BASE_URL = "http://localhost:5000/api/Location"

# Fixture voor headers


@pytest.fixture
def headers():
    return {
        "API_KEY": "cargohub123", 
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


def test_get_all_locations(headers):
    url = f"{BASE_URL}/byAmount/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)


def test_get_location_by_id(headers):

    get_all_url = f"{BASE_URL}"
    get_all_response = requests.get(get_all_url, headers=headers)

    if get_all_response.status_code != 200 or not get_all_response.json():
        pytest.skip("Geen locaties gevonden om te testen.")

    existing_location = get_all_response.json()[0]
    location_id = existing_location["id"]
    url = f"{BASE_URL}/{location_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    response_data = response.json()

    print(f"Response Data: {response_data}")

    assert "id" in response_data, "Expected 'id' in response"
    assert response_data["id"] == location_id, f"Expected ID {location_id}, got {response_data['id']}"
    assert "name" in response_data, "Expected 'name' in response"
    assert "createdAt" in response_data, "Expected 'createdAt' in response"


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
    warehouse_id = 1
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
    location_id = 1
    url = f"{BASE_URL}/{location_id}"

    sample_location["name"] = "Updated Location Name"
    response = requests.put(url, json=sample_location, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert response.json()["name"] == "Updated Location Name"

# Test: Delete Location By ID


def test_delete_location_by_id(headers):
    location_id = 15 
    url = f"{BASE_URL}/{location_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]

# Test: Get Total Location Count


def test_get_location_count(headers):
    url = f"{BASE_URL}/Count"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), int)
