import pytest
import requests
import time

BASE_URL = "http://localhost:5000/api/ItemGroups"


@pytest.fixture
def headers():
    return {
        "API_KEY": "cargohub123", 
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_item_group():
    return {
        "name": "Test Item Group",
        "description": "A description for the test item group."
    }


# Test GetAllItemGroups
@pytest.mark.asyncio
def test_get_all_item_groups(headers):
    url = f"{BASE_URL}/byAmount/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    response_json = response.json()
    assert isinstance(response.json(), list)
    assert len(response_json) > 0, "Response list is empty"


@pytest.mark.asyncio
def test_get_all_item_groups_with_max_pagination(headers):
    url = f"{BASE_URL}/byAmount/1000"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    response_json = response.json()
    assert isinstance(response.json(), list)
    assert len(response_json) > 0, "Response list is empty"


# Test GetItemGroupById
def test_get_item_group_by_id(headers):
    item_group_id = 1  
    url = f"{BASE_URL}/{item_group_id}"

    response = requests.get(url, headers=headers)
    assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"

    response_data = response.json()
    assert "name" in response_data, "Response JSON does not contain 'name' field"
    assert response_data["name"] == "Electronics", f"Expected name 'Electronics', got {response_data['name']}"

# Test GetItemGroupByName
def test_get_item_group_by_name(headers):
    name = "Test Item Group"  
    url = f"{BASE_URL}/ByName/{name}"

    response = requests.get(url, headers=headers)
    assert response.status_code in [200, 404], f"Unexpected status code: {response.status_code}"

    if response.status_code == 404:
        expected_message = f"Item group with Name: {name} not found."
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

# Test AddItemGroup
def test_create_duplicate_item_group(headers, sample_item_group):
    url = f"{BASE_URL}/Add"
    response1 = requests.post(url, json=sample_item_group, headers=headers)
    time.sleep(1)

    response2 = requests.post(url, json=sample_item_group, headers=headers)
    assert response1.status_code == 400  
    assert "Item Group with this name already exists" in response2.text

# Test UpdateItemGroup


@pytest.mark.asyncio
def test_update_item_group(headers, sample_item_group):
    item_group_id = 1  
    url = f"{BASE_URL}/{item_group_id}"

    sample_item_group["name"] = "Electronics"
    response = requests.put(url, json=sample_item_group, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        assert response.json()["name"] == "Electronics"


# Test RemoveItemGroupById
@pytest.mark.asyncio
def test_remove_item_group_by_id(headers):
    item_group_id = 110 
    url = f"{BASE_URL}/Delete/{item_group_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert "Item group deleted successfully" in response.text
