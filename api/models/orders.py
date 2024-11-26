import json
import os

from models.base import Base
from providers import data_provider

ORDERS = []


class Orders(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/orders.json")
        self.load(is_debug)  # Load data on initialization


    def get_orders(self):
        return self.data

    def get_order(self, order_id):
        for x in self.data:
            if x["id"] == order_id:
                return x
        return None
    
    def get_order_data(self, order_id, data_type):
        for x in self.data:
            if x["id"] == order_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def get_orders_in_shipment(self, shipment_id):
        result = []
        for x in self.data:
            if x["shipment_id"] == shipment_id:
                result.append(x["id"])
        return result

    def get_orders_for_client(self, client_id):
        result = []
        for x in self.data:
            if x["ship_to"] == client_id or x["bill_to"] == client_id:
                result.append(x)
        return result

    def add_order(self, order):
        order["created_at"] = self.get_timestamp()
        order["updated_at"] = self.get_timestamp()
        self.data.append(order)

    def update_order(self, order_id, order):
        order["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == order_id:
                self.data[i] = order
                break

    def update_items_in_order(self, order_id, items):
        order = self.get_order(order_id)
        current = order["items"]
        for x in current:
            found = False
            for y in items:
                if x["item_id"] == y["item_id"]:
                    found = True
                    break
            if not found:
                inventories = data_provider.fetch_inventory_pool().get_inventories_for_item(x["item_id"])
                min_ordered = 1_000_000_000_000_000_000
                min_inventory
                for z in inventories:
                    if z["total_allocated"] > min_ordered:
                        min_ordered = z["total_allocated"]
                        min_inventory = z
                min_inventory["total_allocated"] -= x["amount"]
                min_inventory["total_expected"] = y["total_on_hand"] + y["total_ordered"]
                data_provider.fetch_inventory_pool().update_inventory(min_inventory["id"], min_inventory)
        for x in current:
            for y in items:
                if x["item_id"] == y["item_id"]:
                    inventories = data_provider.fetch_inventory_pool().get_inventories_for_item(x["item_id"])
                    min_ordered = 1_000_000_000_000_000_000
                    min_inventory
                    for z in inventories:
                        if z["total_allocated"] < min_ordered:
                            min_ordered = z["total_allocated"]
                            min_inventory = z
                min_inventory["total_allocated"] += y["amount"] - x["amount"]
                min_inventory["total_expected"] = y["total_on_hand"] + y["total_ordered"]
                data_provider.fetch_inventory_pool().update_inventory(min_inventory["id"], min_inventory)
        order["items"] = items
        self.update_order(order_id, order)

    def update_orders_in_shipment(self, shipment_id, orders):
        packed_orders = self.get_orders_in_shipment(shipment_id)
        for x in packed_orders:
            if x not in orders:
                order = self.get_order(x)
                order["shipment_id"] = -1
                order["order_status"] = "Scheduled"
                self.update_order(x, order)
        for x in orders:
            order = self.get_order(x)
            order["shipment_id"] = shipment_id
            order["order_status"] = "Packed"
            self.update_order(x, order)

    def remove_order(self, order_id, dry_run=False):
        """Simulate or perform deletion of an order by ID."""
        order_to_remove = None
        for order in self.data:
            if order["id"] == order_id:
                order_to_remove = order
                break

        if order_to_remove:
            if dry_run:
                return {"message": f"Order with ID {order_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(order_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to orders.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Order with ID {order_id} successfully removed."}, 200
        else:
            return {"error": f"Order with ID {order_id} not found."}, 404

    def load(self, is_debug=False):
        """Load orders from the JSON file."""
        if is_debug:
            self.data = ORDERS
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
        """Save orders back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")