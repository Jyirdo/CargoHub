import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all items
def test_get_all_items(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/items", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    items = response.json()

    assert isinstance(items, list)  # Check if the result is a list
    for items in items:
        # Ensure that key fields are present in each items
        assert "uid" in items
        assert "code" in items
        assert "description" in items
        assert "short_description" in items
        assert "upc_code" in items
        assert "model_number" in items
        assert "commodity_code" in items
        assert "item_line" in items
        assert "item_group" in items
        assert "item_type" in items
        assert "unit_purchase_quantity" in items
        assert "unit_order_quantity" in items
        assert "pack_order_quantity" in items
        assert "supplier_id" in items
        assert "supplier_code" in items
        assert "supplier_part_number" in items
        assert "created_at" in items
        assert "updated_at" in items

# Test to GET a specific item by its UID
def test_get_item_by_uid(api_data):
    url, api_key = api_data
    item_uid = 1
    response = requests.get(f"{url}/items/{item_uid}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    item = response.json()

    assert item["uid"] == item_uid
    assert "code" in item
    assert "description" in item
    assert "short_description" in item
    assert "upc_code" in item
    assert "model_number" in item
    assert "commodity_code" in item
    assert "item_line" in item
    assert "item_group" in item
    assert "item_type" in item
    assert "unit_purchase_quantity" in item
    assert "unit_order_quantity" in item
    assert "pack_order_quantity" in item
    assert "supplier_id" in item
    assert "supplier_code" in item
    assert "supplier_part_number" in item
    assert "created_at" in item
    assert "updated_at" in item

# Test to ADD a new item and DELETE it afterwards
def test_add_and_delete_item(api_data):
    url, api_key = api_data
    new_item = {
        "uid": 9999,
        "code": "ITEM9999",
        "description": "Test Item",
        "short_description": "Test Short Desc",
        "upc_code": "123456789012",
        "model_number": "MODEL9999",
        "commodity_code": "COMMOD9999",
        "item_line": "Line1",
        "item_group": "Group1",
        "item_type": "Type1",
        "unit_purchase_quantity": 1,
        "unit_order_quantity": 10,
        "pack_order_quantity": 5,
        "supplier_id": 123,
        "supplier_code": "SUP123",
        "supplier_part_number": "PART123",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item
    post_response = requests.post(f"{url}/items", json=new_item, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added item
    delete_response = requests.delete(f"{url}/items/{new_item['uid']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing item
def test_update_item(api_data):
    url, api_key = api_data
    item_uid = 1

    # Fetch current item data to update
    original_data = requests.get(f"{url}/items/{item_uid}", headers={"API_KEY": api_key}).json()

    updated_item = {
        "uid": item_uid,
        "code": original_data["code"],
        "description": "Updated Item Description",
        "short_description": original_data["short_description"],
        "upc_code": original_data["upc_code"],
        "model_number": original_data["model_number"],
        "commodity_code": original_data["commodity_code"],
        "item_line": original_data["item_line"],
        "item_group": original_data["item_group"],
        "item_type": original_data["item_type"],
        "unit_purchase_quantity": original_data["unit_purchase_quantity"],
        "unit_order_quantity": original_data["unit_order_quantity"],
        "pack_order_quantity": original_data["pack_order_quantity"],
        "supplier_id": original_data["supplier_id"],
        "supplier_code": original_data["supplier_code"],
        "supplier_part_number": original_data["supplier_part_number"],
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated item using PUT request
    put_response = requests.put(f"{url}/items/{item_uid}", json=updated_item, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the description
    get_response = requests.get(f"{url}/items/{item_uid}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["description"] == "Updated Item Description"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/items/{item_uid}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE an item by its UID
def test_delete_item(api_data):
    url, api_key = api_data
    new_item = {
        "uid": 9999,
        "code": "ITEM9999",
        "description": "Test Item",
        "short_description": "Test Short Desc",
        "upc_code": "123456789012",
        "model_number": "MODEL9999",
        "commodity_code": "COMMOD9999",
        "item_line": "Line1",
        "item_group": "Group1",
        "item_type": "Type1",
        "unit_purchase_quantity": 1,
        "unit_order_quantity": 10,
        "pack_order_quantity": 5,
        "supplier_id": 123,
        "supplier_code": "SUP123",
        "supplier_part_number": "PART123",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the item (POST)
    post_response = requests.post(f"{url}/items", json=new_item, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added item (DELETE)
    delete_response = requests.delete(f"{url}/items/{new_item['uid']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid item UID
def test_get_item_invalid_uid(api_data):
    url, api_key = api_data
    invalid_uid = "invalid_uid"  # Use an invalid UID

    response = requests.get(f"{url}/items/{invalid_uid}", headers={"API_KEY": api_key})
    
    # For invalid UIDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty item UID
def test_get_item_empty_uid(api_data):
    url, api_key = api_data
    empty_uid = ""  # Empty UID

    response = requests.get(f"{url}/items/{empty_uid}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty UID
    assert response.status_code == 400

# Test GET with a negative item UID
def test_get_item_negative_uid(api_data):
    url, api_key = api_data
    negative_uid = -1  # Negative UID

    response = requests.get(f"{url}/items/{negative_uid}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative UID
    assert response.status_code == 400