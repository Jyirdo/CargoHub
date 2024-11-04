import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all warehouses
def test_get_all_warehouses(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/warehouses", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    warehouses = response.json()

    assert isinstance(warehouses, list)  # Check if the result is a list
    for warehouses in warehouses:
        # Ensure that key fields are present in each warehouses
        assert "id" in warehouses
        assert "code" in warehouses
        assert "name" in warehouses
        assert "address" in warehouses
        assert "zip" in warehouses
        assert "city" in warehouses
        assert "province" in warehouses
        assert "country" in warehouses
        assert "contact" in warehouses
        assert "created_at" in warehouses
        assert "updated_at" in warehouses

        contact = warehouses["contact"]
        assert isinstance(contact, dict)
        assert "name" in contact
        assert "phone" in contact
        assert "email" in contact