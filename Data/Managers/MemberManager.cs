using OOP2FinalProjectLibrary.Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP2FinalProjectLibrary.Data.Managers
{
    public class MemberManager
    {
		public static List<Member> members;
		private readonly DBHandler _dbHan;

		public MemberManager(DBHandler dbHan)
		{
			_dbHan = dbHan;
		}

		public string AddMember(string lastName, string firstName, string phone, string email, string address, params object[] additionalParam)
		{
			if (additionalParam.Length < 1)
			{
				return "Error: Missing additional parameters for Rental.";
			}

			//parse the additional parameters
			if (!int.TryParse(additionalParam[0] as string, out int memberId))
			{
				return "Error: Invalid member ID.";
			}

			try
			{
				_dbHan.InsertMemberDB(lastName, firstName, phone, email, address);
				return "Member Added Successfully!";
			}

			catch (Exception ex)
			{
				return $"An error occurred while adding the member: {ex.Message}";
			}
		}

		public string UpdateMember(Member mem)
		{
			if (mem == null)
			{
				return "Error: Member cannot be null.";
			}

			try
			{
				return _dbHan.UpdateMemberDB(mem);
			}
			catch (Exception ex)
			{
				return $"An error occurred while updating the member: {ex.Message}";
			}
		}

		public string DeleteMember(Member mem)
		{
			if (mem == null)
			{
				return "Error: Member cannot be null.";
			}

			try
			{
				return _dbHan.DeleteMemberDB(mem.MemberId);
			}

			catch (Exception ex)
			{
				return $"An error occurred while deleting the Member: {ex.Message}";
			}
		}

		public List<Member> LoadAllMembers()
		{
			return _dbHan.LoadMembersFromDB().OfType<Member>().ToList();
		}

		public Member LoadItemById(int memberId)
		{
			return _dbHan.LoadMembersFromDB().OfType<Member>().FirstOrDefault(mem => mem.MemberId == memberId);
		}
	}
}

