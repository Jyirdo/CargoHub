import pytest
import requests

BASE_URL = "http://localhost:5000/api/Orders"


@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_order():
    return {
        "source_id": 1,
        "order_date": "2023-01-01",
        "request_date": "2023-01-05",
        "reference": "TestOrder123",
        "reference_extra": "ExtraRef",
        "order_status": "Pending",
        "notes": "Test order notes",
        "shipping_notes": "Handle with care",
        "picking_notes": "Fragile items",
        "warehouse_id": 1,
        "ship_to": "123 Test St, Test City",
        "bill_to": "456 Billing St, Billing City",
        "shipment_id": 1,
        "total_amount": 1000.0,
        "total_discount": 50.0,
        "total_tax": 150.0,
        "total_surcharge": 20.0,
        "items": [
            {
                "item_id": 1,
                "quantity": 5
            },
            {
                "item_id": 2,
                "quantity": 3
            }
        ]
    }


# Test GetAllOrders
def test_get_all_orders(headers):
    url = f"{BASE_URL}"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)


# Test GetOrderById
def test_get_order_by_id(headers):
    order_id = 1  # Replace with a valid order ID
    url = f"{BASE_URL}/{order_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        assert "order_status" in response.json()


# Test GetItemsInOrder
def test_get_items_in_order(headers):
    order_id = 1  # Replace with a valid order ID
    url = f"{BASE_URL}/{order_id}/items"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), list)


# Test GetOrdersForShipment
def test_get_orders_for_shipment(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/shipment/{shipment_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)


# Test GetOrdersForClient
def test_get_orders_for_client(headers):
    client_id = "12345"  # Replace with a valid client ID
    url = f"{BASE_URL}/client/{client_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)


# Test AddOrder
def test_add_order(headers, sample_order):
    url = f"{BASE_URL}/Add"

    response = requests.post(url, json=sample_order, headers=headers)

    assert response.status_code in [201, 400]
    if response.status_code == 400:
        assert "error" in response.json()


# Test UpdateOrder
def test_update_order(headers, sample_order):
    order_id = 1  # Replace with a valid order ID
    url = f"{BASE_URL}/Update/{order_id}"

    sample_order["order_status"] = "Shipped"
    response = requests.put(url, json=sample_order, headers=headers)

    assert response.status_code in [204, 400, 404]


# Test DeleteOrder
def test_delete_order(headers):
    order_id = 1  # Replace with a valid order ID
    url = f"{BASE_URL}/Delete/{order_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204, 404]
