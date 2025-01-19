import pytest
import requests

BASE_URL = "http://localhost:5000/api/Shipments"


@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_shipment():
    return {
        "order_id": 12345,
        "source_id": 1,
        "order_date": "2023-01-01",
        "request_date": "2023-01-05",
        "shipment_date": "2023-01-10",
        "shipment_type": "Standard",
        "shipment_status": "Pending",
        "notes": "Test shipment notes",
        "carrier_code": "CARR123",
        "carrier_description": "Test Carrier",
        "service_code": "SERV123",
        "payment_type": "Prepaid",
        "transfer_mode": "Air",
        "total_package_count": 10,
        "total_package_weight": 200.5,
        "items": [
            {
                "item_id": 1,
                "quantity": 5
            },
            {
                "item_id": 2,
                "quantity": 10
            }
        ]
    }


# Test GetAllShipments
@pytest.mark.asyncio
def test_get_all_shipments(headers):
    url = f"{BASE_URL}"
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
        assert "shipment_status" in response.json()


# Test GetItemsInShipment
def test_get_items_in_shipment(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}/items"

    response = requests.get(url, headers=headers)

    assert response.status_code in [200, 404]
    if response.status_code == 200:
        assert isinstance(response.json(), list)


# Test AddShipment
def test_add_shipment(headers, sample_shipment):
    url = f"{BASE_URL}/Add"

    response = requests.post(url, json=sample_shipment, headers=headers)

    assert response.status_code in [201, 400]
    if response.status_code == 400:
        assert "error" in response.json()


# Test UpdateShipment
@pytest.mark.asyncio
def test_update_shipment(headers, sample_shipment):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}"

    sample_shipment["shipment_status"] = "Shipped"
    response = requests.put(url, json=sample_shipment, headers=headers)

    assert response.status_code in [200, 204, 400]
    if response.status_code == 200:
        assert response.json()["shipment_status"] == "Shipped"


# Test UpdateItemsInShipment
@pytest.mark.asyncio
def test_update_items_in_shipment(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}/items"

    updated_items = [
        {
            "item_id": 1,
            "quantity": 8
        },
        {
            "item_id": 2,
            "quantity": 12
        }
    ]
    response = requests.put(url, json=updated_items, headers=headers)

    assert response.status_code in [200, 204, 400]
    if response.status_code == 200:
        assert isinstance(response.json(), list)


# Test RemoveShipment
@pytest.mark.asyncio
def test_remove_shipment(headers):
    shipment_id = 1  # Replace with a valid shipment ID
    url = f"{BASE_URL}/{shipment_id}"

    response = requests.delete(url, headers=headers)

    assert response.status_code in [200, 204, 404]
