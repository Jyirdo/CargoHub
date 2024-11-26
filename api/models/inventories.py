import json
import os
from models.base import Base

INVENTORIES = []


class Inventories(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/inventories.json")
        self.load(is_debug)  # Load data on initialization

    def get_inventories(self):
        return self.data

    def get_inventory(self, inventory_id):
        for x in self.data:
            if x["id"] == inventory_id:
                return x
        return None
    
    def get_inventory_data(self, inventory_id, data_type):
        for x in self.data:
            if x["id"] == inventory_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def get_inventories_for_item(self, item_id):
        result = []
        for x in self.data:
            if x["item_id"] == item_id:
                result.append(x)
        return result

    def get_inventory_totals_for_item(self, item_id):
        result = {
            "total_expected": 0,
            "total_ordered": 0,
            "total_allocated": 0,
            "total_available": 0
        }
        for x in self.data:
            if x["item_id"] == item_id:
                result["total_expected"] += x["total_expected"]
                result["total_ordered"] += x["total_ordered"]
                result["total_allocated"] += x["total_allocated"]
                result["total_available"] += x["total_available"]
        return result

    def add_inventory(self, inventory):
        inventory["created_at"] = self.get_timestamp()
        inventory["updated_at"] = self.get_timestamp()
        self.data.append(inventory)

    def update_inventory(self, inventory_id, inventory):
        inventory["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == inventory_id:
                self.data[i] = inventory
                break

    def remove_inventory(self, inventory_id, dry_run=False):
        """Simulate or perform deletion of an inventory by ID."""
        inventory_to_remove = None
        for inventory in self.data:
            if inventory["id"] == inventory_id:
                inventory_to_remove = inventory
                break

        if inventory_to_remove:
            if dry_run:
                return {"message": f"Inventory with ID {inventory_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(inventory_to_remove)  # Remove from in-memory data
                self.save()  # Persist the changes to inventories.json
                self.load(is_debug=False)  # Reload data to reflect the changes
                return {"message": f"Inventory with ID {inventory_id} successfully removed."}, 200
        else:
            return {"error": f"Inventory with ID {inventory_id} not found."}, 404

    def load(self, is_debug=False):
        """Load inventories from the JSON file."""
        if is_debug:
            self.data = INVENTORIES
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
        """Save inventories back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")