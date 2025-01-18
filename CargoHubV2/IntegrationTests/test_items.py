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
        "uid": "test-uid",
        "code": "TESTCODE123",
        "description": "Test Item Description",
        "short_description": "Test Short Desc",
        "upc_code": "123456789012",
        "model_number": "TST-123",
        "commodity_code": "98765",
        "item_line": 1,
        "item_group": 1,
        "item_type": 1,
        "unit_purchase_quantity": 10,
        "unit_order_quantity": 5,
        "pack_order_quantity": 2,
        "supplier_id": 1,
        "supplier_code": "SUP123",
        "supplier_part_number": "SUP-PART-123",
        "created_at": "2023-01-01T00:00:00Z",
        "updated_at": "2023-01-01T00:00:00Z"
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
        assert isinstance(response.json(), list)


# Test GetItemsByItemGroup
def test_get_items_by_item_group(headers):
    item_group_id = 1  # Replace with a valid item group ID
    url = f"{BASE_URL}/ByItemGroup/{item_group_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), list)


# Test GetItemsByItemType
def test_get_items_by_item_type(headers):
    item_type_id = 1  # Replace with a valid item type ID
    url = f"{BASE_URL}/ByItemType/{item_type_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), list)


# Test GetItemsBySupplier
def test_get_items_by_supplier(headers):
    supplier_id = 1  # Replace with a valid supplier ID
    url = f"{BASE_URL}/BySupplier/{supplier_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), list)


# Test AddItem
def test_add_item(headers, sample_item):
    url = f"{BASE_URL}/Add"

    response = requests.post(url, json=sample_item, headers=headers)

    assert response.status_code in [201, 400]
    if response.status_code == 400:
        assert "Item with this UID already exists" in response.text


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
