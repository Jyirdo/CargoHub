import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all orders
def test_get_all_orders(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/orders", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    orders = response.json()

    assert isinstance(orders, list)  # Check if the result is a list
    for orders in orders:
        # Ensure that key fields are present in each orders
        assert "id" in orders
        assert "source_id" in orders
        assert "order_date" in orders
        assert "request_date" in orders
        assert "reference" in orders
        assert "reference_extra" in orders
        assert "order_status" in orders
        assert "notes" in orders
        assert "shipping_notes" in orders
        assert "picking_notes" in orders
        assert "warehouse_id" in orders
        assert "ship_to" in orders
        assert "bill_to" in orders
        assert "shipment_id" in orders
        assert "total_amount" in orders
        assert "total_discount" in orders
        assert "total_tax" in orders
        assert "total_surcharge" in orders
        assert "created_at" in orders
        assert "updated_at" in orders
        assert "items" in orders

# Test to GET a specific order by its ID
def test_get_order_by_id(api_data):
    url, api_key = api_data
    order_id = 1
    response = requests.get(f"{url}/orders/{order_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    order = response.json()

    assert order["id"] == order_id
    assert "source_id" in order
    assert "order_date" in order
    assert "request_date" in order
    assert "reference" in order
    assert "reference_extra" in order
    assert "order_status" in order
    assert "notes" in order
    assert "shipping_notes" in order
    assert "picking_notes" in order
    assert "warehouse_id" in order
    assert "ship_to" in order
    assert "bill_to" in order
    assert "shipment_id" in order
    assert "total_amount" in order
    assert "total_discount" in order
    assert "total_tax" in order
    assert "total_surcharge" in order
    assert "created_at" in order
    assert "updated_at" in order
    assert "items" in order

# Test to ADD a new order and DELETE it afterwards
def test_add_and_delete_order(api_data):
    url, api_key = api_data
    new_order = {
        "id": 9999,
        "source_id": "SRC9999",
        "order_date": "2024-01-01",
        "request_date": "2024-01-02",
        "reference": "REF12345",
        "reference_extra": "EXTRA123",
        "order_status": "pending",
        "notes": "Test order",
        "shipping_notes": "Handle with care",
        "picking_notes": "Pick items carefully",
        "warehouse_id": 1,
        "ship_to": "Address A",
        "bill_to": "Address B",
        "shipment_id": 123,
        "total_amount": 1000,
        "total_discount": 50,
        "total_tax": 100,
        "total_surcharge": 20,
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00",
        "items": [{"item_id": 101, "quantity": 10}]
    }

    # Add the order
    post_response = requests.post(f"{url}/orders", json=new_order, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added order
    delete_response = requests.delete(f"{url}/orders/{new_order['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing order
def test_update_order(api_data):
    url, api_key = api_data
    order_id = 1

    # Fetch current order data to update
    original_data = requests.get(f"{url}/orders/{order_id}", headers={"API_KEY": api_key}).json()

    updated_order = {
        "id": order_id,
        "source_id": original_data["source_id"],
        "order_date": original_data["order_date"],
        "request_date": original_data["request_date"],
        "reference": original_data["reference"],
        "reference_extra": original_data["reference_extra"],
        "order_status": "completed",
        "notes": original_data["notes"],
        "shipping_notes": original_data["shipping_notes"],
        "picking_notes": original_data["picking_notes"],
        "warehouse_id": original_data["warehouse_id"],
        "ship_to": original_data["ship_to"],
        "bill_to": original_data["bill_to"],
        "shipment_id": original_data["shipment_id"],
        "total_amount": original_data["total_amount"],
        "total_discount": original_data["total_discount"],
        "total_tax": original_data["total_tax"],
        "total_surcharge": original_data["total_surcharge"],
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00",
        "items": original_data["items"]
    }

    # Send the updated order using PUT request
    put_response = requests.put(f"{url}/orders/{order_id}", json=updated_order, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the order status
    get_response = requests.get(f"{url}/orders/{order_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["order_status"] == "completed"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/orders/{order_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE an order by its ID
def test_delete_order_item(api_data):
    url, api_key = api_data
    new_order = {
        "id": 9999,
        "source_id": "SRC9999",
        "order_date": "2024-01-01",
        "request_date": "2024-01-02",
        "reference": "REF12345",
        "reference_extra": "EXTRA123",
        "order_status": "pending",
        "notes": "Test order",
        "shipping_notes": "Handle with care",
        "picking_notes": "Pick items carefully",
        "warehouse_id": 1,
        "ship_to": "Address A",
        "bill_to": "Address B",
        "shipment_id": 123,
        "total_amount": 1000,
        "total_discount": 50,
        "total_tax": 100,
        "total_surcharge": 20,
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00",
        "items": [{"item_id": 101, "quantity": 10}]
    }

    # Add the order (POST)
    post_response = requests.post(f"{url}/orders", json=new_order, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added order (DELETE)
    delete_response = requests.delete(f"{url}/orders/{new_order['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid order ID
def test_get_order_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/orders/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty order ID
def test_get_order_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/orders/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative order ID
def test_get_order_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/orders/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400