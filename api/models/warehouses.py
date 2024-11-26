import json
import os

from models.base import Base

WAREHOUSES = []


class Warehouses(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/warehouses.json")
        self.load(is_debug)  # Load data on initialization

    def get_warehouses(self):
        return self.data

    def get_warehouse(self, warehouse_id):
        for x in self.data:
            if x["id"] == warehouse_id:
                return x
        return None

    def get_warehouse_data(self, warehouse_id, data_type):
        for x in self.data:
            if x["id"] == warehouse_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def add_warehouse(self, warehouse):
        warehouse["created_at"] = self.get_timestamp()
        warehouse["updated_at"] = self.get_timestamp()
        self.data.append(warehouse)

    def update_warehouse(self, warehouse_id, warehouse):
        for i in range(len(self.data)):
            if self.data[i]["id"] == warehouse_id:
                self.data[i] = warehouse
                break

    def remove_warehouse(self, warehouse_id, dry_run=False):
        """Simulate or perform deletion of a warehouse by ID."""
        warehouse_to_remove = None
        for warehouse in self.data:
            if warehouse["id"] == warehouse_id:
                warehouse_to_remove = warehouse
                break

        if warehouse_to_remove:
            if dry_run:
                return {"message": f"Warehouse with ID {warehouse_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(warehouse_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to warehouses.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Warehouse with ID {warehouse_id} successfully removed."}, 200
        else:
            return {"error": f"Warehouse with ID {warehouse_id} not found."}, 404

    def load(self, is_debug=False):
        """Load warehouses from the JSON file."""
        if is_debug:
            self.data = WAREHOUSES
        else:
            try:
                with open(self.data_path, "r") as f:
                    self.data = json.load(f)
            except FileNotFoundError:
                print(f"File {self.data_path} not found. Initializing empty data.")
                self.data = []  # Initialize empty list if file is missing
            except json.JSONDecodeError:
                print(f"Error decoding JSON from {self.data_path}. Initializing empty data.")
                self.data = []

    def save(self):
        """Save warehouses back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")