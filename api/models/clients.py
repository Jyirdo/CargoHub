import json
import os
from models.base import Base

CLIENTS = []


class Clients(Base):
    def __init__(self, root_path, is_debug=False):
        # Correct path to `clients.json` in the `data` folder
        self.data_path = os.path.join(root_path, "../data/clients.json")
        print(f"Initializing Clients: data_path set to {self.data_path}")
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
            
#def get_clients(self):
#    """Fetch all clients from the file (always up-to-date)."""
#    self.load(is_debug=False)  # Always reload from file
#    return self.data

#def get_client(self, client_id):
#    """Fetch a single client by ID (always up-to-date)."""
#    self.load(is_debug=False)  # Always reload from file
#    for client in self.data:
#        if client["id"] == client_id:
#            return client
#    return None

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

    def remove_client(self, client_id, dry_run=False):
        """Simulate or perform deletion of a client by ID."""
        client_to_remove = None
        for client in self.data:
            if client["id"] == client_id:
                client_to_remove = client
                break

        if client_to_remove:
            if dry_run:
                return {"message": f"Client with ID {client_id} would be removed (dry-run mode)."}, 200
            else:
                # Remove client from memory
                self.data.remove(client_to_remove)  
                self.save()  # Save to clients.json
                self.load(is_debug=False)  # Manually reload data to refresh in-memory state
                return {"message": f"Client with ID {client_id} successfully removed."}, 200
        else:
            return {"error": f"Client with ID {client_id} not found."}, 404

    def load(self, is_debug):
        if is_debug:
            self.data = CLIENTS
        else:
            try:
                with open(self.data_path, "r") as f:
                    self.data = json.load(f)
            except FileNotFoundError:
                print(f"File {self.data_path} not found. Initializing empty data.")
                self.data = []
            except json.JSONDecodeError:
                print(f"Error decoding JSON from {self.data_path}. Initializing empty data.")
                self.data = []


    def save(self):
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)
            print(f"Successfully saved data to {self.data_path}")
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")
