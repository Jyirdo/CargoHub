import json
import os

from models.base import Base
from providers import data_provider

SHIPMENTS = []


class Shipments(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/shipments.json")
        self.load(is_debug)  # Load data on initialization

    def get_shipments(self):
        return self.data

    def get_shipment(self, shipment_id):
        for x in self.data:
            if x["id"] == shipment_id:
                return x
        return None

    def get_shipment_data(self, shipment_id, data_type):
        for x in self.data:
            if x["id"] == shipment_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def add_shipment(self, shipment):
        shipment["created_at"] = self.get_timestamp()
        shipment["updated_at"] = self.get_timestamp()
        self.data.append(shipment)

    def update_shipment(self, shipment_id, shipment):
        for i in range(len(self.data)):
            if self.data[i]["id"] == shipment_id:
                self.data[i].update(shipment)
                self.data[i]['updated_at'] = self.get_timestamp()
                break

    def update_items_in_shipment(self, shipment_id, items):
        shipment = self.get_shipment(shipment_id)
        current = shipment["items"]
        for x in current:
            found = False
            for y in items:
                if x["item_id"] == y["item_id"]:
                    found = True
                    break
            if not found:
                inventories = data_provider.fetch_inventory_pool().get_inventories_for_item(x["item_id"])
                max_ordered = -1
                max_inventory
                for z in inventories:
                    if z["total_ordered"] > max_ordered:
                        max_ordered = z["total_ordered"]
                        max_inventory = z
                max_inventory["total_ordered"] -= x["amount"]
                max_inventory["total_expected"] = y["total_on_hand"] + y["total_ordered"]
                data_provider.fetch_inventory_pool().update_inventory(max_inventory["id"], max_inventory)
        for x in current:
            for y in items:
                if x["item_id"] == y["item_id"]:
                    inventories = data_provider.fetch_inventory_pool().get_inventories_for_item(x["item_id"])
                    max_ordered = -1
                    max_inventory
                    for z in inventories:
                        if z["total_ordered"] > max_ordered:
                            max_ordered = z["total_ordered"]
                            max_inventory = z
                    max_inventory["total_ordered"] += y["amount"] - x["amount"]
                    max_inventory["total_expected"] = y["total_on_hand"] + y["total_ordered"]
                    data_provider.fetch_inventory_pool().update_inventory(max_inventory["id"], max_inventory)
        shipment["items"] = items
        self.update_shipment(shipment_id, shipment)

    def remove_shipment(self, shipment_id, dry_run=False):
        """Simulate or perform deletion of a shipment by ID."""
        shipment_to_remove = None
        for shipment in self.data:
            if shipment["id"] == shipment_id:
                shipment_to_remove = shipment
                break

        if shipment_to_remove:
            if dry_run:
                return {"message": f"Shipment with ID {shipment_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(shipment_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to shipments.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Shipment with ID {shipment_id} successfully removed."}, 200
        else:
            return {"error": f"Shipment with ID {shipment_id} not found."}, 404

    def load(self, is_debug=False):
        """Load shipments from the JSON file."""
        if is_debug:
            self.data = SHIPMENTS
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
        """Save shipments back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")