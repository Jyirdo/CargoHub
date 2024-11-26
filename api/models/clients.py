import json

from models.base import Base

CLIENTS = []


class Clients(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "clients.json"
        self.load(is_debug)

    def get_clients(self):
        return self.data

    def get_client(self, client_id):
        for x in self.data:
            if x["id"] == client_id:
                return x
        return None
    
    def get_client_data(self, client_id, data_type):
        for x in self.data:
            if x["id"] == client_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def add_client(self, client):
        client["created_at"] = self.get_timestamp()
        client["updated_at"] = self.get_timestamp()
        self.data.append(client)

    def update_client(self, client_id, client):
        client["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == client_id:
                self.data[i] = client
                break

    def remove_client(self, client_id):
        for x in self.data:
            if x["id"] == client_id:
                self.data.remove(x)

    def load(self, is_debug):
        if is_debug:
            self.data = CLIENTS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
        
    def validate_client_data(self, client):
        required_fields = [
            "id", "name", "address", "city", "zip_code", "province",
            "country", "contact_name", "contact_phone", "contact_email"
        ]
        for field in required_fields:
            if field not in client:
                raise ValueError(f"Het veld '{field}' ontbreekt in de gegevens van de client.")
    
    def add_client(self, client):
        self.validate_client_data(client)
        client["created_at"] = self.get_timestamp()
        client["updated_at"] = self.get_timestamp()
        self.data.append(client)
        self.save()
