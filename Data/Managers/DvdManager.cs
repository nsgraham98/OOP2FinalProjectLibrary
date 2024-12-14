using OOP2FinalProjectLibrary.Data.Objects.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
    public class DvdManager : IItemManager<Dvd>
    {
		public static List<Dvd> dvds;
		private readonly DBHandler _dbHan;

		public DvdManager(DBHandler dbHan)
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

				string director = additionalParam[0] as string;
				string duration = additionalParam[1] as string;
				string format = additionalParam[2] as string;

			try
			{
				_dbHan.InsertDvdDB(
					title,
					category,
					publisher,
					genre,
					location,
					status,
					replaceCost,
					pubDate,
					director,
					duration,
					format);

				return "Dvd Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"ERROR: Dvd cannot be null - {ex.Message}";
			}
		}

		public string UpdateItem(Dvd d)
		{
			if (d == null)
			{
				return "ERROR! DVD cannot be null.";
			}

			try
			{
				return _dbHan.UpdateDvdDB(d);
			}

			catch (Exception ex)
			{
				return $"An error occurred while updating the DVD: {ex.Message}";

			}
		}

		public string DeleteItem(Dvd d)
		{
			if (d == null)
			{
				return "Error: DVD cannot be null.";
			}

			try
			{
				return _dbHan.DeleteDvdDB(d);
			}
			catch (Exception ex)
			{
				return $"An error occurred while deleting the DVD: {ex.Message}";
			}
		}

		public List<Dvd> LoadAllItems()
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Dvd>().ToList();
		}

		public Dvd LoadItemById(int itemId)
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Dvd>().FirstOrDefault(d => d.ItemId == itemId);
		}
	}
}

