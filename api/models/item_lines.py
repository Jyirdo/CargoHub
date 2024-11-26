import json

from models.base import Base

ITEM_LINES = []


class ItemLines(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "item_lines.json"
        self.load(is_debug)

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

    def remove_item_line(self, item_line_id):
        for x in self.data:
            if x["id"] == item_line_id:
                self.data.remove(x)

    def load(self, is_debug):
        if is_debug:
            self.data = ITEM_LINES
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()

    def validate_item_line_data(self, item_line):
        required_fields = [
            "id", "name", "description"
        ]
        for field in required_fields:
            if field not in item_line:
                raise ValueError(f"Field '{field}' is missing in the item line data.")

    def add_item_line(self, item_line):
        self.validate_item_line_data(item_line)
        item_line["created_at"] = self.get_timestamp()
        item_line["updated_at"] = self.get_timestamp()
        self.data.append(item_line)
        self.save()
