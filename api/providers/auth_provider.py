USERS = [
    {
        "api_key": "a1b2c3d4e5",
        "app": "CargoHUB Dashboard 1",
        "endpoint_access": {
            "full": True
        }
    },
    {
        "api_key": "f6g7h8i9j0",
        "app": "CargoHUB Dashboard 2",
        "endpoint_access": {
            "full": False,
            "warehouses": {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "locations":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "transfers":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "items":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "item_lines":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "item_groups":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "item_types":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "inventories":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "suppliers":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "orders":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "clients":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            },
            "shipments":  {
                "full": False,
                "get": True,
                "post": False,
                "put": False,
                "delete": True
            }
        }
    }
]

_users = None

def init():
    global _users
    _users = USERS

def get_user(api_key):
    for user in _users:
        if user["api_key"] == api_key:
            return user
    return None
 
def has_access(user, path, method):
    access = user["endpoint_access"]
    if access["full"]:
        return True
    else:
        return access[path[0]][method]