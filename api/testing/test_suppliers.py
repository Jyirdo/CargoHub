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

# Test to GET a specific supplier by its ID
def test_get_supplier_by_id(api_data):
    url, api_key = api_data
    supplier_id = 1
    response = requests.get(f"{url}/suppliers/{supplier_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    supplier = response.json()

    assert supplier["id"] == supplier_id
    assert "code" in supplier
    assert "name" in supplier
    assert "address" in supplier
    assert "address_extra" in supplier
    assert "city" in supplier
    assert "zip_code" in supplier
    assert "province" in supplier
    assert "country" in supplier
    assert "contact_name" in supplier
    assert "phonenumber" in supplier
    assert "reference" in supplier
    assert "created_at" in supplier
    assert "updated_at" in supplier

# Test to ADD a new supplier and DELETE it afterwards
def test_add_and_delete_supplier(api_data):
    url, api_key = api_data
    new_supplier = {
        "id": 9999,
        "code": "SUPP9999",
        "name": "Test Supplier",
        "address": "123 Test St",
        "address_extra": "Suite 100",
        "city": "Testville",
        "zip_code": "12345",
        "province": "Test Province",
        "country": "Testland",
        "contact_name": "John Doe",
        "phonenumber": "123-456-7890",
        "reference": "REF123",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the supplier
    post_response = requests.post(f"{url}/suppliers", json=new_supplier, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added supplier
    delete_response = requests.delete(f"{url}/suppliers/{new_supplier['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing supplier
def test_update_supplier(api_data):
    url, api_key = api_data
    supplier_id = 1

    # Fetch current supplier data to update
    original_data = requests.get(f"{url}/suppliers/{supplier_id}", headers={"API_KEY": api_key}).json()

    updated_supplier = {
        "id": supplier_id,
        "code": original_data["code"],
        "name": "Updated Supplier",
        "address": original_data["address"],
        "address_extra": original_data["address_extra"],
        "city": original_data["city"],
        "zip_code": original_data["zip_code"],
        "province": original_data["province"],
        "country": original_data["country"],
        "contact_name": original_data["contact_name"],
        "phonenumber": original_data["phonenumber"],
        "reference": original_data["reference"],
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated supplier using PUT request
    put_response = requests.put(f"{url}/suppliers/{supplier_id}", json=updated_supplier, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the name
    get_response = requests.get(f"{url}/suppliers/{supplier_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["name"] == "Updated Supplier"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/suppliers/{supplier_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test to DELETE a supplier by its ID
def test_delete_supplier_item(api_data):
    url, api_key = api_data
    new_supplier = {
        "id": 9999,
        "code": "SUPP9999",
        "name": "Test Supplier",
        "address": "123 Test St",
        "address_extra": "Suite 100",
        "city": "Testville",
        "zip_code": "12345",
        "province": "Test Province",
        "country": "Testland",
        "contact_name": "John Doe",
        "phonenumber": "123-456-7890",
        "reference": "REF123",
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the supplier (POST)
    post_response = requests.post(f"{url}/suppliers", json=new_supplier, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Now delete the added supplier (DELETE)
    delete_response = requests.delete(f"{url}/suppliers/{new_supplier['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test GET with invalid supplier ID
def test_get_supplier_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/suppliers/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty supplier ID
def test_get_supplier_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/suppliers/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative supplier ID
def test_get_supplier_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/suppliers/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400
