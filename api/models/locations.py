import json
import os

from models.base import Base

LOCATIONS = []


class Locations(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/locations.json")
        self.load(is_debug)  # Load data on initialization
    def get_locations(self):
        return self.data

    def get_location(self, location_id):
        for x in self.data:
            if x["id"] == location_id:
                return x
        return None
    
    def get_location_data(self, location_id, data_type):
        for x in self.data:
            if x["id"] == location_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def get_locations_in_warehouse(self, warehouse_id):
        result = []
        for x in self.data:
            if x["warehouse_id"] == warehouse_id:
                result.append(x)
        return result

    def add_location(self, location):
        location["created_at"] = self.get_timestamp()
        location["updated_at"] = self.get_timestamp()
        self.data.append(location)

    def update_location(self, location_id, location):
        location["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == location_id:
                self.data[i] = location
                break

    def remove_location(self, location_id, dry_run=False):
        """Simulate or perform deletion of a location by ID."""
        location_to_remove = None
        for location in self.data:
            if location["id"] == location_id:
                location_to_remove = location
                break

        if location_to_remove:
            if dry_run:
                return {"message": f"Location with ID {location_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(location_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to locations.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Location with ID {location_id} successfully removed."}, 200
        else:
            return {"error": f"Location with ID {location_id} not found."}, 404

    def load(self, is_debug=False):
        """Load locations from the JSON file."""
        if is_debug:
            self.data = LOCATIONS
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
        """Save locations back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")