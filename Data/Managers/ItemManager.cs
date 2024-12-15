using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOP2FinalProjectLibrary.Data.Objects.Items;

namespace OOP2FinalProjectLibrary.Data.Managers
{
    internal class ItemManager : IItemManager<Item>
    {
        public static List<Item> items;

        private readonly DBHandler _dbHan;

        public ItemManager(DBHandler dbHan)
        {
            _dbHan = dbHan;
        }

        public string AddItem(int itemId, string title, string category, string publisher, string genre,
            string location, string status, float replaceCost, DateTime pubDate,
            params object[] additionalParam)
        {
            if (additionalParam.Count() > 0)
            {
                return "Error";
            }
            try
            {
                Item item = _dbHan.InsertItemDB(
                    title,
                    category,
                    publisher,
                    genre,
                    location,
                    status,
                    replaceCost,
                    pubDate);
                return "Item Added Successfully!";
            }

            catch (Exception ex)
            {
                return $"ERROR: Item cannot be null - {ex.Message}";
            }
        }

        public string UpdateItem(Item b)
        {
            if (b == null)
            {
                return "ERROR! Item cannot be null.";
            }

            try
            {
                bool wasUpdated = _dbHan.UpdateItemDB(b);
                if (wasUpdated) {
                    return "Item Updated Succesfully";
                }
                else
                {
                    return $"An error occurred while updating the item";
                }
            }

            catch (Exception ex)
            {
                return $"An error occurred while updating the item: {ex.Message}";

            }
        }

        public string DeleteItem(Item b)
        {
            if (b == null)
            {
                return "ERROR! Item cannot be null.";
            }
            try
            {
                bool wasUpdated = _dbHan.DeleteItemDB(b);
                if (wasUpdated)
                {
                    return "Item Deleted Succesfully";
                }
                else
                {
                    return $"An error occurred while deleting the item";
                }
            }
            catch (Exception ex)
            {
                return $"An error occurred while deleting the item: {ex.Message}";
            }
        }

        public List<Item> LoadAllItems()
        {
            return _dbHan.LoadTypedItemsFromDB().OfType<Item>().ToList();
        }

        public Item LoadItemById(int itemId)
        {
            return _dbHan.LoadTypedItemsFromDB().OfType<Item>().FirstOrDefault(b => b.ItemId == itemId);
        }
        public List<Item> SearchItem(string searchField, bool isTitle, bool isCategory, bool isStatus, bool isLocation)
        {
            var allItems = LoadAllItems();
            var filteredItems = allItems.Where(items =>
                (isTitle && items.Title.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isCategory && items.Category.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isStatus && items.Status.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isLocation && items.Location.Contains(searchField, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            return filteredItems;
        }
    }   
}
