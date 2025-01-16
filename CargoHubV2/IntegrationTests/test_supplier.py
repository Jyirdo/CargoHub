import pytest
import requests

BASE_URL = "http://localhost:5000/api/Supplier"  

# Fixture voor headers
@pytest.fixture
def headers():
    return {
        "Content-Type": "application/json"
    }

# Fixture voor een voorbeeldleverancier
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
    url = f"{BASE_URL}"
    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    assert isinstance(response.json(), list)

# Test: Get Supplier By ID
def test_get_supplier_by_id(headers):
    supplier_id = 1  # Vervang door een bestaande ID
    url = f"{BASE_URL}/{supplier_id}"

    response = requests.get(url, headers=headers)

    assert response.status_code == 200
    if response.status_code == 200:
        assert "name" in response.json()

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
    url = f"{BASE_URL}/CheckDuplicate"

    # Eerste poging
    requests.post(url, json=sample_supplier, headers=headers)

    # Tweede poging (duplicaatcheck)
    response = requests.post(url, json=sample_supplier, headers=headers)
    assert response.status_code == 200
    assert response.json() is True  # Geeft aan dat duplicaat bestaat

# Test: Delete Batch of Suppliers
def test_delete_batch_of_suppliers(headers):
    supplier_ids = [1, 2, 3]  # Vervang met echte IDs
    url = f"{BASE_URL}/DeleteBatch"

    response = requests.delete(url, json=supplier_ids, headers=headers)

    assert response.status_code in [200, 204]
