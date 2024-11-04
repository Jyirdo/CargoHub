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