using OOP2FinalProjectLibrary.Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
    public class RentalManager
    {
		public static List<Rental> rentals;
		private readonly DBHandler _dbHan;

		public RentalManager(DBHandler dbHan)
		{
			_dbHan = dbHan;
		}

		public string AddRental(string rentStatus, params object[] additionalParam)
		{
			if (additionalParam.Length < 5)
				return "Error: Missing additional parameters for Rental.";

			// Safely parse the additional parameters
			if (!int.TryParse(additionalParam[0] as string, out int rentalId))
			{
				return "Error: Invalid rental ID.";
			}

			if (!int.TryParse(additionalParam[1] as string, out int memberId))
			{ 
				return "Error: Invalid member ID."; 
			}

			if (!DateTime.TryParse(additionalParam[2] as string, out DateTime startDate))
			{
				return "Error: Invalid start date.";
			}

			if (!DateTime.TryParse(additionalParam[3] as string, out DateTime dueDate))
			{
				return "Error: Invalid due date.";
			}

			if (!DateTime.TryParse(additionalParam[4] as string, out DateTime returnedDate))
			{
				return "Error: Invalid returned date.";
			}

			try
			{
				_dbHan.InsertRentalDB(memberId, startDate, dueDate, returnedDate, rentStatus);
				return "Rental Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"An error occurred while adding the rental: {ex.Message}";
			}
		}

		public string UpdateRental(Rental r)
		{
			if (r == null)

			{
				return "Error: Rental cannot be null.";
			}

			try
			{
				return _dbHan.UpdateRentalDB(r);
			}

			catch (Exception ex)
			{
				return $"An error occurred while updating the rental: {ex.Message}";
			}
		}

		public string DeleteRental(Rental r)
		{
			if (r == null)
			{
				return "Error: Magazine cannot be null.";
			}

			try
			{
				return _dbHan.DeleteRentalDB(r.RentalId);
			}

			catch (Exception ex)
			{
				return $"An error occurred while deleting the Rental: {ex.Message}";
			}
		}

		public List<Rental> LoadAllRentals()
		{
			return _dbHan.LoadRentalsFromDB().OfType<Rental>().ToList();
		}

		public Rental LoadRentalsById(int RentalId)
		{
			return _dbHan.LoadRentalsFromDB().OfType<Rental>().FirstOrDefault(r => r.RentalId == RentalId);
		}
	}
}
