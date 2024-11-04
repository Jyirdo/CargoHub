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