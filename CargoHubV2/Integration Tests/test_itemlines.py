import pytest
import requests
import time

BASE_URL = "http://localhost:5000/api/ItemLines"  


@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_item_line():
    return {
        "name": "Test Item Line",
        "description": "A description for the test item line."
    }


# Test GetAllItemLines
@pytest.mark.asyncio
def test_get_all_item_lines(headers):
    url = f"{BASE_URL}/page/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    response_json = response.json()
    assert isinstance(response_json, list), "Expected a list in response"
    assert len(response_json) > 0, "Response list is empty"


@pytest.mark.asyncio
def test_get_all_item_lines_with_max_pagination(headers):
    url = f"{BASE_URL}/page/1000"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    response_json = response.json()
    assert isinstance(response_json, list), "Expected a list in response"
    assert len(response_json) > 0, "Response list is empty"


# Test GetItemLineById
def test_get_item_line_by_id(headers):
    item_line_id = 1  # Replace with a valid item line ID
    url = f"{BASE_URL}/{item_line_id}"

    response = requests.get(url, headers=headers)

    # Check for successful response
    assert response.status_code == 200, f"Expected status code 200, got {response.status_code}"

    # Check that the response contains a 'name' field
    response_data = response.json()
    assert "name" in response_data, "Response JSON does not contain 'name' field"
    assert response_data["name"] == "Tech Gadgets", f"Expected name 'Tech Gadgets', got {response_data['name']}"


# Test GetItemLineByName
def test_get_item_line_by_name(headers):
    name = "Test Item Line"  # Replace with a valid item line name
    url = f"{BASE_URL}/ByName/{name}"

    response = requests.get(url, headers=headers)

    # Check for both success and "not found" scenarios
    assert response.status_code in [200, 404], f"Unexpected status code: {response.status_code}"

    if response.status_code == 404:
        # Ensure the error message matches the expected pattern
        expected_message = f"Item line with Name: {name} not found."
        response_data = response.json()  # Assuming API returns a JSON response
        assert "message" in response_data, "Error response does not contain 'message' field"
        assert response_data["message"] == expected_message, (
            f"Expected message '{expected_message}', got '{response_data['message']}'"
        )
    elif response.status_code == 200:
        # Validate the 'name' field in successful responses
        response_data = response.json()
        assert "name" in response_data, "Response JSON does not contain 'name' field"
        assert response_data["name"] == name, (
            f"Expected name '{name}', got '{response_data['name']}'"
        )


# Test AddItemLine
def test_create_duplicate_item_line(headers, sample_item_line):
    url = f"{BASE_URL}/Add"

    # First request to create the item line
    response1 = requests.post(url, json=sample_item_line, headers=headers)

    # Wait for 1 second before the second request to ensure no concurrency issue
    time.sleep(1)

    # Second request to create the same item line
    response2 = requests.post(url, json=sample_item_line, headers=headers)
    assert response2.status_code == 400, "Expected status code 400 (Bad Request)"
    assert "Item Group with this name already exists" in response2.text


# Test UpdateItemLine
@pytest.mark.asyncio
def test_update_item_line(headers, sample_item_line):
    item_line_id = 99  # Replace with a valid item line ID
    url = f"{BASE_URL}/{item_line_id}"

    sample_item_line["name"] = "Updated Item Line Name"
    response = requests.put(url, json=sample_item_line, headers=headers)

    assert response.status_code in [200, 204], f"Unexpected status code: {response.status_code}"
    if response.status_code == 200:
        assert response.json()["name"] == "Updated Item Line Name", (
            f"Expected updated name 'Updated Item Line Name', got '{response.json()['name']}'"
        )


# Test RemoveItemLineById
@pytest.mark.asyncio
def test_remove_item_line_by_id(headers):
    item_line_id = 99  # Replace with a valid item line ID
    url = f"{BASE_URL}/Delete/{item_line_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 404], f"Unexpected status code: {response.status_code}"
    if response.status_code == 200:
        assert "Item group deleted successfully" in response.text