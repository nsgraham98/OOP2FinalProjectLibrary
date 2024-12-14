using OOP2FinalProjectLibrary.Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
    public class ItemInRentalManager
    {
		public static List<ItemInRental> itemInRentals;
		private readonly DBHandler _dbHan;

		public ItemInRentalManager(DBHandler dbHan)
		{
			_dbHan = dbHan;
		}

		public string AddItemInRental(string itemStatus, params object[] additionalParam)
		{
			if (additionalParam.Length < 3)
			{
				return "Error: Missing additional parameters for Rental.";
			}

			if (!int.TryParse(additionalParam[0] as string, out int rentalId))
			{
				return "ERRPR: Invalid RentalID.";
			}

			if (!int.TryParse(additionalParam[1] as string, out int itemId))
			{
				return "Error: Invalid ItemID.";
			}

			if (!int.TryParse(additionalParam[2] as string, out int itemNum))

			{
				return "Error: Invalid Item Number.";
			}

			try
			{
				_dbHan.InsertItemInRentalDB(rentalId, itemId, itemNum, itemStatus);
				return "Item in rental Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"An error occurred while adding the item in rental: {ex.Message}";
			}
		}

		public string UpdateItem(ItemInRental ir)
		{
			if (ir == null)
			{
				return "ERROT: Item in rental cannot be null.";
			}

			try
			{
				return _dbHan.UpdateItemInRentalDB(ir);
			}
			catch (Exception ex)
			{
				return $"An error occurred while updating the Item in rental: {ex.Message}";
			}
		}

		public string DeleteItem(ItemInRental ir)
		{
			if (ir == null)
			{
				return "Error: Item in rental cannot be null.";
			}

			try
			{
				return _dbHan.DeleteItemInRentalDB(ir.ItemId, ir.RentalId);
			}

			catch (Exception ex)
			{
				return $"An error occurred while deleting the Rental: {ex.Message}";
			}
		}

		public List<ItemInRental> LoadAllItems()
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<ItemInRental>().ToList();
		}

		public ItemInRental LoadItemById(int RentalId)
		{
			return _dbHan.LoadTypedItemsFromDB().OfType<ItemInRental>().FirstOrDefault(ir => ir.RentalId == RentalId);
		}
	}
}
