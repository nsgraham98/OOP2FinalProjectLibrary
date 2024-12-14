using OOP2FinalProjectLibrary.Data.Objects.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
	public class CdManager : IItemManager<Cd>

	{
		public static List<Cd> cds;
		private readonly DBHandler _dbHan;

		public CdManager(DBHandler dbHan)
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

			string artist = additionalParam[0] as string;
			string label = additionalParam[1] as string;
			string duration = additionalParam[2] as string;

			try
			{
				_dbHan.InsertCdDB(
					title,
					category,
					publisher,
					genre,
					location,
					status,
					replaceCost,
					pubDate,
					artist,
					label,
					duration);
				return "CD Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"ERROR: CD cannot be null - {ex.Message}";
			}
		}

		public string UpdateItem(Cd cd)
		{
			if (cd == null)
			{
				return "Error: CD cannot be null.";
			}

			try
			{
				return _dbHan.UpdateCdDB(cd);
			}
			catch (Exception ex)
			{
				return $"An error occurred while updating the CD: {ex.Message}";
			}
		}

		public string DeleteItem(Cd cd)
		{
			if (cd == null)
			{
				return "Error: CD cannot be null.";
			}

			try
			{
				return _dbHan.DeleteCdDB(cd);
			}
			catch (Exception ex)
			{
				return $"An error occurred while deleting the CD: {ex.Message}";
			}
		}

		public List<Cd> LoadAllItems()
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Cd>().ToList();
		}

		public Cd LoadItemById(int itemId)
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Cd>().FirstOrDefault(cd => cd.ItemId == itemId);
		}

        public List<Cd> SearchItem(string searchField, bool isTitle, bool isCategory, bool isStatus, bool isLocation)
        {
            var allCds = LoadAllItems();
            var filteredCds = allCds.Where(cd =>
                (isTitle && cd.Title.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isCategory && cd.Category.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isStatus && cd.Status.Contains(searchField, StringComparison.OrdinalIgnoreCase)) ||
                (isLocation && cd.Location.Contains(searchField, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            return filteredCds;
        }
    }
}