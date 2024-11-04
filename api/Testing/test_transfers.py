import pytest
import requests

# Setup common API info using a fixture
@pytest.fixture
def api_data():
    url = 'http://localhost:3000/api/v1'
    api_key = 'a1b2c3d4e5'
    return url, api_key

# Test to GET all transfers
def test_get_all_transfers(api_data):
    url, api_key = api_data
    response = requests.get(f"{url}/transfers", headers={"API_KEY": api_key})
    
    assert response.status_code == 200
    transfers = response.json()

    assert isinstance(transfers, list)  # Check if the result is a list
    for transfers in transfers:
        # Ensure that key fields are present in each transfers
        assert "id" in transfers
        assert "reference" in transfers
        assert "transfer_from" in transfers
        assert "transfer_to" in transfers
        assert "transfer_status" in transfers
        assert "created_at" in transfers
        assert "updated_at" in transfers
        assert "items" in transfers