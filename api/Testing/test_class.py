import requests

class TestClass:
    def test_get_warehouse_authorised(_url):
        url = 'http://localhost:3000/api/v1/' + 'orders'
        header = {'API_KEY': "a1b2c3d4e5"}
        status_code = requests.get(url, headers=header).status_code
        assert status_code == 200

    def test_create_resource_success(self):
        url = 'http://localhost:3000/api/v1/orders'
        headers = {'API_KEY': "a1b2c3d4e5", 'Content-Type': 'application/json'}
        payload = {
            "order_id": 67890,
            "customer_id": 12345,
            "items": [{"item_id": 2, "quantity": 1}]
        }
        response = requests.post(url, json=payload, headers=headers)
        assert response.status_code == 201

    def test_get_warehouse_unauthorised(_url):
        url = 'http://localhost:3000/api/v1/' + 'warehouses'
        parameter = {'id': 1}
        status_code = requests.get(url, params=parameter).status_code
        assert status_code == 401

    def test_get_not_found(_url):
        url = 'http://localhost:3000/api/v1/' + 'order'
        header = {'API_KEY': "a1b2c3d4e5"}
        status_code = requests.get(url, headers=header).status_code
        assert status_code == 404

    def test_get_forbidden_access(self):
        url = 'http://localhost:3000/api/v1/secure-data'
        header = {'API_KEY': "invalid_api_key"}
        status_code = requests.get(url, headers=header).status_code
        assert status_code == 403

    def test_internal_server_error(self):
        url = 'http://localhost:3000/api/v1/orders'
        headers = {'API_KEY': "a1b2c3d4e5", 'Content-Type': 'application/json'}
        payload = {
            "customer_id": 12345,
            "items": "invalid_value"
        }
        response = requests.post(url, json=payload, headers=headers)
        assert response.status_code == 500