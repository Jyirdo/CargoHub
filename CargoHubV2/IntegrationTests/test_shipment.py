import pytest
import requests

BASE_URL = "http://localhost:5000/api/Shipments" 

@pytest.fixture
def headers():
    return {
        "API_KEY": "cargohub123",  
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
def test_get_all_shipments_by_amount(headers):
    """
    Test fetching a limited number of shipments by amount.
    """
    amount = 5  # Number of shipments to fetch
    url = f"{BASE_URL}/byAmount/{amount}"

    response = requests.get(url, headers=headers)

    # Ensure the response status code is successful
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"

    # Parse the response JSON
    shipments = response.json()

    # Ensure the response is a list
    assert isinstance(shipments, list), "Expected response to be a list"

    # Ensure the number of returned shipments does not exceed the requested amount
    assert len(shipments) <= amount, f"Expected at most {amount} shipments, got {len(shipments)}"

    # Optional: Validate the structure of each shipment in the response
    for shipment in shipments:
        assert "id" in shipment, "Shipment missing 'id'"
        assert "shipmentStatus" in shipment, "Shipment missing 'shipmentStatus'"


# Test AddShipment
def test_add_shipment(headers, sample_shipment):
    url = f"{BASE_URL}/Add"
    response = requests.post(url, json=sample_shipment, headers=headers)

    assert response.status_code == 201
    created_shipment = response.json()
    assert created_shipment["shipmentStatus"] == sample_shipment["shipmentStatus"]

def test_update_shipment(headers, sample_shipment):
    # Create a sample shipment to update
    create_url = f"{BASE_URL}/Add"
    response = requests.post(create_url, json=sample_shipment, headers=headers)

    assert response.status_code == 201, f"Expected 201, got {response.status_code}"
    shipment = response.json()
    shipment_id = shipment["id"]

# Test UpdateItemsInShipment


@pytest.mark.asyncio
def test_update_items_in_shipment(headers):
    shipment_id = 1  
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

    print(f"Request Payload: {updated_items}")
    print(f"Response Status Code: {response.status_code}")
    print(f"Response Body: {response.text}")

    assert response.status_code in [200, 204, 400], f"Unexpected status code: {response.status_code}"


    if response.status_code == 200:
        response_data = response.json()

        # Controleer of de response correct is
        if isinstance(response_data, bool) and response_data is False:
            print("API returned 'false', indicating the update was not successful.")
        else:
            assert isinstance(response_data, list), "Expected response to be a list"
            print(f"Updated Items Response Data: {response_data}")

            for item in response_data:
                assert "item_id" in item, "Response item missing 'item_id'"
                assert "quantity" in item, "Response item missing 'quantity'"

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