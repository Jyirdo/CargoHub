import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all suppliers
def test_get_all_suppliers(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/suppliers", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    suppliers = response.json()

    assert isinstance(suppliers, list)  # Check if the result is a list
    for suppliers in suppliers:
        # Ensure that key fields are present in each suppliers
        assert "id" in suppliers
        assert "code" in suppliers
        assert "name" in suppliers
        assert "address" in suppliers
        assert "address_extra" in suppliers
        assert "city" in suppliers
        assert "zip_code" in suppliers
        assert "province" in suppliers
        assert "country" in suppliers
        assert "contact_name" in suppliers
        assert "phonenumber" in suppliers
        assert "reference" in suppliers
        assert "created_at" in suppliers
        assert "updated_at" in suppliers