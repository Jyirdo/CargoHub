import pytest
import requests

BASE_URL = "http://localhost:5000/api/Orders"  # Replace with your actual base URL

@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }

@pytest.fixture
def sample_order():
    return {
        "sourceId": 1,
        "orderDate": "2025-01-01T12:00:00Z",
        "requestDate": "2025-01-10T12:00:00Z",
        "reference": "TestOrder123",
        "reference_extra": "ExtraRef",
        "order_status": "Pending",
        "notes": "Test order notes",
        "shippingNotes": "Shipping notes",
        "pickingNotes": "Picking notes",
        "warehouseId": 1,
        "shipTo": "Client1",
        "billTo": "Client1",
        "shipmentId": 1,
        "totalAmount": 100.0,
        "totalDiscount": 10.0,
        "totalTax": 15.0,
        "totalSurcharge": 5.0
    }

# Test GetAllOrders
@pytest.mark.asyncio
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
        assert "reference" in response.json()

# Test AddOrder
def test_add_order(headers, sample_order):
    url = f"{BASE_URL}/Add"
    response = requests.post(url, json=sample_order, headers=headers)

    assert response.status_code == 201
    created_order = response.json()
    assert created_order["reference"] == sample_order["reference"]

# Test UpdateOrder
@pytest.mark.asyncio
def test_update_order(headers, sample_order):
    order_id = 1  # Replace with a valid order ID
    url = f"{BASE_URL}/Update/{order_id}"

    sample_order["reference"] = "UpdatedTestOrder123"
    response = requests.put(url, json=sample_order, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        updated_order = requests.get(f"{BASE_URL}/{order_id}", headers=headers).json()
        assert updated_order["reference"] == "UpdatedTestOrder123"

# Test DeleteOrder
@pytest.mark.asyncio
def test_delete_order(headers):
    order_id = 1  # Replace with a valid order ID
    url = f"{BASE_URL}/Delete/{order_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        get_response = requests.get(f"{BASE_URL}/{order_id}", headers=headers)
        assert get_response.status_code == 204

# Test GetOrdersForShipment
def test_get_orders_for_shipment(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/shipment/{shipment_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test GetOrdersForClient
def test_get_orders_for_client(headers):
    client_id = "Client1"  # Replace with a valid client ID
    url = f"{BASE_URL}/client/{client_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)
