import json

from models.base import Base

SUPPLIERS = []


class Suppliers(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "suppliers.json"
        self.load(is_debug)

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
        supplier["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == supplier_id:
                self.data[i] = supplier
                break

    def remove_supplier(self, supplier_id):
        for x in self.data:
            if x["id"] == supplier_id:
                self.data.remove(x)

    def load(self, is_debug):
        if is_debug:
            self.data = SUPPLIERS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
    
    def validate_supplier_data(self, supplier):
        required_fields = [
            "id", "code", "name", "address", "address_extra", "city",
            "zip_code", "province", "country", "contact_name", 
            "phonenumber", "reference"
        ]
        for field in required_fields:
            if field not in supplier:
                raise ValueError(f"Field '{field}' is missing in the supplier data.")

    def add_supplier(self, supplier):
        self.validate_supplier_data(supplier)
        supplier["created_at"] = self.get_timestamp()
        supplier["updated_at"] = self.get_timestamp()
        self.data.append(supplier)
        self.save()
