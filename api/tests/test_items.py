import pytest
"""
This module contains unit tests for the Items class in the api.models.items module.
Fixtures:
    items: A pytest fixture that patches the built-in open function and the Base.get_timestamp method,
           and returns an instance of the Items class.
Tests:
    test_get_items: Tests that the get_items method returns the correct number of items and the correct item data.
    test_get_item: Tests that the get_item method returns the correct item data for a given UID.
    test_get_items_for_item_line: Tests that the get_items_for_item_line method returns the correct items for a given item line.
    test_get_items_for_item_group: Tests that the get_items_for_item_group method returns the correct items for a given item group.
    test_get_items_for_item_type: Tests that the get_items_for_item_type method returns the correct items for a given item type.
    test_get_items_for_supplier: Tests that the get_items_for_supplier method returns the correct items for a given supplier ID.
    test_add_item: Tests that the add_item method correctly adds a new item and that the item can be retrieved.
    test_update_item: Tests that the update_item method correctly updates an existing item.
    test_remove_item: Tests that the remove_item method correctly removes an item.
    test_save: Tests that the save method correctly writes the items to a file.
"""
from unittest.mock import patch, mock_open
from api.models.items import Items

@pytest.fixture
@patch("builtins.open", new_callable=mock_open, read_data='[{"uid": "1", "item_line": "line1", "item_group": "group1", "item_type": "type1", "supplier_id": "supplier1"}]')
@patch("api.models.items.Base.get_timestamp", return_value="2023-01-01T00:00:00Z")
def items(mock_get_timestamp, mock_open):
    return Items(root_path="", is_debug=False)

def test_get_items(items):
    assert len(items.get_items()) == 1
    assert items.get_items()[0]["uid"] == "1"

def test_get_item(items):
    item = items.get_item("1")
    assert item is not None
    assert item["uid"] == "1"

def test_get_items_for_item_line(items):
    items_list = items.get_items_for_item_line("line1")
    assert len(items_list) == 1
    assert items_list[0]["item_line"] == "line1"

def test_get_items_for_item_group(items):
    items_list = items.get_items_for_item_group("group1")
    assert len(items_list) == 1
    assert items_list[0]["item_group"] == "group1"

def test_get_items_for_item_type(items):
    items_list = items.get_items_for_item_type("type1")
    assert len(items_list) == 1
    assert items_list[0]["item_type"] == "type1"

def test_get_items_for_supplier(items):
    items_list = items.get_items_for_supplier("supplier1")
    assert len(items_list) == 1
    assert items_list[0]["supplier_id"] == "supplier1"

def test_add_item(items):
    new_item = {"uid": "2", "item_line": "line2", "item_group": "group2", "item_type": "type2", "supplier_id": "supplier2"}
    items.add_item(new_item)
    assert len(items.get_items()) == 2
    assert items.get_item("2")["uid"] == "2"

def test_update_item(items):
    updated_item = {"uid": "1", "item_line": "line1_updated", "item_group": "group1", "item_type": "type1", "supplier_id": "supplier1"}
    items.update_item("1", updated_item)
    assert items.get_item("1")["item_line"] == "line1_updated"

def test_remove_item(items):
    items.remove_item("1")
    assert items.get_item("1") is None

@patch("builtins.open", new_callable=mock_open)
def test_save(mock_open, items):
    items.save()
    mock_open.assert_called_once_with("items.json", "w")
    handle = mock_open()
    handle.write.assert_called_once()
