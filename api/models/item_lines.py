import json
import os
from models.base import Base

ITEM_LINES = []


class ItemLines(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/item_lines.json")
        self.load(is_debug)  # Load data on initialization

    def get_item_lines(self):
        return self.data

    def get_item_line(self, item_line_id):
        for x in self.data:
            if x["id"] == item_line_id:
                return x
        return None
    
    def get_item_line_data(self, item_line_id, data_type):
        for x in self.data:
            if x["id"] == item_line_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def add_item_line(self, item_line):
        item_line["created_at"] = self.get_timestamp()
        item_line["updated_at"] = self.get_timestamp()
        self.data.append(item_line)

    def update_item_line(self, item_line_id, item_line):
        item_line["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == item_line_id:
                self.data[i] = item_line
                break

    def remove_item_line(self, item_line_id, dry_run=False):
        """Simulate or perform deletion of an item line by ID."""
        item_line_to_remove = None
        for item_line in self.data:
            if item_line["id"] == item_line_id:
                item_line_to_remove = item_line
                break

        if item_line_to_remove:
            if dry_run:
                return {"message": f"Item Line with ID {item_line_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(item_line_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to item_lines.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Item Line with ID {item_line_id} successfully removed."}, 200
        else:
            return {"error": f"Item Line with ID {item_line_id} not found."}, 404

    def load(self, is_debug=False):
        """Load item lines from the JSON file."""
        if is_debug:
            self.data = ITEM_LINES
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
        """Save item lines back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")