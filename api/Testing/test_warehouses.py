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

# Test to GET a specific warehouse by its ID
def test_get_warehouse_by_id(api_data):
    url, api_key = api_data
    warehouse_id = 1
    response = requests.get(f"{url}/warehouses/{warehouse_id}", headers={"API_KEY": api_key})

    assert response.status_code == 200
    warehouse = response.json()

    assert warehouse["id"] == warehouse_id
    assert "code" in warehouse
    assert "name" in warehouse
    assert "address" in warehouse
    assert "zip" in warehouse
    assert "city" in warehouse
    assert "province" in warehouse
    assert "country" in warehouse
    assert "contact" in warehouse
    assert "created_at" in warehouse
    assert "updated_at" in warehouse

    contact = warehouse["contact"]
    assert isinstance(contact, dict)
    assert "name" in contact
    assert "phone" in contact
    assert "email" in contact

# Test to ADD a new warehouse and DELETE it afterwards
def test_add_and_delete_warehouse(api_data):
    url, api_key = api_data
    new_warehouse = {
        "id": 9999,
        "code": "WH9999",
        "name": "Test Warehouse",
        "address": "123 Test St",
        "zip": "12345",
        "city": "Test City",
        "province": "Test Province",
        "country": "Testland",
        "contact": {
            "name": "Test Contact",
            "phone": "123-456-7890",
            "email": "test.contact@example.com"
        },
        "created_at": "2024-01-01 12:00:00",
        "updated_at": "2024-01-01 12:00:00"
    }

    # Add the warehouse
    post_response = requests.post(f"{url}/warehouses", json=new_warehouse, headers={"API_KEY": api_key})
    assert post_response.status_code == 201

    # Delete the newly added warehouse
    delete_response = requests.delete(f"{url}/warehouses/{new_warehouse['id']}", headers={"API_KEY": api_key})
    assert delete_response.status_code == 200

# Test to UPDATE an existing warehouse
def test_update_warehouse(api_data):
    url, api_key = api_data
    warehouse_id = 1

    # Fetch current warehouse data to update
    original_data = requests.get(f"{url}/warehouses/{warehouse_id}", headers={"API_KEY": api_key}).json()

    updated_warehouse = {
        "id": warehouse_id,
        "code": original_data["code"],
        "name": "Updated Warehouse",
        "address": original_data["address"],
        "zip": original_data["zip"],
        "city": original_data["city"],
        "province": original_data["province"],
        "country": original_data["country"],
        "contact": {
            "name": "Updated Contact",
            "phone": original_data["contact"]["phone"],
            "email": original_data["contact"]["email"]
        },
        "created_at": original_data["created_at"],
        "updated_at": "2024-01-01 12:00:00"
    }

    # Send the updated warehouse using PUT request
    put_response = requests.put(f"{url}/warehouses/{warehouse_id}", json=updated_warehouse, headers={"API_KEY": api_key})
    assert put_response.status_code == 200

    # Verify that the update happened by checking the name
    get_response = requests.get(f"{url}/warehouses/{warehouse_id}", headers={"API_KEY": api_key})
    assert get_response.status_code == 200
    assert get_response.json()["name"] == "Updated Warehouse"
    assert get_response.json()["contact"]["name"] == "Updated Contact"

    # Restore original data to keep things clean
    restore_response = requests.put(f"{url}/warehouses/{warehouse_id}", json=original_data, headers={"API_KEY": api_key})
    assert restore_response.status_code == 200

# Test GET with invalid warehouse ID
def test_get_warehouse_invalid_id(api_data):
    url, api_key = api_data
    invalid_id = "invalid_id"  # Use an invalid ID

    response = requests.get(f"{url}/warehouses/{invalid_id}", headers={"API_KEY": api_key})
    
    # For invalid IDs, expect a 400 Bad Request or other error response
    assert response.status_code == 400 or response.status_code == 404

# Test GET with an empty warehouse ID
def test_get_warehouse_empty_id(api_data):
    url, api_key = api_data
    empty_id = ""  # Empty ID

    response = requests.get(f"{url}/warehouses/{empty_id}", headers={"API_KEY": api_key})

    # Expecting a 400 Bad Request for empty ID
    assert response.status_code == 400

# Test GET with a negative warehouse ID
def test_get_warehouse_negative_id(api_data):
    url, api_key = api_data
    negative_id = -1  # Negative ID

    response = requests.get(f"{url}/warehouses/{negative_id}", headers={"API_KEY": api_key})

    # Expect a 400 Bad Request for invalid negative ID
    assert response.status_code == 400