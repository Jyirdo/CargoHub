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