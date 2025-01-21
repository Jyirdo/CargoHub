import pytest
import requests

BASE_URL = "http://localhost:5000/api/Items"


@pytest.fixture
def headers():
    return {
        "API_KEY": "cargohub123",
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_item():
    return {
        "uid": "P011721",
        "code": "sjQ23408K",
        "description": "Face-to-face clear-thinking complexity",
        "short_description": "must",
        "upc_code": "6523540947122",
        "model_number": "63-OFFTq0T",
        "commodity_code": "oTo304",
        "item_line": 11,
        "item_group": 73,
        "item_type": 14,
        "unit_purchase_quantity": 47,
        "unit_order_quantity": 13,
        "pack_order_quantity": 11,
        "supplier_id": 34,
        "supplier_code": "SUP423",
        "supplier_part_number": "E-86805-uTM"
    }

# Test GetAllItems


@pytest.mark.asyncio
def test_get_all_items(headers):
    url = f"{BASE_URL}/byAmount/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)


# Test GetItemById
def test_get_item_by_id(headers):
    item_id = "test-uid"  # Replace with a valid item UID
    url = f"{BASE_URL}/{item_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert "description" in response.json()


# Test GetItemsByItemLine
def test_get_items_by_item_line(headers):
    item_line_id = 1  # Replace with a valid item line ID
    url = f"{BASE_URL}/ByItemLine/{item_line_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), dict)


# Test GetItemsByItemGroup
def test_get_items_by_item_group(headers):
    item_group_id = 1  # Replace with a valid item group ID
    url = f"{BASE_URL}/ByItemGroup/{item_group_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), dict)


# Test GetItemsByItemType
def test_get_items_by_item_type(headers):
    item_type_id = 1  # Replace with a valid item type ID
    url = f"{BASE_URL}/ByItemType/{item_type_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), dict)


# Test GetItemsBySupplier
def test_get_items_by_supplier(headers):
    supplier_id = 1  # Replace with a valid supplier ID
    url = f"{BASE_URL}/BySupplier/{supplier_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), dict)


# Test AddItem

def test_add_item(headers, sample_item):
    url = f"{BASE_URL}/Add"

    # Make the request
    response = requests.post(url, json=sample_item, headers=headers)

    # Log response for debugging
    print(f"Response Status: {response.status_code}")
    print(f"Response Text: {response.text}")

    # Assert response
    assert response.status_code in [201, 400], f"Unexpected status code: {response.status_code}"
    if response.status_code == 400:
        assert "Item with this UID already exists" in response.text, f"Unexpected error: {response.text}"

def test_populate_weight_in_kg(headers):
    url = f"{BASE_URL}/PopulateWeightInKg"  # Endpoint for populating WeightInKg column

    # Make the request
    response = requests.post(url, headers=headers)

    # Log response for debugging
    print(f"Response Status: {response.status_code}")
    print(f"Response Text: {response.text}")

    # Assert response
    assert response.status_code == 200, f"Unexpected status code: {response.status_code}"
    assert response.text == '"WeightInKg column populated with random values."', "Unexpected response text"

    # Verify the WeightInKg values are populated
    get_url = f"{BASE_URL}/byAmount/10"
    get_response = requests.get(get_url, headers=headers)

    assert get_response.status_code == 200, "Failed to retrieve items"
    items = get_response.json()

    # Ensure all items have a populated WeightInKg value
    for item in items:
        assert "weight_in_kg" in item, "WeightInKg is missing in the item"
        assert isinstance(item["weight_in_kg"], int), "WeightInKg is not an integer"



# Test UpdateItem

@pytest.mark.asyncio
def test_update_item(headers, sample_item):
    item_id = 1  # Replace with a valid item ID
    url = f"{BASE_URL}/{item_id}"

    sample_item["description"] = "Updated Description"
    response = requests.put(url, json=sample_item, headers=headers)

    assert response.status_code in [200, 204, 400]
    if response.status_code == 400:
        assert "ID in the URL does not match the ID in the payload" in response.text


# Test RemoveItem
@pytest.mark.asyncio
def test_remove_item(headers):
    item_id = 1  # Replace with a valid item ID
    url = f"{BASE_URL}/{item_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204, 404]
