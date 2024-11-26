import json
import os

from models.base import Base

SUPPLIERS = []


class Suppliers(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/suppliers.json")
        self.load(is_debug)  # Load data on initialization


    def get_suppliers(self):
        return self.data

    def get_supplier(self, supplier_id):
        for x in self.data:
            if x["id"] == supplier_id:
                return x
        return None
    
    def get_supplier_data(self, supplier_id, data_type):
        for x in self.data:
            if x["id"] == supplier_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def add_supplier(self, supplier):
        supplier["created_at"] = self.get_timestamp()
        supplier["updated_at"] = self.get_timestamp()
        self.data.append(supplier)

    def update_supplier(self, supplier_id, supplier):
        for i in range(len(self.data)):
            if self.data[i]["id"] == supplier_id:
                self.data[i] = supplier
                break

    def remove_supplier(self, supplier_id, dry_run=False):
        """Simulate or perform deletion of a supplier by ID."""
        supplier_to_remove = None
        for supplier in self.data:
            if supplier["id"] == supplier_id:
                supplier_to_remove = supplier
                break

        if supplier_to_remove:
            if dry_run:
                return {"message": f"Supplier with ID {supplier_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(supplier_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to suppliers.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Supplier with ID {supplier_id} successfully removed."}, 200
        else:
            return {"error": f"Supplier with ID {supplier_id} not found."}, 404

    def load(self, is_debug=False):
        """Load suppliers from the JSON file."""
        if is_debug:
            self.data = SUPPLIERS
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
        """Save suppliers back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")