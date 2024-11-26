import json
import os

from models.base import Base

ITEMS = []


class Items(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/items.json")
        self.load(is_debug)  # Load data on initialization

    def get_items(self):
        return self.data

    def get_item(self, item_id):
        for x in self.data:
            if x["uid"] == item_id:
                return x
        return None
    
    def get_item_data(self, item_id, data_type):
        for x in self.data:
            if x["uid"] == item_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def get_items_for_item_line(self, item_line_id):
        result = []
        for x in self.data:
            if x["item_line"] == item_line_id:
                result.append(x)
        return result

    def get_items_for_item_group(self, item_group_id):
        result = []
        for x in self.data:
            if x["item_group"] == item_group_id:
                result.append(x)
        return result

    def get_items_for_item_type(self, item_type_id):
        result = []
        for x in self.data:
            if x["item_type"] == item_type_id:
                result.append(x)
        return result

    def get_items_for_supplier(self, supplier_id):
        result = []
        for x in self.data:
            if x["supplier_id"] == supplier_id:
                result.append(x)
        return result

    def add_item(self, item):
        item["created_at"] = self.get_timestamp()
        item["updated_at"] = self.get_timestamp()
        self.data.append(item)

    def update_item(self, item_id, item):
        item["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["uid"] == item_id:
                self.data[i] = item
                break

    def remove_item(self, item_id, dry_run=False):
        """Simulate or perform deletion of an item by uid."""
        item_to_remove = None
        for item in self.data:
            if item["uid"] == item_id:
                item_to_remove = item
                break

        if item_to_remove:
            if dry_run:
                return {"message": f"Item with UID {item_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(item_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to items.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Item with UID {item_id} successfully removed."}, 200
        else:
            return {"error": f"Item with UID {item_id} not found."}, 404

    def load(self, is_debug=False):
        """Load items from the JSON file."""
        if is_debug:
            self.data = ITEMS
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
        """Save items back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")
