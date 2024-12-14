using OOP2FinalProjectLibrary.Data.Objects.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
    public class MagazineManager : IItemManager<Magazine>
    {
		public static List<Magazine> magazines;
		private readonly DBHandler _dbHan;

		public MagazineManager(DBHandler dbHan)
		{
			_dbHan = dbHan;
		}

		public string AddItem(int itemId, string title, string category, string publisher, string genre,
			string location, string status, float replaceCost, DateTime pubDate,
			params object[] additionalParam)
		{
			if (additionalParam.Length < 3)
			{
				return "Error: Missing additional parameters for CD.";
			}

			string issn = additionalParam[0] as string;
			string publication = additionalParam[1] as string;

			if (!DateTime.TryParse(additionalParam[2] as string, out DateTime coverDate))
			{
				return "Error: Invalid date format for cover date.";
			}

			try
			{
				_dbHan.InsertMagazineDB(
					title,
					category,
					publisher,
					genre,
					location,
					status,
					replaceCost,
					pubDate,
					issn,
					publication,
					coverDate
				);
				return "Magazine Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"An error occurred while adding the magazine: {ex.Message}";
			}
		}

		public string UpdateItem(Magazine m)
		{
			if (m == null)
			{
				return "Error: Magazine cannot be null.";
			}

			try
			{
				return _dbHan.UpdateMagazineDB(m);
			}
			catch (Exception ex)
			{
				return $"An error occurred while updating the Magazine: {ex.Message}";
			}
		}

		public string DeleteItem(Magazine m)
		{
			if (m == null)
			{
				return "Error: Magazine cannot be null.";
			}

			try
			{
				return _dbHan.DeleteMagazineDB(m);
			}
			catch (Exception ex)
			{
				return $"An error occurred while deleting the Magazine: {ex.Message}";
			}
		}

		public List<Magazine> LoadAllItems()
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Magazine>().ToList();
		}

		public Magazine LoadItemById(int itemId)
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Magazine>().FirstOrDefault(m => m.ItemId == itemId);
		}
        public List<Magazine> SearchItem(string searchField, bool isTitle, bool isCategory, bool isStatus, bool isLocation)
        {

            var allMags = LoadAllItems();
            var filteredMags = allMags.Where(magazines =>
                (isTitle && magazines.Title.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isCategory && magazines.Category.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isStatus && magazines.Status.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isLocation && magazines.Location.Contains(searchField, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            return filteredMags;
        }
    }
}
