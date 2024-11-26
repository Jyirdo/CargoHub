import json

from models.base import Base

WAREHOUSES = []


class Warehouses(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "warehouses.json"
        self.load(is_debug)

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
                self.data[i].update(warehouse)
                self.data[i]['updated_at'] = self.get_timestamp()
                break

    def remove_warehouse(self, warehouse_id):
        for x in self.data:
            if x["id"] == warehouse_id:
                self.data.remove(x)

    def load(self, is_debug):
        if is_debug:
            self.data = WAREHOUSES
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
