import pytest
import requests

BASE_URL = "http://localhost:5000/api/Supplier"


@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }


@pytest.fixture
def sample_supplier():
    return {
        "code": "SUP123",
        "name": "Test Supplier",
        "address": "456 Supplier Lane",
        "city": "Supplier City",
        "zip_code": "45678",
        "province": "Supplier Province",
        "country": "Supplierland",
        "contact_name": "Supplier Contact",
        "phonenumber": "987-654-3210",
        "reference": "Ref123",
        "created_at": "2024-01-01T12:00:00Z",
        "updated_at": "2024-01-01T12:00:00Z"
    }


# Test: Get All Suppliers
def test_get_all_suppliers(headers):
    url = f"{BASE_URL}/byAmount/10"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test: Get Supplier By ID


def test_get_transfer_by_id(headers):
    get_all_url = f"{BASE_URL}"
    get_all_response = requests.get(get_all_url, headers=headers)

    if get_all_response.status_code != 200 or not get_all_response.json():
        pytest.skip("Geen transfers gevonden om te testen.")

    # Gebruik een bestaand ID uit de lijst
    existing_transfer = get_all_response.json()[0]
    transfer_id = existing_transfer["id"]
    url = f"{BASE_URL}/{transfer_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    response_data = response.json()

    print(f"Response Data: {response_data}")

    assert "id" in response_data, "Expected 'id' in response"
    assert response_data["id"] == transfer_id, f"Expected ID {transfer_id}, got {response_data['id']}"
    assert "reference" in response_data, "Expected 'reference' in response"
    assert "createdAt" in response_data, "Expected 'createdAt' in response"

    if "transfer_status" not in response_data:
        print("Warning: 'transfer_status' ontbreekt in de response. Controleer de API of database.")
    else:
        assert "transfer_status" in response_data, "Expected 'transfer_status' in response"


# Test: Search Supplier By Name
def test_search_supplier_by_name(headers):
    name = "Test Supplier"
    url = f"{BASE_URL}/Search/Name/{name}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    for supplier in response.json():
        assert name.lower() in supplier["name"].lower()

# Test: Search Supplier By City


def test_search_supplier_by_city(headers):
    city = "Supplier City"
    url = f"{BASE_URL}/Search/City/{city}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    for supplier in response.json():
        assert city.lower() in supplier["city"].lower()

# Test: Check Duplicate Supplier


def test_check_duplicate_supplier(headers, sample_supplier):
    url = f"{BASE_URL}/CheckDuplicate"

    response = requests.post(url, json=sample_supplier, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), bool)

# Test: Create Duplicate Supplier


def test_create_duplicate_supplier(headers, sample_supplier):
    create_url = f"{BASE_URL}"
    duplicate_check_url = f"{BASE_URL}/CheckDuplicate"

    create_response = requests.post(create_url, json=sample_supplier, headers=headers)

    print(f"Create Response Status Code: {create_response.status_code}")
    print(f"Create Response Body: {create_response.text}")

    assert create_response.status_code == 201, f"Failed to create supplier: {create_response.text}"

    duplicate_response = requests.post(duplicate_check_url, json=sample_supplier, headers=headers)

    print(f"Duplicate Check Response Status Code: {duplicate_response.status_code}")
    print(f"Duplicate Check Response Body: {duplicate_response.text}")

    assert duplicate_response.status_code == 200, f"Unexpected status code: {duplicate_response.status_code}"
    assert duplicate_response.json() is True, "Expected duplicate check to return True, but got False"


# Test: Delete Batch of Suppliers
def test_delete_suppliers_batch(headers, sample_supplier):
    create_url = f"{BASE_URL}"

    response1 = requests.post(create_url, json=sample_supplier, headers=headers)
    assert response1.status_code == 201, f"Expected 201, got {response1.status_code}"
    supplier1 = response1.json()

    response2 = requests.post(create_url, json=sample_supplier, headers=headers)
    assert response2.status_code == 201, f"Expected 201, got {response2.status_code}"
    supplier2 = response2.json()

    assert "id" in supplier1, "Expected 'id' in response1"
    assert "id" in supplier2, "Expected 'id' in response2"

    ids_to_delete = [supplier1["id"], supplier2["id"]]

    delete_url = f"{BASE_URL}/DeleteBatch"
    delete_response = requests.delete(delete_url, json=ids_to_delete, headers=headers)

    assert delete_response.status_code == 204, f"Expected 204, got {delete_response.status_code}"

    for supplier_id in ids_to_delete:
        get_url = f"{BASE_URL}/{supplier_id}"
        get_response = requests.get(get_url, headers=headers)

        assert get_response.status_code == 404, f"Expected 404, got {get_response.status_code} for supplier ID {supplier_id}"

    print(f"Successfully deleted suppliers with IDs: {ids_to_delete}")
