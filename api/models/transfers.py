import json
import os

from models.base import Base

TRANSFERS = []


class Transfers(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = os.path.join(root_path, "../data/transfers.json")
        self.load(is_debug)  # Load data on initialization


    def get_transfers(self):
        return self.data

    def get_transfer(self, transfer_id):
        for x in self.data:
            if x["id"] == transfer_id:
                return x
        return None

    def get_transfer_data(self, transfer_id, data_type):
        for x in self.data:
            if x["id"] == transfer_id:
                if data_type in x:
                    return x[data_type]
                else:
                    return None

    def get_items_in_transfer(self, transfer_id):
        for x in self.data:
            if x["id"] == transfer_id:
                return x["items"]
        return None

    def add_transfer(self, transfer):
        transfer["transfer_status"] = "Scheduled"
        transfer["created_at"] = self.get_timestamp()
        transfer["updated_at"] = self.get_timestamp()
        self.data.append(transfer)

    def update_transfer(self, transfer_id, transfer):
        for i in range(len(self.data)):
            if self.data[i]["id"] == transfer_id:
                self.data[i] = transfer
                break

    def remove_transfer(self, transfer_id, dry_run=False):
        """Simulate or perform deletion of a transfer by ID."""
        transfer_to_remove = None
        for transfer in self.data:
            if transfer["id"] == transfer_id:
                transfer_to_remove = transfer
                break

        if transfer_to_remove:
            if dry_run:
                return {"message": f"Transfer with ID {transfer_id} would be removed (dry-run mode)."}, 200
            else:
                self.data.remove(transfer_to_remove)  # Remove from in-memory list
                self.save()  # Persist the change to transfers.json
                self.load(is_debug=False)  # Reload data from the file to reflect the changes
                return {"message": f"Transfer with ID {transfer_id} successfully removed."}, 200
        else:
            return {"error": f"Transfer with ID {transfer_id} not found."}, 404

    def load(self, is_debug=False):
        """Load transfers from the JSON file."""
        if is_debug:
            self.data = TRANSFERS
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
        """Save transfers back to the JSON file."""
        try:
            with open(self.data_path, "w") as f:
                json.dump(self.data, f, indent=4)  # Pretty print JSON
        except Exception as e:
            print(f"Error saving data to {self.data_path}: {e}")