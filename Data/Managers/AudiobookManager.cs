using OOP2FinalProjectLibrary.Data.Objects.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
    public class AudiobookManager : IItemManager<Audiobook>
    {
        public static List<Audiobook> audiobooks;

		private readonly DBHandler _dbHan;

		public AudiobookManager(DBHandler dbHan)
		{
			_dbHan = dbHan;
		}

		public string AddItem(int itemId, string title, string category, string publisher, string genre,
			string location, string status, float replaceCost, DateTime pubDate,
			params object[] additionalParam)
		{

			if (additionalParam.Length < 4)
			{
				return "Error: Missing additional parameters for CD.";
			}

			string isbn = additionalParam[0] as string;
			string author = additionalParam[1] as string;
			string duration = additionalParam[2] as string;
			string narrator = additionalParam[3] as string;

			try
			{
				_dbHan.InsertAudiobookDB(
					itemId,
					title,
					category,
					publisher,
					genre, location,
					status,
					replaceCost,
					pubDate,
					isbn,
					author,
					duration,
					narrator);
				return "Audiobook Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"ERROR: Audiobook cannot be null: {ex.Message}.";
			}
		

        }

		public string DeleteItem(Audiobook ab)
		{
			if (ab == null)
			{
				return "ERROR! Audiobook cannot be null.";
			}

			try
			{
				return _dbHan.DeleteAudiobookDB(ab);
			}
			catch (Exception ex)
			{
				return $"An error occurred while deleting the audiobook: {ex.Message}";
			}
		}

		public string UpdateItem(Audiobook ab)
		{
			if (ab == null)
			{
				return "Error: Audiobook cannot be null.";
			}

			try
			{
				return _dbHan.UpdateAudiobookDB(ab);
			}

			catch (Exception ex)
			{
				return $"An error occurred while updating the audiobook: {ex.Message}";

			}
		}

		public List<Audiobook> LoadAllItems()
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Audiobook>().ToList();
		}

		public Audiobook LoadItemById(int itemId)
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<Audiobook>().FirstOrDefault(ab => ab.ItemId == itemId);
		}
	}
}
