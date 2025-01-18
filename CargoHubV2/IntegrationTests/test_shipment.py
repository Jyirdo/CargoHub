import pytest
import requests

BASE_URL = "http://localhost:5000/api/Shipments"  # Replace with your actual base URL


@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_shipment():
    return {
        "orderId": 1,
        "sourceId": 1,
        "orderDate": "2025-01-01T12:00:00Z",
        "requestDate": "2025-01-05T12:00:00Z",
        "shipmentDate": "2025-01-07T12:00:00Z",
        "shipmentType": "Standard",
        "shipmentStatus": "Pending",
        "notes": "Test shipment notes",
        "carrierCode": "Carrier123",
        "carrierDescription": "Test Carrier",
        "serviceCode": "Service123",
        "paymentType": "Prepaid",
        "transferMode": "Ground",
        "totalPackageCount": 10,
        "totalPackageWeight": 100.0
    }


@pytest.fixture
def sample_items():
    return [
        {"itemId": 1, "quantity": 5},
        {"itemId": 2, "quantity": 10}
    ]

# Test GetAllShipments


@pytest.mark.asyncio
def test_get_all_shipments(headers):
    url = f"{BASE_URL}/byAmount/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test GetShipmentById


def test_get_shipment_by_id(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert "shipmentStatus" in response.json()

# Test AddShipment


def test_add_shipment(headers, sample_shipment):
    url = f"{BASE_URL}/Add"
    response = requests.post(url, json=sample_shipment, headers=headers)

    assert response.status_code == 201
    created_shipment = response.json()
    assert created_shipment["shipmentStatus"] == sample_shipment["shipmentStatus"]

# Test UpdateShipment


@pytest.mark.asyncio
def test_update_shipment(headers, sample_shipment):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}"

    sample_shipment["shipmentStatus"] = "UpdatedStatus"
    response = requests.put(url, json=sample_shipment, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        updated_shipment = requests.get(f"{BASE_URL}/{shipment_id}", headers=headers).json()
        assert updated_shipment["shipmentStatus"] == "UpdatedStatus"

# Test UpdateItemsInShipment


@pytest.mark.asyncio
def test_update_items_in_shipment(headers, sample_items):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}/items"

    response = requests.put(url, json=sample_items, headers=headers)

    assert response.status_code in [200, 204]

# Test RemoveShipment


@pytest.mark.asyncio
def test_remove_shipment(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204]
    if response.status_code == 200:
        get_response = requests.get(f"{BASE_URL}/{shipment_id}", headers=headers)
        assert get_response.status_code == 404

# Test GetItemsInShipment


def test_get_items_in_shipment(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}/items"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)
