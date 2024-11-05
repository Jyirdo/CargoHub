import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all shipments
def test_get_all_shipments(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/shipments", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    shipments = response.json()

    assert isinstance(shipments, list)  # Check if the result is a list
    for shipments in shipments:
        # Ensure that key fields are present in each shipments
        assert "id" in shipments
        assert "order_id" in shipments
        assert "source_id" in shipments
        assert "order_date" in shipments
        assert "request_date" in shipments
        assert "shipment_date" in shipments
        assert "shipment_status" in shipments
        assert "notes" in shipments
        assert "carrier_code" in shipments
        assert "carrier_description" in shipments
        assert "service_code" in shipments
        assert "payment_type" in shipments
        assert "transfer_mode" in shipments
        assert "total_package_count" in shipments
        assert "total_package_weight" in shipments
        assert "created_at" in shipments
        assert "updated_at" in shipments

# Test to GET a specific shipment by its ID
def test_get_shipment_by_id(api_data):
    url, api_key = api_data
    shipment_id = 1
    response = requests.get(f"{url}/shipments/{shipment_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    shipment = response.json()

    assert shipment["id"] == shipment_id
    assert "order_id" in shipment
    assert "source_id" in shipment
    assert "order_date" in shipment
    assert "request_date" in shipment
    assert "shipment_date" in shipment
    assert "shipment_status" in shipment
    assert "notes" in shipment
    assert "carrier_code" in shipment
    assert "carrier_description" in shipment
    assert "service_code" in shipment
    assert "payment_type" in shipment
    assert "transfer_mode" in shipment
    assert "total_package_count" in shipment
    assert "total_package_weight" in shipment
    assert "created_at" in shipment
    assert "updated_at" in shipment

# Test to ADD a new shipment and DELETE it afterwards
def test_add_and_delete_shipment(api_data):
    url, api_key = api_data
    new_shipment = {
        "id": 9999,
        "order_id": "ORD9999",
        "source_id": "SRC9999",
        "order_date": "2024-01-01",
        "request_date": "2024-01-02",
        "shipment_date": "2024-01-03",
        "shipment_status": "pending",
        "notes": "Test shipment",
        "carrier_code": "CARRIER1",
        "carrier_description": "Test Carrier",
        "service_code": "SERV1",
        "payment_type": "prepaid",
        "transfer_mode": "air",
        "total_package_count": 5,
        "total_package_weight": 25.5,
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the shipment
    post_response = requests.post(f"{url}/shipments", json=new_shipment, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added shipment
    delete_response = requests.delete(f"{url}/shipments/{new_shipment['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing shipment
def test_update_shipment(api_data):
    url, api_key = api_data
    shipment_id = 1

    # Fetch current shipment data to update
    original_data = requests.get(f"{url}/shipments/{shipment_id}", headers={"API_KEY": api_key}).json()

    updated_shipment = {
        "id": shipment_id,
        "order_id": original_data["order_id"],
        "source_id": original_data["source_id"],
        "order_date": original_data["order_date"],
        "request_date": original_data["request_date"],
        "shipment_date": "2024-01-04",
        "shipment_status": "shipped",
        "notes": original_data["notes"],
        "carrier_code": original_data["carrier_code"],
        "carrier_description": original_data["carrier_description"],
        "service_code": original_data["service_code"],
        "payment_type": original_data["payment_type"],
        "transfer_mode": original_data["transfer_mode"],
        "total_package_count": original_data["total_package_count"],
        "total_package_weight": original_data["total_package_weight"],
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated shipment using PUT request
    put_response = requests.put(f"{url}/shipments/{shipment_id}", json=updated_shipment, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the shipment status
    get_response = requests.get(f"{url}/shipments/{shipment_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["shipment_status"] == "shipped"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/shipments/{shipment_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE a shipment by its ID
def test_delete_shipment_item(api_data):
    url, api_key = api_data
    new_shipment = {
        "id": 9999,
        "order_id": "ORD9999",
        "source_id": "SRC9999",
        "order_date": "2024-01-01",
        "request_date": "2024-01-02",
        "shipment_date": "2024-01-03",
        "shipment_status": "pending",
        "notes": "Test shipment",
        "carrier_code": "CARRIER1",
        "carrier_description": "Test Carrier",
        "service_code": "SERV1",
        "payment_type": "prepaid",
        "transfer_mode": "air",
        "total_package_count": 5,
        "total_package_weight": 25.5,
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the shipment (POST)
    post_response = requests.post(f"{url}/shipments", json=new_shipment, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added shipment (DELETE)
    delete_response = requests.delete(f"{url}/shipments/{new_shipment['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid shipment ID
def test_get_shipment_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/shipments/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty shipment ID
def test_get_shipment_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/shipments/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative shipment ID
def test_get_shipment_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/shipments/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400