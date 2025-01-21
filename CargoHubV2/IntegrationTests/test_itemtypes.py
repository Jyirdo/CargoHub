import pytest
import requests
import time

BASE_URL = "http://localhost:5000/api/ItemTypes"


@pytest.fixture
def headers():
    return {
        "API_KEY": "cargohub123",
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_item_type():
    return {
        "name": "Test Item Type",
        "description": "A description for the test item type."
    }

# Test GetAllItemTypes
@pytest.mark.asyncio
def test_get_all_item_types(headers):
    url = f"{BASE_URL}/byAmount/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    response_json = response.json()
    assert isinstance(response_json, list), "Expected a list in response"
    assert len(response_json) > 0, "Response list is empty"

@pytest.mark.asyncio
def test_get_all_item_types_with_max_pagination(headers):
    url = f"{BASE_URL}/byAmount/1000"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    response_json = response.json()
    assert isinstance(response_json, list), "Expected a list in response"
    assert len(response_json) > 0, "Response list is empty"

# Test GetItemTypeById
def test_get_item_type_by_id(headers):
    item_type_id = 60  
    url = f"{BASE_URL}/{item_type_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"
    response_data = response.json()
    assert "name" in response_data, "Response JSON does not contain 'name' field"
    assert response_data[
        "name"] == "Pen Holder", f"Expected name 'Pen Holder', got {response_data['name']}"

# Test GetItemTypeByName
def test_get_item_type_by_name(headers):
    name = "Test Item Type"  
    url = f"{BASE_URL}/ByName/{name}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404], f"Unexpected status code: {response.status_code}"

    if response.status_code == 404:
        expected_message = f"Item type with Name: {name} not found."
        response_data = response.json()
        assert "message" in response_data, "Error response does not contain 'message' field"
        assert response_data["message"] == expected_message, (
            f"Expected message '{expected_message}', got '{response_data['message']}'"
        )
    elif response.status_code == 200:
        response_data = response.json()
        assert "name" in response_data, "Response JSON does not contain 'name' field"
        assert response_data["name"] == name, (
            f"Expected name '{name}', got '{response_data['name']}'"
        )

# Test AddItemType
def test_create_duplicate_item_type(headers, sample_item_type):
    url = f"{BASE_URL}/Add"

    requests.post(url, json=sample_item_type, headers=headers)
    time.sleep(1)  
    response2 = requests.post(url, json=sample_item_type, headers=headers)

    assert response2.status_code == 400, "Expected status code 400 (Bad Request)"
    assert "Item Type with this name already exists" in response2.text

# Test UpdateItemType
@pytest.mark.asyncio
def test_update_item_type(headers, sample_item_type):
    item_type_id = 101 
    url = f"{BASE_URL}/{item_type_id}"

    sample_item_type["name"] = "Updated Item Type Name"
    response = requests.put(url, json=sample_item_type, headers=headers)

    assert response.status_code in [200, 204], f"Unexpected status code: {response.status_code}"
    if response.status_code == 200:
        assert response.json()["name"] == "Updated Item Type Name", (
            f"Expected updated name 'Updated Item Type Name', got '{response.json()['name']}'"
        )

# Test RemoveItemTypeById
@pytest.mark.asyncio
def test_remove_item_type_by_id(headers):
    item_type_id = 101  
    url = f"{BASE_URL}/{item_type_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 404], f"Unexpected status code: {response.status_code}"
    if response.status_code == 200:
        assert "Item type deleted successfully" in response.text
